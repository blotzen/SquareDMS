using System.ComponentModel.DataAnnotations;

namespace SquareDMS.Core.Models
{
    public class FileFormat : DatabaseAccess.Entities.FileFormat
    {
        private string _extension;

        /// <summary>
        /// Default constructor to create empty FileFormat.
        /// </summary>
        public FileFormat() { }

        /// <param name="extension"></param>
        /// <param name="description"></param>
        public FileFormat(string extension, string description) :
            base(extension, description)
        { }

        /// <summary>
        /// An all uppercase indication of the fileformat.
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public new string Extension
        {
            get => _extension;

            set
            {
                _extension = value.ToUpper();
            }
        }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public new string Description { get; set; }
    }
}
