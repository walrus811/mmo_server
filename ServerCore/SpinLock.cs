using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public  class SpinLock
    {
        volatile int _locked = 0;

        public void Aquire()
        {
            while (true)
            {
                //int original = Interlocked.Exchange(ref _locked, 1);
                //if (original == 0)
                //    break;

                //CAS Compare-And-Swap
                int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected)==expected)
                    break;
                //Thread.Sleep(1)
                //Thread.Sleep(0)
                Thread.Yield();
            }
        }

        public void Release()
        {
            _locked=0;
        }
    }
}
