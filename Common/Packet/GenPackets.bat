START ../../PacketGenerator/bin/Debug/PacketGenerator.exe ../../PacketGenerator/PDL.xml
COPY /y GenPackets.cs "../../DummyClient/Packet"
COPY /y GenPackets.cs "../../Server/Packet"
COPY /y ClientPacketManager.cs "../../DummyClient/Packet"
COPY /y ServerPacketManager.cs "../../Server/Packet"