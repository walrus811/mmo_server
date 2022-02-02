using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    /* 연결 관리 */
    public class Listener
    {
        private Socket _listenSocket;

        private Func<Session> sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> _sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sessionFactory = _sessionFactory;
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);

            //다수의 소켓을 받고 싶으면 여러개 돌리면 됨
            var args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAccept(args);
        }

        public void RegisterAccept(SocketAsyncEventArgs args)
        {
            //이전에 열린 소켓 정보 초기화
            args.AcceptSocket = null;

            bool pending=_listenSocket.AcceptAsync(args);
            if (!pending)
                OnAcceptCompleted(null, args);
        }
        /*
         * 만약 pending이 계속 true로 동기적으로 처리된다면 RegiesterAccept->OnAcceptCompleted 가 계속 실행 되어
         * 스택 오버 플로가 발생할 수도 있음.
         * 근데 소켓을 한번에 하나만 받는 것도 아니고 그다지 현실적인 이야기는 아님
         */

        /*
         * 비동기기 떄문에 이제 이 부분은 크리티컬 섹션이 될 수 있는 영역이 되어버림.
         */
        public void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                /* 어차피 새 리스너를 받는 부분은 그렇게 자주 벌어지지 않아서 괜찮다는 거 아닐까?*/
                var session = sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError);
            }
            RegisterAccept(args);
        }
    }
}
