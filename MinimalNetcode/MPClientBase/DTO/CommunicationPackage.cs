namespace MPClientBase.DTO
{
    public abstract class CommunicationPackage<TDataDTO> : IDataPack
        where TDataDTO : IDataPackTyped<TDataDTO>
    {
        public TDataDTO Data;

        public abstract byte[] GetBytes();// => Data.GetBytes();

        protected CommunicationPackage() { }

        public CommunicationPackage(TDataDTO inData)
        {
            Data = inData;
        }
    }


    public class CPWithType<T, EPT> : CommunicationPackage<T>
        where T : IDataPackTyped<T>, new()
        where EPT : Enum
    {
        public EPT Purpose;

        public CPWithType() { }

        public CPWithType(EPT purpose, T input)
        {
            Purpose = purpose;
            Data = input;
        }

        public static CPWithType<T, EPT> Parse(byte[] data)
        {
            var purpose = (EPT)Enum.ToObject(typeof(EPT), data[0]);
            var dataBuff = new byte[data.Length - 1];

            Buffer.BlockCopy(data, 1, dataBuff, 0, dataBuff.Length);

            return new CPWithType<T, EPT>(purpose, new T().ParseIn(dataBuff));
        }

        public override byte[] GetBytes()
        {
            var bytes = Data.GetBytes();
            var buffer = new byte[bytes.Length + 1];
            Buffer.BlockCopy(bytes, 0, buffer, 1, bytes.Length);
            buffer[0] = Convert.ToByte(Purpose);
            return buffer;
        }
    }
}
