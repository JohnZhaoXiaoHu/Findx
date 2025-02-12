﻿using System;
namespace Findx.Utils
{
    /// <summary>
    /// 雪花算法生成ID
    /// </summary>
    public class SnowflakeId
    {
        public const long Twepoch = 1288834974657L;

        private const int WorkerIdBits = 5;
        private const int DatacenterIdBits = 5;
        private const int SequenceBits = 12;
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private static SnowflakeId _snowflakeId;

        private readonly object _lock = new object();
        private static readonly object SLock = new object();
        private long _lastTimestamp = -1L;

        public SnowflakeId(long workerId, long datacenterId, long sequence = 0L)
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            Sequence = sequence;

            // sanity check for workerId
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
                throw new ArgumentException($"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
        }

        public long WorkerId { get; protected set; }
        public long DatacenterId { get; protected set; }

        public long Sequence { get; internal set; }

        public static SnowflakeId Default()
        {
            lock (SLock)
            {
                if (_snowflakeId != null)
                {
                    return _snowflakeId;
                }

                var workerId = RandomUtil.RandomInt((int)MaxWorkerId);
                var datacenterId = RandomUtil.RandomInt((int)MaxDatacenterId);
                return _snowflakeId = new SnowflakeId(workerId, datacenterId);
            }
        }

        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                    throw new Exception(
                        $"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");

                if (_lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & SequenceMask;
                    if (Sequence == 0) timestamp = TilNextMillis(_lastTimestamp);
                }
                else
                {
                    Sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TimestampLeftShift) |
                         (DatacenterId << DatacenterIdShift) |
                         (WorkerId << WorkerIdShift) | Sequence;

                return id;
            }
        }

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) timestamp = TimeGen();
            return timestamp;
        }

        protected virtual long TimeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
