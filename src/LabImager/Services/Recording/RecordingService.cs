using System.Windows.Threading;

namespace LabImager.Services.Recording;

public sealed class RecordingService : IRecordingService
{
    private readonly DispatcherTimer _timer;
    private DateTime? _startedAt;
    private TimeSpan _accumulated;

    public RecordingState State { get; private set; } = RecordingState.Idle;

    public TimeSpan Elapsed =>
        State == RecordingState.Recording && _startedAt.HasValue
            ? _accumulated + (DateTime.Now - _startedAt.Value)
            : _accumulated;

    public event EventHandler? StateChanged;
    public event EventHandler? ElapsedChanged;

    public RecordingService()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += (_, _) => ElapsedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Start()
    {
        _accumulated = TimeSpan.Zero;
        _startedAt = DateTime.Now;
        SetState(RecordingState.Recording);
        _timer.Start();
        ElapsedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Pause()
    {
        if (State != RecordingState.Recording)
            return;

        if (_startedAt.HasValue)
            _accumulated += DateTime.Now - _startedAt.Value;

        _startedAt = null;
        SetState(RecordingState.Paused);
        _timer.Stop();
        ElapsedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Resume()
    {
        if (State != RecordingState.Paused)
            return;

        _startedAt = DateTime.Now;
        SetState(RecordingState.Recording);
        _timer.Start();
        ElapsedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        if (State == RecordingState.Recording && _startedAt.HasValue)
            _accumulated += DateTime.Now - _startedAt.Value;

        _startedAt = null;
        _timer.Stop();
        SetState(RecordingState.Stopped);
        ElapsedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Reset()
    {
        _startedAt = null;
        _accumulated = TimeSpan.Zero;
        _timer.Stop();
        SetState(RecordingState.Idle);
        ElapsedChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetState(RecordingState state)
    {
        if (State == state)
            return;

        State = state;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
