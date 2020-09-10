using System.ComponentModel.DataAnnotations;

namespace SquareDMS.Core.Models
{
    /// <summary>
    /// Derieves from <see cref="DatabaseAccess.Entities.User"/>.
    /// Checks the input length of the string parameters and validates 
    /// them to a max length of 250.
    /// </summary>
    public class User : DatabaseAccess.Entities.User
    {
        private string _userName, _email;

        /// <summary>
        /// Default constructor to create empty User.
        /// </summary>
        public User() { }

        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <param name="active"></param>
        public User(string lastName, string firstName, string userName,
            string email, byte[] passwordHash, bool active = true) :
            base(lastName, firstName, userName, email, passwordHash, active)
        { }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public new string LastName { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public new string FirstName { get; set; }

        /// <summary>
        /// An all lowercase indication of the userName.
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public new string UserName
        {
            get => _userName;
            set => _userName = value.ToLower();
        }

        /// <summary>
        /// An all lowercase indication of the email.
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public new string Email
        {
            get => _email;
            set => _email = value.ToLower();
        }
    }
}