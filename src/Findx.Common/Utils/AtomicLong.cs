﻿using System.Threading;

namespace Findx.Utils
{
    /// <summary>
    /// 原子长整型对象
    /// </summary>
    public class AtomicLong
    {
        private long _value;

        public AtomicLong()
            : this(0)
        {
        }

        public AtomicLong(long value)
        {
            _value = value;
        }

        public long Value
        {
            get
            {
                return Interlocked.Read(ref _value);
            }

            set
            {
                Interlocked.Exchange(ref _value, value);
            }
        }

        public bool CompareAndSet(long expected, long update)
        {
            return Interlocked.CompareExchange(ref _value, update, expected) == expected;
        }

        public long GetAndSet(long value)
        {
            return Interlocked.Exchange(ref _value, value);
        }

        public long AddAndGet(long value)
        {
            return Interlocked.Add(ref _value, value);
        }
    }
}
