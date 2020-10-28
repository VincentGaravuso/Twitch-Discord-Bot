using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Bot.Keys
{
    public static partial class APIKeys
    {
        static APIKeys()
        {
            DiscordToken = "NzQ3OTcxNTU2MTA1Mzg4MTEz.X0Woyw.6OqFEWim2ygFOX0IKa771jrq1mg";
            TwitchClientId = "u6te6l5iy6poovrbmh4hefxuiv4fey";
            TwitchBearer = "gnsbdmben6yvq10wri2kc77o3nlxcg";
            TwitchBotConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\vinny\source\repos\Twitch\Twitch_Bot\Twitch_Bot\TwitchBot.mdf;Integrated Security=True";
            SQLConnectionString = @"Server=tcp:vgaravsql.database.windows.net,1433;Initial Catalog=Twitch_Bot;Persist Security Info=False;User ID=vinnyg96;Password=Volcom24--;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }
    }
}
