using System.ComponentModel.DataAnnotations;

namespace SquareDMS.DataLibrary.Entities
{
    public class FileFormat : IDataTransferObject
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

        /// <summary>
        /// Id of the entity
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// An all lowercase indication of the fileformat.
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Extension { get; private set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Description { get; private set; }
    }
}