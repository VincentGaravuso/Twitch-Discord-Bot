using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    public class GameInfo
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("box_art_url")]
        public string BoxArtUrl;
    }

    public class GameInfoRoot
    {
        [JsonProperty("data")]
        public List<GameInfo> Data;
    }
}
