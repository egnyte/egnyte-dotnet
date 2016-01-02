namespace Egnyte.Api.Files
{
    public class CreateOrUpdateFile
    {
        public CreateOrUpdateFile(string checksum, string groupId, string entryId)
        {
            Checksum = checksum;
            GroupId = groupId;
            EntryId = entryId;
        }

        /// <summary>
        /// SHA512 hash of entire file that can be used for validating upload integrity.
        /// </summary>
        public string Checksum { get; private set; }

        /// <summary>
        /// Current value of the group tag.
        /// </summary>
        public string GroupId { get; private set; }

        /// <summary>
        /// Current value of the entity tag that can be used to compare whether two versions of a resource are the same.
        /// </summary>
        public string EntryId { get; private set; }
    }
}
