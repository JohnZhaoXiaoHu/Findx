﻿using Findx.AspNetCore.Mvc;
using Findx.Module.Admin.Areas.Admin.DTO;
using Findx.Module.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Findx.Module.Admin.Areas.Admin.Controllers
{
    [Area("api/admin")]
    [Route("[area]/sysSms")]
    public class SysSmsController : CrudControllerBase<SysSmsInfo, SysSmsInfo, SysSmsInfo, SysSmsInfo, SysQuery, long, long>
    {
    }
}
