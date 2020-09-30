using System.ComponentModel.DataAnnotations;

namespace SquareDMS.DataLibrary.Entities
{
    /// <summary>
    /// Represents a Group in the Square_DB
    /// </summary>
    public class Group : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public Group() { }

        /// <summary>
        /// ManipulationResult
        /// </summary>
        public Group(int? id)
        {
            Id = id;
        }

        /// <param name="admin">If its admin group</param>
        /// <param name="creator">If its creator group</param>
        public Group(string name, string description, bool admin = false, bool creator = false)
        {
            Name = name;
            Description = description;
            Admin = admin;
            Creator = creator;
        }

        public int? Id { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Description { get; set; }

        public bool? Admin { get; set; }

        public bool? Creator { get; set; }
    }
}
