﻿using System.ComponentModel.DataAnnotations;

namespace SquareDMS.DataLibrary.Entities
{
    public class DocumentType : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public DocumentType() { }

        /// <summary>
        /// ManipluationResult
        /// </summary>
        public DocumentType(int? id)
        {
            Id = id;
        }

        /// <param name="name">Name of document type</param>
        /// <param name="description">optional description</param>
        public DocumentType(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public int? Id { get; set; }

        /// <summary>
        /// Name of the document type.
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Name { get; set; }

        /// <summary>
        /// Optional description of the document type.
        /// </summary>
        [StringLength(250, ErrorMessage = "Exceeded 250 characters limit")]
        public string Description { get; set; }
    }
}
