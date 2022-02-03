using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DummyClient
{
    public abstract class Packet
    {
        /* 네트워크의 정보지, 데이터의 모양과는 관련이 없다! */
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

        //클라이언트에서 쓰는 이 부분에서 들어오는 패킷은 절대 믿지 말아야한다.
        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset+count, s.Count - count));
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


    //서버의 대리인
    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");
            var packet = new PlayerInfoReq() {  playerId = 1001 };
            //for (int i = 0; i < 5; i++)
            {
                var s = packet.Write();
                if (s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            var recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transffered bytes : {numOfBytes}");
        }
    }
}
