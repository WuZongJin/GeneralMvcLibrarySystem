﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using General.Framework.Security.Admin;
using General.Core;
using General.Core.Extensions;

namespace General.Framework.Filters
{

    /// <summary>
    /// 权限判断过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionActionFilterAttribute : Attribute, IActionFilter
    {
        private readonly bool _dontValidate;

        public PermissionActionFilterAttribute() : this(false)
        {

        }
        public PermissionActionFilterAttribute(bool dontValidate)
        {
            this._dontValidate = dontValidate;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        /// <summary>
        /// 允许匿名访问
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private bool getAllowAttributes(ActionExecutingContext descriptor)
        {
            return descriptor.ActionDescriptor.FilterDescriptors.Any(o => o.Filter.GetType().Name.Equals("AllowAnonymousAttribute"));
        }

        /// <summary>
        /// 只有添加此特性的控制器方法才启用权限验证
        /// 未添加的不判断权限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool isPermissionPageRequested(ActionExecutingContext context)
        {
            return context.ActionDescriptor.FilterDescriptors.Any(o => o.Filter.GetType().Name.Equals(this.GetType().Name));
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (_dontValidate)
                return;
            if (getAllowAttributes(context))
                return;
            if (isPermissionPageRequested(context))
            { 
                if (!EnginContext.Current.Resolve<IAdminAuthService>().authorize(context))
                    handleRequest(context);
            }
        }

        /// <summary>
        /// 处理结果，跳转登录界面
        /// </summary>
        /// <param name="context"></param>
        private void handleRequest(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = new JsonResult(new AjaxResult() { Status = false, Message = "您没有权限" });
            }
            else
            {
                context.Result = new ViewResult() { ViewName = "NotPermission" };
            }
        }
    }
}
