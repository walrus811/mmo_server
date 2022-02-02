using ServerCore;
using System.Net;
using System.Net.Sockets;

string host = Dns.GetHostName();
var ipHost = Dns.GetHostEntry(host);
var ipAddr = ipHost.AddressList[0];
var endPoint = new IPEndPoint(ipAddr, 7777);

Console.WriteLine("Listening...");
var listener=new Listener();
listener.Init(endPoint, () => new GameSession());

while (true)
{
    ;
}
