using System.ComponentModel.DataAnnotations;

namespace SquareDMS.DataLibrary.Entities
{
    public class User : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public User() { }

        /// <summary>
        /// ManipulationResult
        /// </summary>
        public User(int? id)
        {
            Id = id;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        public User(string lastName, string firstName, string userName,
            string email, byte[] passwordHash, bool active = true)
        {
            LastName = lastName;
            FirstName = firstName;
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            Active = active;
        }

        public int? Id { get; private set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string LastName { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string FirstName { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string UserName { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Email { get; set; }

        /// <summary>
        /// Password unencrypted
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Password { get; set; }

        public byte[] PasswordHash { get; set; }

        public bool? Active { get; set; }
    }
}
