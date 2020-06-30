using System.Collections.Generic;

namespace Egnyte.Api.Common
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
