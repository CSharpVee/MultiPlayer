﻿using System.Text;

namespace MPClientBase.DTO
{
    public enum PacketType
    {
        UNUSED___ = 0,
        ConnectJoin = 1,
        Reconnect = 2,
        Disconnect = 3,//or server shutdown.
        Heartbeat = 4,
        NormalComms = 5
    }

    public abstract class PacketBase//The lowest base to transport the data. Should really just be used for connection configuration purposes by itself.
    {
        public PacketType Type { get; set; }
        public ushort PacketLength { get; set; }
        public byte[] PacketData { get; set; }
        public abstract byte[] GetBytes();
    }

    public class ClientPacket : PacketBase
    {
        protected ClientPacket() { }
        public override byte[] GetBytes()
        {
            var length_bits = BitConverter.GetBytes(PacketLength);

            var buffer = new byte[1 + 2 + PacketLength];
            buffer[0] = (byte)Type;

            buffer[1] = length_bits[0];
            buffer[2] = length_bits[1];

            Buffer.BlockCopy(PacketData, 0, buffer, 3, PacketLength);

            return buffer;
        }

        public static ClientPacket Construct(PacketType type, string data)
        {
            return new ClientPacket() { Type = type, PacketLength = (ushort)data.Length, PacketData = Encoding.ASCII.GetBytes(data) };
        }

        public static ClientPacket Construct(PacketType type, byte[] data)
        {
            return new ClientPacket() { Type = type, PacketLength = (ushort)data.Length, PacketData = data };
        }

        public static ClientPacket Construct(IDataPack packet)
        {
            return Construct(PacketType.NormalComms, packet.GetBytes());
        }
    }

    public class ServerPacket : PacketBase
    {
        public byte Sync { get; set; }

        protected ServerPacket() { }

        public override byte[] GetBytes()
        {
            var length_bits = BitConverter.GetBytes(PacketLength);

            var buffer = new byte[1 + 1 + 2 + PacketLength];
            buffer[0] = (byte)Type;
            buffer[1] = Sync;

            buffer[2] = length_bits[0];
            buffer[3] = length_bits[1];

            Buffer.BlockCopy(PacketData, 0, buffer, 4, PacketLength);

            return buffer;
        }

        public static ServerPacket Construct(PacketType type, byte sync, string data)
        {
            return new ServerPacket() { Type = type, Sync = sync, PacketLength = (ushort)data.Length, PacketData = Encoding.ASCII.GetBytes(data) };
        }

        public static ServerPacket Construct(PacketType type, byte sync, byte[] data)
        {
            return new ServerPacket() { Type = type, Sync = sync, PacketLength = (ushort)data.Length, PacketData = data };
        }

        public static ServerPacket Construct(byte sync, IDataPack packet)
        {
            return Construct(PacketType.NormalComms, sync, packet.GetBytes());
        }
    }
}
