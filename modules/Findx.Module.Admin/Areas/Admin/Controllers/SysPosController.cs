﻿using Findx.AspNetCore.Mvc;
using Findx.Data;
using Findx.Module.Admin.Areas.Admin.DTO;
using Findx.Module.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Findx.Module.Admin.Areas.Admin.Controllers
{

    /// <summary>
    /// 系统-职位-管理
    /// </summary>
    [Authorize(Roles = "admin")]
    [Area("System")]
    [Route("api/system/[controller]/[action]")]
    public class SysPosController : CrudControllerBase<SysPosInfo, SysPosDTO, SysPosCreateRequest, SysPosUpdateRequest, PagerBase, long>
    {

    }
}