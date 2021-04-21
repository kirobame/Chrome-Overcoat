namespace Chrome
{
    public interface IModification<T> where T : struct
    {
        bool Update(T initial, T value, out T output);
    }
}