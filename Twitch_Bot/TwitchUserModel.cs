using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Stream
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("user_id")]
        public string UserId;

        [JsonProperty("user_name")]
        public string UserName;

        [JsonProperty("game_id")]
        public string GameId;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("viewer_count")]
        public int ViewerCount;

        [JsonProperty("started_at")]
        public DateTime StartedAt;

        [JsonProperty("language")]
        public string Language;

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl;

        [JsonProperty("tag_ids")]
        public List<string> TagIds;
    }

    public class PaginationId
    {
    }

    public class StreamByIdRoot
    {
        [JsonProperty("data")]
        public List<Stream> streams;

        [JsonProperty("pagination")]
        public PaginationId Pagination;
    }


    public static class TwitchUserModel
    {
        /* Dict<TwitchId, isDisplayed> */
        public static Dictionary<string, bool> Subscriptions = new Dictionary<string, bool>();
    }
}
