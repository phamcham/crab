public interface IUnitTask {
    public void StartDoTask();
    public void EndDoTask();
    public bool IsTaskRunning { get; }
}