using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    /* 엔진과 컨텐츠 분리 */
    public  class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

           // var packet = new Packet() { size = 100, packetId = 13 };
           // var openSegment = SendBufferHelper.Open(4096);
           //var buffer = BitConverter.GetBytes(packet.size);
           // var buffer2 = BitConverter.GetBytes(packet.packetId);
           // Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
           // Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset, buffer2.Length);
           // var sendBuff = SendBufferHelper.Close(packet.size);
           // Send(sendBuff);a

            Thread.Sleep(5000);
            Disconnect();
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"size : {size}, id : {id}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transffered bytes : {numOfBytes}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

    }
}
