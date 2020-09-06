using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    public class UserInformation
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("login")]
        public string Login;

        [JsonProperty("display_name")]
        public string DisplayName;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("broadcaster_type")]
        public string BroadcasterType;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl;

        [JsonProperty("offline_image_url")]
        public string OfflineImageUrl;

        [JsonProperty("view_count")]
        public int ViewCount;

        [JsonProperty("email")]
        public string Email = "";
    }

    public class UserInformationRoot
    {
        [JsonProperty("data")]
        public List<UserInformation> Data;
    }


}
