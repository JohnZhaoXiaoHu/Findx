﻿using System;
using System.IO;
using System.IO.Compression;

namespace Findx.Extensions
{
    /// <summary>
    /// 系统扩展 - 字节数组
    /// </summary>
    public partial class Extensions
    {
        /// <summary>
        /// 字节数组压缩
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] bytes)
        {
            if (bytes is null) return Array.Empty<byte>();

            using var output = new MemoryStream();

            using var stream = new BrotliStream(output, CompressionMode.Compress);

            stream.Write(bytes, 0, bytes.Length);

            return output.ToArray();
        }

        /// <summary>
        /// 字节数组解压
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] bytes)
        {
            using var input = new MemoryStream(bytes);

            using var stream = new BrotliStream(input, CompressionMode.Decompress);

            using var output = new MemoryStream();

            stream.CopyTo(output);

            return output.ToArray();
        }
    }
}
