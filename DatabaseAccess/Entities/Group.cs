namespace SquareDMS.DatabaseAccess.Entities
{
    /// <summary>
    /// Represents a Group in the Square_DB
    /// </summary>
    public class Group : IEntity
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public Group() { }

        /// <param name="admin">If its admin group</param>
        /// <param name="creator">If its creator group</param>
        public Group(string name, string description, bool admin = false, bool creator = false)
        {
            Name = name;
            Description = description;
            Admin = admin;
            Creator = creator;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool Admin { get; private set; }

        public bool Creator { get; private set; }
    }
}
