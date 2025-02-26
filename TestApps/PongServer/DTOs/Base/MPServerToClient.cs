using MPClientBase.DTO;

namespace PongServer.DTOs.Base
{
    public enum ServerSendPurpose
    {
        Start = 1,
        Countdown,
        StatusUpdate,
    }

    
    public class MPServerToClient<T> : CPWithType<T, ServerSendPurpose> where T : IDataPackTyped<T>, new()
    {
        public MPServerToClient(ServerSendPurpose purpose, T data) : base(purpose, data)
        {
        }
    }
}
