using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DirectShowLib;

Console.WriteLine("Lab Imager Recording Probe");
Console.WriteLine("==========================");

var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

if (devices.Length == 0)
{
    Console.WriteLine("No video input devices found.");
    return;
}

for (var i = 0; i < devices.Length; i++)
{
    Console.WriteLine($"{i}: {devices[i].Name}");
}

Console.Write("Select device index: ");
var input = Console.ReadLine();

if (!int.TryParse(input, out var selectedIndex) ||
    selectedIndex < 0 ||
    selectedIndex >= devices.Length)
{
    Console.WriteLine("Invalid selection.");
    return;
}

var device = devices[selectedIndex];

var outputDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
    "Lab Imager",
    "Recording Probe");

Directory.CreateDirectory(outputDir);

var outputPath = Path.Combine(
    outputDir,
    $"recording-probe-{DateTime.Now:yyyyMMdd-HHmmss}.avi");

IGraphBuilder? graphBuilder = null;
ICaptureGraphBuilder2? captureBuilder = null;
IBaseFilter? sourceFilter = null;
IBaseFilter? muxFilter = null;
IFileSinkFilter? sinkFilter = null;
IMediaControl? mediaControl = null;
AMMediaType? selectedMediaType = null;

try
{
    graphBuilder = (IGraphBuilder)new FilterGraph();
    captureBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

    DsError.ThrowExceptionForHR(captureBuilder.SetFiltergraph(graphBuilder));

    object sourceObject;
    var filterGuid = typeof(IBaseFilter).GUID;

    device.Mon.BindToObject(null!, null!, ref filterGuid, out sourceObject);
    sourceFilter = (IBaseFilter)sourceObject;

    DsError.ThrowExceptionForHR(graphBuilder.AddFilter(sourceFilter, device.Name));

    selectedMediaType = SelectBestCaptureFormat(captureBuilder, sourceFilter);

    DsError.ThrowExceptionForHR(
        captureBuilder.SetOutputFileName(
            MediaSubType.Avi,
            outputPath,
            out muxFilter,
            out sinkFilter));

    DsError.ThrowExceptionForHR(
        captureBuilder.RenderStream(
            PinCategory.Capture,
            MediaType.Video,
            sourceFilter,
            null,
            muxFilter));

    mediaControl = (IMediaControl)graphBuilder;

    Console.WriteLine($"Recording to: {outputPath}");
    Console.WriteLine("Press ENTER to stop.");

    DsError.ThrowExceptionForHR(mediaControl.Run());

    Console.ReadLine();

    mediaControl.Stop();

    Console.WriteLine("Recording stopped.");
    Console.WriteLine(outputPath);
}
catch (Exception ex)
{
    Console.WriteLine("Recording probe failed:");
    Console.WriteLine(ex);
}
finally
{
    if (selectedMediaType is not null)
    {
        DsUtils.FreeAMMediaType(selectedMediaType);
    }

    Release(mediaControl);
    Release(sinkFilter);
    Release(muxFilter);
    Release(sourceFilter);
    Release(captureBuilder);
    Release(graphBuilder);
}

static AMMediaType? SelectBestCaptureFormat(ICaptureGraphBuilder2 captureBuilder, IBaseFilter sourceFilter)
{
    object configObject;

    var streamConfigGuid = typeof(IAMStreamConfig).GUID;

    DsError.ThrowExceptionForHR(
        captureBuilder.FindInterface(
            PinCategory.Capture,
            MediaType.Video,
            sourceFilter,
            streamConfigGuid,
            out configObject));

    var streamConfig = (IAMStreamConfig)configObject;

    DsError.ThrowExceptionForHR(
        streamConfig.GetNumberOfCapabilities(out var count, out var size));

    var capsPointer = Marshal.AllocHGlobal(size);

    AMMediaType? bestType = null;
    double bestScore = double.MinValue;

    Console.WriteLine();
    Console.WriteLine("Supported capture formats:");
    Console.WriteLine("--------------------------");

    try
    {
        for (var i = 0; i < count; i++)
        {
            DsError.ThrowExceptionForHR(
                streamConfig.GetStreamCaps(i, out var mediaType, capsPointer));

            try
            {
                if (mediaType.formatPtr == IntPtr.Zero ||
                    mediaType.formatType != FormatType.VideoInfo)
                {
                    DsUtils.FreeAMMediaType(mediaType);
                    continue;
                }

                var info = Marshal.PtrToStructure<VideoInfoHeader>(mediaType.formatPtr);

                var width = info.BmiHeader.Width;
                var height = Math.Abs(info.BmiHeader.Height);
                var fps = info.AvgTimePerFrame > 0
                    ? 10000000.0 / info.AvgTimePerFrame
                    : 0;

                Console.WriteLine($"{i}: {width}x{height} @ {fps:0.##} fps | {mediaType.subType}");

                var score = ScoreFormat(width, height, fps);

                if (score > bestScore)
                {
                    if (bestType is not null)
                    {
                        DsUtils.FreeAMMediaType(bestType);
                    }

                    bestType = mediaType;
                    bestScore = score;
                }
                else
                {
                    DsUtils.FreeAMMediaType(mediaType);
                }
            }
            catch
            {
                DsUtils.FreeAMMediaType(mediaType);
                throw;
            }
        }

        if (bestType is null)
        {
            Console.WriteLine("No selectable capture format found. Using camera default.");
            return null;
        }

        var selectedInfo = Marshal.PtrToStructure<VideoInfoHeader>(bestType.formatPtr);
        var selectedWidth = selectedInfo.BmiHeader.Width;
        var selectedHeight = Math.Abs(selectedInfo.BmiHeader.Height);
        var selectedFps = selectedInfo.AvgTimePerFrame > 0
            ? 10000000.0 / selectedInfo.AvgTimePerFrame
            : 0;

        Console.WriteLine();
        Console.WriteLine($"Selected format: {selectedWidth}x{selectedHeight} @ {selectedFps:0.##} fps");

        DsError.ThrowExceptionForHR(streamConfig.SetFormat(bestType));

        return bestType;
    }
    finally
    {
        Marshal.FreeHGlobal(capsPointer);
        Release(streamConfig);
    }
}

static double ScoreFormat(int width, int height, double fps)
{
    if (width == 1280 && height == 720 && fps >= 59)
        return 100000;

    if (width == 1920 && height == 1080 && fps >= 29)
        return 90000;

    if (width == 1280 && height == 720 && fps >= 29)
        return 80000;

    return (width * height) + fps;
}

static void Release(object? obj)
{
    if (obj != null && Marshal.IsComObject(obj))
    {
        Marshal.ReleaseComObject(obj);
    }
}
