using System;
using System.IO;

namespace SquareDMS.DataLibrary
{
    /// <summary>
    /// Rrepresents the file that is being downloaded
    /// </summary>
    public class DownloadFile
    {
        private const string FALLBACK_MEDIATYPE = "application/octet-stream";

        private readonly MediaTypeHeaderValue _mediaType;

        /// <param name="downloadFileStream"></param>
        /// <param name="mediaType">Media type for the data in the stream</param>
        public DownloadFile(Stream downloadFileStream)
        {
            DownloadFileStream = downloadFileStream;
        }

        /// <summary>
        /// Generates the media type from the string and
        /// sets it to the property. In case of an conversion
        /// error the default value will be use <see cref="FALLBACK_MEDIATYPE"/>
        /// </summary>
        public void GenerateMediaType(string contentType)
        {
            try
            {
                MediaType = new MediaTypeHeaderValue(contentType);
            }
            catch (Exception ex)
            {
                // TODO: log media type conversion fail
                MediaType = new MediaTypeHeaderValue(FALLBACK_MEDIATYPE);
            }
        }

        public Stream DownloadFileStream { get; }

        public MediaTypeHeaderValue MediaType { get; private set; }
    }
}
