﻿using System.Security.Claims;
using System.Security.Principal;

namespace SquareDMS.RestEndpoint
{
    public static class Extensions
    {
        /// <summary>
        /// Checks the IIdentity for the nameId Claim, converts its value
        /// to int and returns it. Null if not found.
        /// </summary>
        public static int? GetUserIdClaim(this IIdentity identity)
        {
            string nameClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

            if (identity != null)
            {
                var ci = identity as ClaimsIdentity;

                var stringValue = ci?.FindFirst(nameClaim)?.Value;

                if (int.TryParse(stringValue, out int result))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Trims and Lowers the given string if its not null
        /// </summary>
        public static string TrimAndLower(this string input)
        {
            if (input is null)
                return null;

            return input.Trim().ToLower();
        }
    }
}
