namespace Egnyte.Api.Files
{
    using System;

    public class DownloadedFile
    {
        public byte[] Data { get; set; }

        public string Checksum { get; set; }

        public DateTime LastModified { get; set; }

        public string ETag { get; set; }

        public string ContentType { get; set; }

        public int ContentLength { get; set; }
    }
}
