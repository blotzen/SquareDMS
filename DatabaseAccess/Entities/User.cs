namespace SquareDMS.DatabaseAccess.Entities
{
    public class User : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public User() { }

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

        public int Id { get; private set; }

        public string LastName { get; private set; }

        public string FirstName { get; private set; }

        public string UserName { get; private set; }

        public string Email { get; private set; }

        public byte[] PasswordHash { get; private set; }

        public bool? Active { get; private set; }
    }
}
