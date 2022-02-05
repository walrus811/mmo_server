using ServerCore;
using System.Text;

public enum PacketID
{
    C_PlayerInfoReq = 1,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class C_PlayerInfoReq : IPacket
{
    public byte testByte;
	
	
	public long playerId;
	
	
	public string name;
	
	
	public struct Skill
	{
	    public int id;
		
		
		public short level;
		
		
		public float duration;
		
		
		public struct Attribute
		{
		    public int att;
		
		    public void Read(ReadOnlySpan<byte> s, ref ushort count)
		    {
		        this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
				count += sizeof(int);
				
		    }
		    public bool Write(Span<byte> s , ref ushort count)
		    {
		        bool success = true;
		        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
				count += sizeof(int);
				
		        return success;
		    }
		
		}
		
		public List<Attribute> attributes = new List<Attribute>();
		
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			this.attributes.Clear();
			ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);
			for(int i=0; i<attributeLen; i++)
			{
			    var attribute = new Attribute();
			    attribute.Read(s, ref count);
			    attributes.Add(attribute);
			}
			
	    }
	    public bool Write(Span<byte> s , ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
			count += sizeof(int);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
			count += sizeof(short);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
			count += sizeof(float);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)attributes.Count);
			count += sizeof(ushort);
			
			foreach(var attribute in attributes)
			{
			    success &= attribute.Write(s, ref count);
			}
			
	        return success;
	    }
	
	}
	
	public List<Skill> skills = new List<Skill>();
	

	public ushort Protocol => (ushort)PacketID.C_PlayerInfoReq;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.testByte = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		for(int i=0; i<skillLen; i++)
		{
		    var skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        var segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        var s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length-count),(ushort)PacketID.C_PlayerInfoReq);
        count += sizeof(ushort);

         segment.Array[segment.Offset + count]=(byte)this.testByte;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(long);
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count+sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
		count += sizeof(ushort);
		
		foreach(var skill in skills)
		{
		    success &= skill.Write(s, ref count);
		}
		

        success &= BitConverter.TryWriteBytes(s, count);
        if (!success)
            return null;

        return SendBufferHelper.Close(count);
    }
}
 
