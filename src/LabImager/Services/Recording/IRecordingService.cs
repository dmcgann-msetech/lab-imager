namespace LabImager.Services.Recording;

public interface IRecordingService
{
    RecordingState State { get; }
    TimeSpan Elapsed { get; }

    event EventHandler? StateChanged;
    event EventHandler? ElapsedChanged;

    void Start();
    void Pause();
    void Resume();
    void Stop();
    void Reset();
}
