using System.Runtime.InteropServices;
using DirectShowLib;
using LabImager.Models.Camera;

namespace LabImager.Services.Camera;

public sealed class DirectShowCameraDeviceService : ICameraDeviceService
{
    public IReadOnlyList<CameraDeviceInfo> GetAvailableDevices()
    {
        var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

        return devices
            .Select(device => new CameraDeviceInfo
            {
                Name = device.Name,
                DevicePath = device.DevicePath
            })
            .ToList();
    }

    public IReadOnlyList<CameraCaptureFormat> GetAvailableFormats(
        CameraDeviceInfo device)
    {
        var formats = new List<CameraCaptureFormat>();

        if (string.IsNullOrWhiteSpace(device.DevicePath) &&
            string.IsNullOrWhiteSpace(device.Name))
        {
            return formats;
        }

        var directShowDevice = DsDevice
            .GetDevicesOfCat(FilterCategory.VideoInputDevice)
            .FirstOrDefault(candidate =>
                string.Equals(candidate.DevicePath, device.DevicePath, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(candidate.Name, device.Name, StringComparison.OrdinalIgnoreCase));

        if (directShowDevice == null)
        {
            return formats;
        }

        IGraphBuilder? graphBuilder = null;
        ICaptureGraphBuilder2? captureBuilder = null;
        IBaseFilter? sourceFilter = null;
        IAMStreamConfig? streamConfig = null;
        IntPtr capsPointer = IntPtr.Zero;

        try
        {
            graphBuilder = (IGraphBuilder)new FilterGraph();
            captureBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

            DsError.ThrowExceptionForHR(
                captureBuilder.SetFiltergraph(graphBuilder));

            object sourceObject;
            var filterGuid = typeof(IBaseFilter).GUID;

            directShowDevice.Mon.BindToObject(
                null!,
                null!,
                ref filterGuid,
                out sourceObject);

            sourceFilter = (IBaseFilter)sourceObject;

            DsError.ThrowExceptionForHR(
                graphBuilder.AddFilter(sourceFilter, directShowDevice.Name));

            var streamConfigGuid = typeof(IAMStreamConfig).GUID;

            DsError.ThrowExceptionForHR(
                captureBuilder.FindInterface(
                    PinCategory.Capture,
                    MediaType.Video,
                    sourceFilter,
                    streamConfigGuid,
                    out var configObject));

            streamConfig = (IAMStreamConfig)configObject;

            DsError.ThrowExceptionForHR(
                streamConfig.GetNumberOfCapabilities(
                    out var count,
                    out var size));

            capsPointer = Marshal.AllocHGlobal(size);

            for (var i = 0; i < count; i++)
            {
                AMMediaType? mediaType = null;

                try
                {
                    DsError.ThrowExceptionForHR(
                        streamConfig.GetStreamCaps(
                            i,
                            out mediaType,
                            capsPointer));

                    if (mediaType == null ||
                        mediaType!.formatPtr == IntPtr.Zero ||
                        mediaType!.formatType != FormatType.VideoInfo)
                    {
                        continue;
                    }

                    var info = Marshal.PtrToStructure<VideoInfoHeader>(
                        mediaType!.formatPtr);

                    var width = info.BmiHeader.Width;
                    var height = Math.Abs(info.BmiHeader.Height);
                    var fps = info.AvgTimePerFrame > 0
                        ? 10000000.0 / info.AvgTimePerFrame
                        : 0;

                    var subtypeName = GetSubtypeName(mediaType!.subType);

                    formats.Add(new CameraCaptureFormat
                    {
                        Width = width,
                        Height = height,
                        FramesPerSecond = Math.Round(fps, 2),
                        SubType = subtypeName,
                        IsAvailable = true,
                        DisplayName = $"{width}x{height} @ {fps:0.##} FPS - {subtypeName}"
                    });
                }
                finally
                {
                    if (mediaType != null)
                    {
                        DsUtils.FreeAMMediaType(mediaType);
                    }
                }
            }
        }
        catch
        {
            return formats;
        }
        finally
        {
            if (capsPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(capsPointer);
            }

            ReleaseComObject(streamConfig);
            ReleaseComObject(sourceFilter);
            ReleaseComObject(captureBuilder);
            ReleaseComObject(graphBuilder);
        }

        return formats
            .GroupBy(format => format.DisplayName)
            .Select(group => group.First())
            .OrderByDescending(format => ScoreFormat(format))
            .ToList();
    }

    private static double ScoreFormat(CameraCaptureFormat format)
    {
        if (format.Width == 1280 && format.Height == 720 &&
            format.FramesPerSecond >= 59 &&
            string.Equals(format.SubType, "MJPG", StringComparison.OrdinalIgnoreCase))
        {
            return 100000;
        }

        if (format.Width == 1920 && format.Height == 1080 &&
            format.FramesPerSecond >= 59 &&
            string.Equals(format.SubType, "MJPG", StringComparison.OrdinalIgnoreCase))
        {
            return 90000;
        }

        if (format.Width == 1280 && format.Height == 720 &&
            format.FramesPerSecond >= 59)
        {
            return 80000;
        }

        return (format.Width * format.Height) + format.FramesPerSecond;
    }

    private static string GetSubtypeName(Guid subtype)
    {
        var subtypeMap = new Dictionary<Guid, string>
        {
            [MediaSubType.MJPG] = "MJPG",
            [MediaSubType.YUY2] = "YUY2",
            [MediaSubType.NV12] = "NV12",
            [FourCcToGuid("H264")] = "H264",
            [FourCcToGuid("H265")] = "H265",
            [FourCcToGuid("HEVC")] = "HEVC",
            [FourCcToGuid("I420")] = "I420",
            [FourCcToGuid("YV12")] = "YV12"
        };

        return subtypeMap.TryGetValue(subtype, out var name)
            ? name
            : subtype.ToString();
    }

    private static Guid FourCcToGuid(string fourCc)
    {
        var bytes = System.Text.Encoding.ASCII.GetBytes(fourCc);
        var value = BitConverter.ToUInt32(bytes, 0);

        return new Guid(
            (int)value,
            0x0000,
            0x0010,
            0x80,
            0x00,
            0x00,
            0xaa,
            0x00,
            0x38,
            0x9b,
            0x71);
    }

    private static void ReleaseComObject(object? comObject)
    {
        if (comObject != null && Marshal.IsComObject(comObject))
        {
            Marshal.ReleaseComObject(comObject);
        }
    }
}


