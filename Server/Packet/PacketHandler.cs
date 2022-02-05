using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        var p = packet as C_PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");

        foreach(var skill in p.skills)
        {
            Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
        }
    }
}
