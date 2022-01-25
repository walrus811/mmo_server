using System.Net;
using System.Net.Sockets;
using System.Text;

// DNS
string host = Dns.GetHostName();
var ipHost = Dns.GetHostEntry(host);
var ipAddr = ipHost.AddressList[0];
var endPoint = new IPEndPoint(ipAddr, 7777);

while (true)
{
    try
    {
        // 휴대폰 설정
        var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // 문지기에게 입장 문의
        socket.Connect(endPoint);
        Console.WriteLine($"Connected To {socket.RemoteEndPoint}");

        // 보낸다
        var sendBuff = Encoding.UTF8.GetBytes("Hello World!");
        int sendBytes = socket.Send(sendBuff);

        // 받는다
        var recvBuff = new byte[1024];
        int recvBytes = socket.Receive(recvBuff);
        string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
        Console.WriteLine($"[From Server] {recvData}");

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        Thread.Sleep(100);

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}