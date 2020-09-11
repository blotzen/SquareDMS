using System.ComponentModel.DataAnnotations;

namespace SquareDMS.RestEndpoint.Authentication
{
    public class Request
    {
        /// <summary>
        /// Username of the user making the request.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Password of the user making the request.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
