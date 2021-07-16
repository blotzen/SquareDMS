using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.DataLibrary.Entities;

namespace SquareDMS.ActionFilters
{
    /// <summary>
    /// This class serves as action filter for the User Entity
    /// </summary>
    public class UserActionFilter : IActionFilter
    {


        public void OnActionExecuting(ActionExecutingContext context)
        {
            var param = context.ActionArguments.Single();

            if (param.Value is null)
            {
                context.Result = new BadRequestObjectResult("Object is null");
                return;
            }

            var user = param.Value as User;

            if (user.UserName is null || user.UserName.Trim().Equals(string.Empty))
            {
                context.Result = new BadRequestObjectResult("Username is empty");
            }

        }


        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
