public interface ISaveObject<T> where T : struct {
    public T GetSaveObjectData();
}
