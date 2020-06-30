using Newtonsoft.Json;
using System;

namespace Egnyte.Api.Search
{
    public class SearchResultItem
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The path to the file in Egnyte.
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        /// <summary>
        /// A plain text snippet of the text containing the matched content.
        /// </summary>
        [JsonProperty(PropertyName = "snippet")]
        public string Snippet { get; set; }

        /// <summary>
        /// A GUID for that particular instance of a file.
        /// </summary>
        [JsonProperty(PropertyName = "entry_id")]
        public string EntryId { get; set; }

        /// <summary>
        /// Timestamp representing the last modified date of the file.
        /// </summary>
        [JsonProperty(PropertyName = "last_modified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// The formatted name of the user who uploaded the file.
        /// </summary>
        [JsonProperty(PropertyName = "uploaded_by")]
        public string UploadedBy { get; set; }

        /// <summary>
        /// The username of the user who uploaded the file.
        /// </summary>
        [JsonProperty(PropertyName = "uploaded_by_username")]
        public string UploadedByUsername { get; set; }

        /// <summary>
        /// The number of versions of the file available.
        /// </summary>
        [JsonProperty(PropertyName = "num_versions")]
        public int NumberOfVersions { get; set; }

        /// <summary>
        /// An HTML formatted snippet of the text containing the matched content.
        /// </summary>
        [JsonProperty(PropertyName = "snippet_html")]
        public string SnippetHtml { get; set; }

        /// <summary>
        /// A boolean value stating if the object is a file or folder.
        /// Please note that, currently, this API only returns file objects.
        /// </summary>
        [JsonProperty(PropertyName = "is_folder")]
        public bool IsFolder { get; set; }
    }
}
