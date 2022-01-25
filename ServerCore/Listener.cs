using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        public delegate void AcceptHandler(Socket sokcet);

        private Socket _listenSocket;
        private event AcceptHandler _onAcceptHandler;

        public Listener(IPEndPoint endPoint, AcceptHandler onAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);

            var args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAccept(args);
        }

        public void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending=_listenSocket.AcceptAsync(args);
            if (!pending)
                OnAcceptCompleted(null, args);
        }

        public void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                if(args.AcceptSocket!=null)
                    _onAcceptHandler.Invoke(args.AcceptSocket);
                else
                    Console.WriteLine("There's no accept socket!");
            }
            else
            {
                Console.WriteLine(args.SocketError);
            }
            RegisterAccept(args);
        }
    }
}
