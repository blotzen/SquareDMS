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

        public DocumentVersion(int docId, int fileFormatId, byte[] fileStreamData)
        {
            DocumentId = docId;
            FileFormatId = fileFormatId;
            FilestreamData = fileStreamData;
        }

        public int Id { get; private set; }

        public int DocumentId { get; set; }

        public DateTime EventDateTime { get; set; }

        public int VersionNr { get; set; }

        public int FileFormatId { get; set; }

        public byte[] FilestreamData { get; set; }

        [JsonIgnore]
        public byte[] TransactionId { get; private set; }

        [JsonIgnore]
        public string FilePath { get; private set; }
    }
}
