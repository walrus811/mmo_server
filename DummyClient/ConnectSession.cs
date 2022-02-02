using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    public class ConnectSession : GameSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");
            for (int i = 0; i < 5; i++)
            {
                // 보낸다
                var sendBuff = Encoding.UTF8.GetBytes("Hello World!");
                Send(sendBuff);
            }
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            var recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
        }
    }
}
