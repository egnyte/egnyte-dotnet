namespace Egnyte.Api.Files
{
    public class ChunkUploadedMetadata
    {
        internal ChunkUploadedMetadata(string uploadId, int chunkNumber, string checksum)
        {
            UploadId = uploadId;
            ChunkNumber = chunkNumber;
            Checksum = checksum;
        }

        public string UploadId { get; private set; }

        public int ChunkNumber { get; private set; }

        public string Checksum { get; private set; }
    }
}
