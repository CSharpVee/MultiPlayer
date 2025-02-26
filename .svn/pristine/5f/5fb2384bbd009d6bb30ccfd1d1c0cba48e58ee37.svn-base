using System.Net;
using System.Net.Sockets;
using MPModuleBase.Communications;

namespace MultiPlayer.Server.Communications
{
    public class UDP_ClientConnection : AClientConnection
    {
        public override IPEndPoint Endpoint => _endptIP;

        public override bool IsActive => true;//Connectionless. So it's Shcrodinger's style both connected and unconnected until observed I guess?

        public override bool IsDataAvailable => _dataPacketQueue.Count > 0;//and this could be other

        private UdpClient _mainServerInst;
        private IPEndPoint _endptIP;

        private List<byte[]> _dataPacketQueue;
        private int _bytePointer = 0;

        public UDP_ClientConnection(UdpClient serverInst, IPEndPoint endpoint)
        {
            _dataPacketQueue = new List<byte[]>();
            _endptIP = endpoint;
            _mainServerInst = serverInst;//don't ever close this anywhere inside!!
        }

        public void PushNewData(byte[] data)
        {
            _dataPacketQueue.Insert(0, data);
        }

        public override void Close(string reason)
        {
            //don't ever close. Uses same UDP client instance
        }

        public override byte[] ReceiveBytes(ushort amount)
        {
            var result = new byte[amount];
            var lastIndex = _dataPacketQueue.Count - 1;//because we insert new data to the list[0]
            var data = _dataPacketQueue[lastIndex];
            
            var packetFinished = _bytePointer + amount >= data.Length;
            var copyAmt = (!packetFinished) ? amount : data.Length - _bytePointer;

            Buffer.BlockCopy(data, _bytePointer, result, 0, copyAmt);

            if (packetFinished)
            {
                _bytePointer = 0;
                _dataPacketQueue.RemoveAt(lastIndex);
            }
            else _bytePointer += copyAmt;
            return result;
        }

        public override void SendBytes(byte[] data)
        {
            _mainServerInst.Send(data, data.Length, _endptIP);// check what happens if client app closes. Might need to have an "ACTIVE" flag, that unsets here.
        }
    }
}
