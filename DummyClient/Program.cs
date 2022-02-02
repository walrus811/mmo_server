using DummyClient;
using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

// DNS
string host = Dns.GetHostName();
var ipHost = Dns.GetHostEntry(host);
var ipAddr = ipHost.AddressList[0];
var endPoint = new IPEndPoint(ipAddr, 7777);

var connector = new Connector();
connector.Connect(endPoint, () => new ConnectSession());

while (true)
{
    try
    {

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}