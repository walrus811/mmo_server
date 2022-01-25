using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

void onAcceptHandler(Socket socket)
{
    try
    {
        var recvBuff = new byte[1024];
        int recvBytes = socket.Receive(recvBuff);
        var recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
        Console.WriteLine($"[From Client] {recvData}");

        var sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
        socket.Send(sendBuff);

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
};

string host = Dns.GetHostName();
var ipHost = Dns.GetHostEntry(host);
var ipAddr = ipHost.AddressList[0];
var endPoint = new IPEndPoint(ipAddr, 7777);

Console.WriteLine("Listening...");
var listener=new Listener(endPoint, onAcceptHandler);

while (true)
{
    ;
}
