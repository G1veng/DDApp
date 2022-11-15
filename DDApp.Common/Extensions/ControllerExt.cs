using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common.Extensions
{
    public static class ControllerExt
    {
        public static String? ControllerAction<T>(this IUrlHelper urlHepler, string name ,object? arg)
            where T : ControllerBase
        {
            var ct = typeof(T);
            var mi = ct.GetMethod(name);
            if (mi == null)
            {
                return null;
            }
            var controller = ct.Name.Replace("Controller", string.Empty);
            var action = urlHepler.Action(name, controller, arg);
            return action;
        }
    }
}
