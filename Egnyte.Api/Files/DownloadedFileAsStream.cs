namespace Egnyte.Api.Files
{
    using System;
    using System.IO;
    public class DownloadedFileAsStream
    {
        internal DownloadedFileAsStream(
            Stream data,
            string checksum,
            DateTime lastModified,
            string eTag,
            string contentType,
            long contentLength,
            long fullFileLength)
        {
            Data = data;
            Checksum = checksum;
            LastModified = lastModified;
            ETag = eTag;
            ContentType = contentType;
            ContentLength = contentLength;
            FullFileLength = fullFileLength;
        }

        public Stream Data { get; private set; }

        public string Checksum { get; private set; }

        public DateTime LastModified { get; private set; }

        public string ETag { get; private set; }

        public string ContentType { get; private set; }

        public long ContentLength { get; private set; }

        public long FullFileLength { get; private set; }
    }
}
