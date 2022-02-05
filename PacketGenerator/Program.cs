
using PacketGenerator;
using System.Xml;

string genPackets = "";
ushort packetId=0;
string packetEnums="";
string clientRegister = "";
string serverRegister = "";

string pdlPath = "../../PDL.xml";

if(args.Length>=1)
    pdlPath = args[0];

var settings = new XmlReaderSettings()
{
    IgnoreComments = true,
    IgnoreWhitespace = true,
};

using (var r = XmlReader.Create(pdlPath, settings))
{
    r.MoveToContent();

    while (r.Read())
    {
        if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
            ParsePacket(r);
    }

    string fileText = string.Format(PacketFormat.fileFormat, packetEnums, genPackets);

    File.WriteAllText("GenPackets.cs", fileText);
    string clientmanagerText = string.Format(PacketFormat.managerFormat, clientRegister);
    File.WriteAllText("ClientPacketManager.cs", clientmanagerText);
    string servermanagerText = string.Format(PacketFormat.managerFormat, serverRegister);
    File.WriteAllText("ServerPacketManager.cs", servermanagerText);
}

void ParsePacket(XmlReader r)
{
    if (r.NodeType == XmlNodeType.EndElement)
        return;

    if (r.Name.ToLower() != "packet")
    {
        Console.WriteLine("Invalid Packet Node");
        return;
    }

    string packetName = r["name"];
    if (string.IsNullOrEmpty(packetName))
    {
        Console.WriteLine("Packet without name");
        return;
    }

    var result = ParseMembers(r);

    genPackets += string.Format(PacketFormat.packetFormat, packetName, result.memberCode, result.readCode, result.writeCode);
    packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + Environment.NewLine + "\t";
    if (packetName.StartsWith("S_") || packetName.StartsWith("s_"))
        clientRegister += string.Format(PacketFormat.managerRegisterForamt, packetName) + Environment.NewLine;
    else
        serverRegister += string.Format(PacketFormat.managerRegisterForamt, packetName) + Environment.NewLine;
}

(string memberCode, string readCode, string writeCode) ParseMembers(XmlReader r)
{
    string packetName = r["name"];

    string memberCode="";
    string readCode="";
    string writeCode="";
    
    int depth = r.Depth + 1;
    while (r.Read())
    {
        if (r.Depth != depth)
            break;

        string memberName = r["name"];
        if (string.IsNullOrEmpty(memberName))
        {
            Console.WriteLine("Member without name");
            return (memberCode, readCode, writeCode);
        }

        if (!string.IsNullOrEmpty(memberCode))
            memberCode += Environment.NewLine;

        if (!string.IsNullOrEmpty(readCode))
            memberCode += Environment.NewLine;

        if (!string.IsNullOrEmpty(writeCode))
            memberCode += Environment.NewLine;

        string memberType = r.Name.ToLower();
        switch (memberType)
        {
            case "byte":
            case "sbyte":
                memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                break;
            case "bool":
            case "short":
            case "ushort":
            case "int":
            case "long":
            case "float":
            case "double":
                memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                break;
            case "string":
                memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                readCode += string.Format(PacketFormat.readStringFormat, memberName);
                writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                break;
            case "list":
                var t = ParseList(r);
                memberCode += t.memberCode;
                readCode += t.readCode;
                writeCode += t.writeCode;
                break;
            default:
                break;
        }
    }

    memberCode = memberCode.Replace(Environment.NewLine,$"{Environment.NewLine}\t");
    readCode = readCode.Replace(Environment.NewLine, $"{Environment.NewLine}\t\t");
    writeCode = writeCode.Replace(Environment.NewLine,$"{Environment.NewLine}\t\t");
    return (memberCode, readCode, writeCode);
}

(string memberCode, string readCode, string writeCode) ParseList(XmlReader r)
{
    string listName = r["name"];

    if(string.IsNullOrEmpty(listName))
    {
        Console.WriteLine("List without name");
        return ("", "", "");
    }

    var t = ParseMembers(r);

    string memberCode = string.Format(PacketFormat.memberListFormat,
        FirstCharToUpper(listName),
        FirstCharToLower(listName),
        t.memberCode,
        t.readCode,
        t.writeCode);

    string readCode = string.Format(PacketFormat.readListFormat,
        FirstCharToUpper(listName),
        FirstCharToLower(listName));


    string writeCode = string.Format(PacketFormat.writeListFormat,
        FirstCharToLower(listName));

    return (memberCode, readCode, writeCode);
}

string ToMemberType(string memberType)
{
    switch (memberType)
    {
        case "bool":
            return "ToBoolean";
        case "short":
            return "ToInt16";
        case "ushort":
            return "ToUInt16";
        case "int":
            return "ToInt32";
        case "long":
            return "ToInt64";
        case "float":
            return "ToSingle";
        case "double":
            return "ToDouble";
        default:
            return "";
    }
}

string FirstCharToUpper(string input)
{
    if (string.IsNullOrEmpty(input))
        return "";
    return input[0].ToString().ToUpper() + input.Substring(1);
}

string FirstCharToLower(string input)
{
    if (string.IsNullOrEmpty(input))
        return "";
    return input[0].ToString().ToLower() + input.Substring(1);
}