namespace Chrome
{
    public interface ILink<in T>
    {
        T Link { set; }
    }
}