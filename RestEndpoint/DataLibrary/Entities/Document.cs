using System.ComponentModel.DataAnnotations;

namespace SquareDMS.DataLibrary.Entities
{
    public class Document : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public Document() { }

        public Document(int docType, string name, bool locked = false, bool discard = false)
        {
            DocumentType = docType;
            Name = name;
            Locked = locked;
            Discard = discard;
        }

        public int? Id { get; private set; }

        public int? Creator { get; set; }

        public int? DocumentType { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Name { get; set; }

        public bool? Locked { get; set; }

        public bool? Discard { get; set; }
    }
}
