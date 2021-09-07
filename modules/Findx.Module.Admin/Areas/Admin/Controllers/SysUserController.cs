﻿using Findx.AspNetCore.Mvc;
using Findx.Module.Admin.Areas.Admin.DTO;
using Findx.Module.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Findx.Module.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// 系统用户
    /// </summary>

    [Area("api/admin")]
    [Route("[area]/sysUser")]
    public class SysUserController : CrudControllerBase<SysUserInfo, SysUserInfo, SysUserRequest, SysUserUpdate, SysUserQuery, long, long>
    {
    }
}
