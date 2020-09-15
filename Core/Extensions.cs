using System;
using System.Threading.Tasks;

namespace SquareDMS.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Task fire and forget
        /// </summary>
        /// <param name="task">Task to fire and forget</param>
        public static async void FireAndForget<T>(this Task<T> task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                // TODO: log errors in fire and forget
                // log errors
            }
        }
    }
}
