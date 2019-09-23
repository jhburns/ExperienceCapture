namespace Saver
{
    public interface ISaveable
    {
        ICustomStorable getSave();
    }

    public interface ICustomStorable
    {
        // Only use for identification 
    }
}