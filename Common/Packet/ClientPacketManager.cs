using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    static PacketManager _instance;
    public static PacketManager Instance
    {
        get
        {
            if(_instance == null)
                _instance = new PacketManager();
            return _instance;
        }
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out var onRecv))
            onRecv.Invoke(session, buffer);
    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        var pkt = new T();
        pkt.Read(buffer);

        if (_handler.TryGetValue(pkt.Protocol, out var handler))
            handler.Invoke(session, pkt);
    }
}
