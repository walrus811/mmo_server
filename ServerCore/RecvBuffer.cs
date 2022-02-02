using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        private ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;
        public int DataSize => _writePos - _readPos;
        public int FreeSize => _buffer.Count - _writePos;

        public ArraySegment<byte> DataSegment => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize);
        public ArraySegment<byte> FreeSegment => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize);

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize],0,bufferSize);
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {// 남은 데이터 없음
                _readPos = _writePos = 0;
            }
            else
            {// 남은 거 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }

    }
}
