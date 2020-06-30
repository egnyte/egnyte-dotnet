
namespace Egnyte.Api
{
    using System;
    using System.Collections.Generic;

    public class TokenRequestParameters
    {
        public Uri BaseAddress { get; set; }

        public Dictionary<string, string> QueryParameters { get; set; } 
    }
}
