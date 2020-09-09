namespace SquareDMS.DatabaseAccess.Entities
{
    public class Document : IEntity, Core.Models.IDocument
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public Document() { }

        public Document(int creator, int docType,
            string name, bool locked = false, bool discard = false)
        {
            Creator = creator;
            DocumentType = docType;
            Name = name;
            Locked = locked;
            Discard = discard;
        }

        public int Id { get; private set; }

        public int Creator { get; private set; }

        public int DocumentType { get; private set; }

        public string Name { get; private set; }

        public bool Locked { get; private set; }

        public bool Discard { get; private set; }
    }
}
