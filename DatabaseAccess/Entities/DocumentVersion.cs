using System;

namespace SquareDMS.DatabaseAccess.Entities
{
    public class DocumentVersion : IEntity
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

        public int DocumentId { get; private set; }

        public DateTime EventDateTime { get; private set; }

        public int VersionNr { get; private set; }

        public int FileFormatId { get; private set; }

        public byte[] FilestreamData { get; set; }

        public byte[] TransactionId { get; private set; }

        public string FilePath { get; private set; }
    }
}
