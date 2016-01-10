namespace Egnyte.Api.Files
{
    using System;

    public class DownloadedFile
    {
        public DownloadedFile(
            byte[] data,
            string checksum,
            DateTime lastModified,
            string eTag,
            string contentType,
            int contentLength,
            int fullFileLength)
        {
            Data = data;
            Checksum = checksum;
            LastModified = lastModified;
            ETag = eTag;
            ContentType = contentType;
            ContentLength = contentLength;
            FullFileLength = fullFileLength;
        }

        public byte[] Data { get; private set; }

        public string Checksum { get; private set; }

        public DateTime LastModified { get; private set; }

        public string ETag { get; private set; }

        public string ContentType { get; private set; }

        public int ContentLength { get; private set; }

        public int FullFileLength { get; private set; }
    }
}
