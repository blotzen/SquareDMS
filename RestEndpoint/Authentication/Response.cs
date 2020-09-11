using SquareDMS.DataLibrary.Entities;

namespace SquareDMS.RestEndpoint.Authentication
{
    /// <summary>
    /// The response for a authentication request.
    /// </summary>
    public class Response
    {
        public Response(User user, string token)
        {
            Id = user.Id;
            UserName = user.UserName;
            Token = token;
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
