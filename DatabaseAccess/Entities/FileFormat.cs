namespace SquareDMS.DatabaseAccess.Entities
{
    public class FileFormat : IEntity
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public FileFormat() { }

        public FileFormat(string extension, string description)
        {
            Extension = extension;
            Description = description;
        }

        public int Id { get; private set; }

        /// <summary>
        /// An all lowercase indication of the fileformat.
        /// </summary>
        public string Extension { get; private set; }

        public string Description { get; private set; }
    }
}