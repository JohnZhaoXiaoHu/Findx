﻿using System.ComponentModel;

namespace Findx.Module.Admin.Internals
{
    internal enum FileLocationEnum
    {
        /// <summary>
        /// 阿里云
        /// </summary>
        [Description("阿里云")]
        ALIYUN = 1,

        /// <summary>
        /// 腾讯云
        /// </summary>
        [Description("腾讯云")]
        TENCENT = 2,

        /// <summary>
        /// minio服务器
        /// </summary>
        [Description("minio服务器")]
        MINIO = 3,

        /// <summary>
        /// 本地
        /// </summary>
        [Description("本地")]
        LOCAL = 4
    }
}
