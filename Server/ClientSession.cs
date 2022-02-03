using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);

    }

    public class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            //option 1 -내부적으로 byte[]를 할당했다!
            //var size = BitConverter.GetBytes(packet.size);
            //var packetId = BitConverter.GetBytes(packet.packetId);
            //var playerId = BitConverter.GetBytes(packet.playerId);


            //Array.Copy(size, 0, s.Array, s.Offset, size.Length);
            //count += 2;
            //Array.Copy(packetId, 0, s.Array, s.Offset + count, packetId.Length);
            //count += 2;
            //Array.Copy(playerId, 0, s.Array, s.Offset + count, playerId.Length);
            //count += 8;

            //option 2 - unsafe로 포인터 연산
            //option 3 - 비트오퍼레이션
            //option 4 - trywrite
            var s = SendBufferHelper.Open(4096);
            ushort count = 0;
            bool success = true;
            //size
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;
            //타입 조심
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            if (!success)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2
    }


    /* 엔진과 컨텐츠 분리 */
    public class ClientSession : PacketSession
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
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch((PacketID)id){
                case PacketID.PlayerInfoReq:
                    {
                        var p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"playerId : {p.playerId}");
                    }
                    break;
            }

            Console.WriteLine($"size : {size}, id : {id}, playerId : ");
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
