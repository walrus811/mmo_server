using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DummyClient
{

	//서버의 대리인
	public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");
            var packet = new C_PlayerInfoReq() {  playerId = 1001, name="나는 야인이" };
			var skill = new C_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f, 
				attributes = new List<C_PlayerInfoReq.Skill.Attribute>() { new C_PlayerInfoReq.Skill.Attribute() { att = 123 } } };

			packet.skills.Add(skill);
			
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 102, level = 2, duration = 4.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 103, level = 3, duration = 5.2f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 104, level = 4, duration = 3.1f });

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
