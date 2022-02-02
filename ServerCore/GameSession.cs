using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Knight
    {
        public int hp { get; set; }
        public int attack { get; set; }
    }
    /* 엔진과 컨텐츠 분리 */
    public  class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            var knight = new Knight() { hp = 100, attack = 100 };
            var openSegment = SendBufferHelper.Open(4096);
           var buffer = BitConverter.GetBytes(knight.hp);
            var buffer2 = BitConverter.GetBytes(knight.attack);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset, buffer2.Length);
            var sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            Send(sendBuff);

            Thread.Sleep(100);
            Disconnect();
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            var recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
            return buffer.Count;
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
