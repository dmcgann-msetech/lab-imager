using DirectShowLib;

namespace LabImager.Services.Preview;

internal sealed class DirectShowSampleGrabberProbe : ISampleGrabberCB
{
    public int SampleCB(double sampleTime, IMediaSample mediaSample)
    {
        return 0;
    }

    public int BufferCB(double sampleTime, System.IntPtr buffer, int bufferLength)
    {
        return 0;
    }

    public static void CompileProbe()
    {
        var sampleGrabberFilter = (IBaseFilter)new SampleGrabber();
        var sampleGrabber = (ISampleGrabber)sampleGrabberFilter;

        var mediaType = new AMMediaType
        {
            majorType = MediaType.Video,
            subType = MediaSubType.RGB24,
            formatType = FormatType.VideoInfo
        };

        sampleGrabber.SetMediaType(mediaType);
        sampleGrabber.SetBufferSamples(false);
        sampleGrabber.SetOneShot(false);
        sampleGrabber.SetCallback(new DirectShowSampleGrabberProbe(), 1);
    }
}
