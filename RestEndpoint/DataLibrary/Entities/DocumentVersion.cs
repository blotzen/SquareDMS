using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace SquareDMS.DataLibrary.Entities
{
    public class DocumentVersion : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public DocumentVersion() { }

        /// <summary>
        /// ManipulationResult
        /// </summary>
        /// <param name="id"></param>
        public DocumentVersion(int? id)
        {
            Id = id;
        }

        public DocumentVersion(int docId, int fileFormatId)
        {
            DocumentId = docId;
            FileFormatId = fileFormatId;
        }

        public int? Id { get; set; }

        public int DocumentId { get; set; }

        public DateTime EventDateTime { get; set; }

        public int VersionNr { get; set; }

        public int FileFormatId { get; set; }

        [JsonIgnore]
        public byte[] TransactionId { get; private set; }

        [JsonIgnore]
        public string FilePath { get; private set; }

        /// <summary>
        /// Binary representation of the payload (used for file download)
        /// </summary>
        [JsonIgnore]
        public byte[] RawFile { get; set; }

        /// <summary>
        /// The uploaded File (Stream)
        /// </summary>
        public IFormFile UploadFile { get; set; }

        /// <summary>
        /// The downloaded File (Stream)
        /// </summary>
        public FileStreamResult DownloadFile { get; set; }
    }
}
