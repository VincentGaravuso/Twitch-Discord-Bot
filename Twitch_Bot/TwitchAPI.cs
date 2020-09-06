using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Twitch_Bot.Keys;

namespace Twitch_Bot
{
    public class TwitchAPI
    {
        static string APIString = "https://api.twitch.tv/helix";
        
        static HttpClient client = new HttpClient();

        //TODO: let user choose their stream from list - right now defualts to first in list (somewhat accurate)
        public static async Task<Root> GetStreamByUsername(string username)
        {
            string APICommand = $"/search/channels?query={username}";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-Id", APIKeys.TwitchClientId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKeys.TwitchBearer);
            HttpResponseMessage response = await client.GetAsync(APIString + APICommand);
            Root root = null;
            if (response.IsSuccessStatusCode)
            {
                root = await response.Content.ReadAsAsync<Root>();
            }
            return root;
        }

        public static async Task<StreamByIdRoot> GetStreamById(string streamId)
        {
            string APICommand = $"/streams?user_id={streamId}";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-Id", APIKeys.TwitchClientId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKeys.TwitchBearer);
            HttpResponseMessage response = await client.GetAsync(APIString + APICommand);
            StreamByIdRoot root = null;
            if (response.IsSuccessStatusCode)
            {
                root = await response.Content.ReadAsAsync<StreamByIdRoot>();
            }
            return root;
        }
        public static async Task<UserInformationRoot> GetUserInfoById(string streamId)
        {
            string APICommand = $"/users?id={streamId}";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-Id", APIKeys.TwitchClientId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKeys.TwitchBearer);
            HttpResponseMessage response = await client.GetAsync(APIString + APICommand);
            UserInformationRoot root = null;
            if (response.IsSuccessStatusCode)
            {
                root = await response.Content.ReadAsAsync<UserInformationRoot>();
            }
            return root;
        }

        public static async Task<GameInfoRoot> GetGameInfoById(string gameId)
        {
            string APICommand = $"/games?id={gameId}";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-Id", APIKeys.TwitchClientId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKeys.TwitchBearer);
            HttpResponseMessage response = await client.GetAsync(APIString + APICommand);
            GameInfoRoot root = null;
            if (response.IsSuccessStatusCode)
            {
                root = await response.Content.ReadAsAsync<GameInfoRoot>();
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

        [JsonProperty("game_Id")]
        public string GameId;

        [JsonProperty("Id")]
        public string Id;

        [JsonProperty("is_live")]
        public bool IsLive;

        [JsonProperty("tag_Ids")]
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
