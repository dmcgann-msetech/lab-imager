using DirectShowLib;

namespace LabImager.Services.Preview;

internal static class DirectShowGraphProbe
{
    public static void CompileProbe()
    {
        var graph = (IGraphBuilder)new FilterGraph();

        var captureBuilder =
            (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        captureBuilder.SetFiltergraph(graph);

        var sampleGrabberFilter =
            (IBaseFilter)new SampleGrabber();

        var sampleGrabber =
            (ISampleGrabber)sampleGrabberFilter;

        var nullRenderer =
            (IBaseFilter)new NullRenderer();

        graph.AddFilter(sampleGrabberFilter, "SampleGrabber");
        graph.AddFilter(nullRenderer, "NullRenderer");
    }
}
