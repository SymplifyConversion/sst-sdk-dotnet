using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SymplifySDK
{
    public class SymplifyCookie
    {
        [JsonPropertyName("_g")]
        public int CookieVersionKey { get; set; }

        [JsonPropertyName("visid")]
        public string WebsiteId { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}