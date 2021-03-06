﻿using Microsoft.AspNetCore.Http;
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

        public DocumentVersion(int docId, int fileFormatId)
        {
            DocumentId = docId;
            FileFormatId = fileFormatId;
        }

        public int Id { get; private set; }

        public int DocumentId { get; set; }

        public DateTime EventDateTime { get; set; }

        public int VersionNr { get; set; }

        public int FileFormatId { get; set; }

        [JsonIgnore]
        public byte[] TransactionId { get; private set; }

        [JsonIgnore]
        public string FilePath { get; private set; }

        /// <summary>
        /// The uploaded File
        /// </summary>
        public IFormFile UploadFile { get; set; }

        public DownloadFile DownloadFile { get; set; }
    }
}
