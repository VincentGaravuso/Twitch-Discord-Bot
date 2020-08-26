using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Twitch_Bot.Keys;

namespace Twitch_Bot
{
    public class TwitchAPI
    {
        static string APIString = "https://api.twitch.tv/helix";

        static HttpClient client = new HttpClient();

        public static async Task<Root> GetStreamById(string streamID)
        {
            string APICommand = $"/search/channels?query={streamID}";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-ID", APIKeys.TwitchClientId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKeys.TwitchBearer);
            HttpResponseMessage response = await client.GetAsync(APIString + APICommand);
            Root root = null;
            if (response.IsSuccessStatusCode)
            {
                root = await response.Content.ReadAsAsync<Root>();
            }
            return root;
        }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Datum
    {
        [JsonProperty("broadcaster_language")]
        public string BroadcasterLanguage;

        [JsonProperty("display_name")]
        public string DisplayName;

        [JsonProperty("game_id")]
        public string GameId;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("is_live")]
        public bool IsLive;

        [JsonProperty("tag_ids")]
        public List<string> TagIds;

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("started_at")]
        public object StartedAt;
    }

    public class Pagination
    {
    }

    public class Root
    {
        [JsonProperty("data")]
        public List<Datum> Data;

        [JsonProperty("pagination")]
        public Pagination Pagination;
    }


}
