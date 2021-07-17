using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.DataLibrary.Entities;
using System.Reflection;

namespace SquareDMS.ActionFilters
{
    /// <summary>
    /// This class serves as action filter for the User Entity
    /// </summary>
    public class UserActionFilter : BaseActionFilter, IActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var dto = context.ActionArguments.Single().Value as IDataTransferObject;

            TrimAllProperties(dto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
