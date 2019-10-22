﻿using General.Framework.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Controllers.Admin
{
    /// <summary>
    /// 需要权限验证的控制器继承
    /// </summary>
    [PermissionActionFilterAttribute]
    public class AdminPermissionController: PublicAdminController
    {

    }
}
