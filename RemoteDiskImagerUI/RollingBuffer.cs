namespace RemoteDiskImanger;

public class RollingBuffer {
    private byte[] _data;
    private int _start;
    private long _received;

    public int Length => _data.Length;
    public long Received => _received;
    public byte[] Data {
        get {
            byte[] result = new byte[_data.Length];
            if (_start == 0) {
                _data.CopyTo(result, 0);
            } else {
                Array.Copy(_data, _start, result, 0, _data.Length - _start);
                Array.Copy(_data, 0, result, _data.Length - _start, _start);
            }
            return result;
        }
    }


    public RollingBuffer(int capacity) {
        _data = new byte[capacity];
        _start = 0;
    }


    public void Append(byte[] buffer, int offset, int count) {
        count = Math.Min(count, buffer.Length - offset);
        if (count <= 0) return;
        _received += count;

        int effectiveCount = int.Min(count, _data.Length);
        int effectiveOffset = offset + count - effectiveCount;

        if (effectiveCount == _data.Length) {
            Array.Copy(buffer, effectiveOffset, _data, 0, effectiveCount);
            _start = 0;
            return;
        }

        if (_start == 0) {
            Array.Copy(buffer, effectiveOffset, _data, 0, effectiveCount);
            _start = effectiveCount;
            return;
        }
        
        int count1 = Math.Min(effectiveCount, _data.Length - _start);
        int count2 = effectiveCount - count1;
        Array.Copy(buffer, effectiveOffset, _data, _start, count1);
        if (count2 > 0) {
            Array.Copy(buffer, effectiveOffset + count1, _data, 0, count2);
        }
        _start = (_start + effectiveCount) % _data.Length;
    }

    public void Clear() {
        Array.Clear(_data, 0, _data.Length);
        _start = 0;
    }

}
