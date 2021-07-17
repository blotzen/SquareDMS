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

        public int? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(250)]
        public string LastName { get; set; }

        [StringLength(250)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(250)]
        public string UserName { get; set; }

        [StringLength(250)]
        public string Email { get; set; }

        /// <summary>
        /// Password unencrypted
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(250)]
        public string Password { get; set; }

        public byte[] PasswordHash { get; set; }

        public bool? Active { get; set; }
    }
}
