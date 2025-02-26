namespace MPClientBase.DTO
{
    public interface IDataPack
    {
        byte[] GetBytes();
    }

    public interface IDataPackTyped<T> : IDataPack
    {
        T ParseIn(byte[] data);
    }
}
