namespace eKYC.Worker;

public sealed class WorkerOptions
{
    public int PollIntervalSeconds { get; set; } = 5;
    public int SimulatedProcessingMilliseconds { get; set; } = 900;
}
