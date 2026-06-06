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
    Release(mediaControl);
    Release(sinkFilter);
    Release(muxFilter);
    Release(sourceFilter);
    Release(captureBuilder);
    Release(graphBuilder);
}

static void Release(object? obj)
{
    if (obj != null && Marshal.IsComObject(obj))
    {
        Marshal.ReleaseComObject(obj);
    }
}
