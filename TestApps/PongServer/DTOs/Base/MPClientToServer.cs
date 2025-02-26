using MPClientBase.DTO;

namespace PongServer.DTOs.Base
{
    public enum ClientSendPurpose
    {
        InputState = 1,
        StartConfirmed = 2,
        //SyncRequest ?
    }

    public class MPClientToServer<T> : CPWithType<T, ClientSendPurpose> where T : IDataPackTyped<T>, new()
    {

        public MPClientToServer(ClientSendPurpose purpose, T data) : base(purpose, data)
        {
        }
    }

}
