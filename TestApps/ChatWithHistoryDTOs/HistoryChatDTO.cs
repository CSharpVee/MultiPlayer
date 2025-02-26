using MPClientBase.DTO;
using System.Text;

namespace ChatWithHistory
{

    public enum Purpose
    {
        ChangeName = 1,
        SendMessage = 2,
        RequestChatHistory = 3,
        //server-to-client
        LastMessage = 4,
        ChatHistory,
    }


    public class HistoryChatPackage<T> : CPWithType<T, Purpose>
        where T : IDataPackTyped<T>, new()
    {
        public HistoryChatPackage(Purpose purpose, T data) : base(purpose, data)
        {
        }
    }


    public class MessageDTO : IDataPackTyped<MessageDTO>
    {
        public string Message { get; private set; }

        public MessageDTO()
        {
        }

        public MessageDTO(string inData)
        {
            Message = inData;
        }

        public byte[] GetBytes() => Encoding.ASCII.GetBytes(Message);

        public MessageDTO ParseIn(byte[] data)
        {
            Message = Encoding.ASCII.GetString(data);
            return this;
        }
    }

    public class NameChangeDTO : IDataPackTyped<NameChangeDTO>
    {
        public string NewName { get; private set; }

        public NameChangeDTO()
        {
        }

        public NameChangeDTO(string inName)
        {
            NewName = inName;
        }

        public byte[] GetBytes() => Encoding.ASCII.GetBytes(NewName);

        public NameChangeDTO ParseIn(byte[] data)
        {
            NewName = Encoding.ASCII.GetString(data);
            return this;
        }
    }
}
