using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    // 재귀적 락 허용? (YES) WriteLock -> WriteLock OK, WriteLock -> ReadLock OK, ReadLock -> WriteLock NO
    // 스핀락 (5000번 -> Yield)
    public class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_THREAD_ID_MASK = 0x7FFF0000;
        const int READ_COUNT_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
        // 굳이 thread ID를 쓰는 건 재귀적락을 위함임
        // 맨 앞은 음수가 될 수 있기에 안 씀.
        int _flag = EMPTY_FLAG;
        int _writeCount = 0; // write 락 자체가 배타적이라 내부적으로 카운트로 관리해도 됨

        public void WriteLock()
        {
            // 동일 스레드가 이미 Write Lock을 흭득했나?
            int lockThreadId = (_flag & WRITE_THREAD_ID_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                _writeCount++;
                return;
            }

            //아무도 WriteLock / ReadLock을 흭득하고 있지 않을 때 경합해서 소유권을 얻는다.
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_THREAD_ID_MASK;
            while (true)
            {
                for(int i=0; i<MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }
                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            int lockCount = --_writeCount;
            if(lockCount==0)
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일 스레드가 이미 Write Lock을 흭득했나?
            int lockThreadId = (_flag & WRITE_THREAD_ID_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            //아무도 WriteLock을 흭득하고 있지 않으면 Read Count++;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    //Read Lock이 동시에 들어와도 순차적으로 처리됨(lock free)
                    int expected = (_flag & READ_COUNT_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;
                }
                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
}
