using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    public class PacketFormat
    {
        // {0} 패킷 등록
        public static string managerFormat =
@"using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{{
    static PacketManager _instance;
    public static PacketManager Instance
    {{
        get
        {{
            if(_instance == null)
                _instance = new PacketManager();
            return _instance;
        }}
    }}

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {{
        {0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out var onRecv))
            onRecv.Invoke(session, buffer);
    }}

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        var pkt = new T();
        pkt.Read(buffer);

        if (_handler.TryGetValue(pkt.Protocol, out var handler))
            handler.Invoke(session, pkt);
    }}
}}
";
        // {0} 패킷 이름
        public static string managerRegisterForamt =
@"
        _onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);";

        // {0} 패킷 이름 / 번호 목록
        // {1} 패킷 목록
        public static string fileFormat =
 @"using ServerCore;
using System.Text;

public enum PacketID
{{
    {0}
}}

public interface IPacket
{{
	ushort Protocol {{ get; }}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}


{1} 
";

        // {0} 패킷 이름
        // {1} 멤버 번호
        public static string packetEnumFormat =
@"{0} = {1},";

        // {0} 패킷 이름
        // {1} 멤버 변수들
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write
        public static string packetFormat =
@"public class {0} : IPacket
{{
    {1}

	public ushort Protocol => (ushort)PacketID.{0};

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        var segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        var s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length-count),(ushort)PacketID.{0});
        count += sizeof(ushort);

        {3}

        success &= BitConverter.TryWriteBytes(s, count);
        if (!success)
            return null;

        return SendBufferHelper.Close(count);
    }}
}}
";
        // {0} 변수 형식
        // {1} 변수 이름
        public static string memberFormat =
@"public {0} {1};";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버 변수들
        // {3} 멤버 변수 Read
        // {4} 멤버 변수 Write
        public static string memberListFormat =
@"public struct {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}
    public bool Write(Span<byte> s , ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}

}}

public List<{0}> {1}s = new List<{0}>();
";

        // {0} 변수 이름
        // {1} To~ 변수 형식
        // {2} 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});
";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});
";

        // {0} 변수 이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, nameLen));
count += {0}Len;
";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string readListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
for(int i=0; i<{1}Len; i++)
{{
    var {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}
";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});
";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeByteFormat =
@" segment.Array[segment.Offset + count]=({1})this.{0};
count += sizeof({1});
";

        // {0} 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count+sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;
";
        // {0} 리스트 이름 [소문자]
        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort){0}s.Count);
count += sizeof(ushort);

foreach(var {0} in {0}s)
{{
    success &= {0}.Write(s, ref count);
}}
";
    }
}