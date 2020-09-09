namespace SquareDMS.DatabaseAccess.Entities
{
    public class DocumentType : IEntity
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public DocumentType() { }

        /// <param name="name">Name of document type</param>
        /// <param name="description">optional description</param>
        public DocumentType(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public int Id { get; private set; }

        /// <summary>
        /// Name of the document type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Optional description of the document type.
        /// </summary>
        public string Description { get; private set; }
    }
}
