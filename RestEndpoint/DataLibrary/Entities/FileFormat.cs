using System.ComponentModel.DataAnnotations;

namespace SquareDMS.DataLibrary.Entities
{
    public class FileFormat : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public FileFormat() { }

        /// <summary>
        /// ManipulationResult
        /// </summary>
        public FileFormat(int? id)
        {
            Id = id;
        }

        public FileFormat(string extension, string description)
        {
            Extension = extension;
            Description = description;
        }

        /// <summary>
        /// Id of the entity
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// An all lowercase indication of the fileformat.
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Extension { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Description { get; set; }
    }
}