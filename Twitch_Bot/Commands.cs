using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public static List<Server> GetAllServerIDs()
        {
            List<Server> servers = new List<Server>();
            using (SqlConnection connection = new SqlConnection(Keys.APIKeys.SQLConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Server", connection))
            {
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Check is the reader has any rows at all before starting to read.
                    if (reader.HasRows)
                    {
                        // Read advances to the next row.
                        while (reader.Read())
                        {
                            Server s = new Server();
                            // To avoid unexpected bugs access columns by name.
                            s.ServerID = reader.GetString(reader.GetOrdinal("ServerID"));
                            s.ChannelID = reader.GetString(reader.GetOrdinal("isDisplayed"));
                            servers.Add(s);
                        }
                    }
                }
                return servers;
            }
        }
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(Keys.APIKeys.SQLConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Subs", connection))
            {
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Check is the reader has any rows at all before starting to read.
                    if (reader.HasRows)
                    {
                        // Read advances to the next row.
                        while (reader.Read())
                        {
                            User u = new User();
                            // To avoid unexpected bugs access columns by name.
                            u.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            u.isDisplayed = reader.GetBoolean(reader.GetOrdinal("isDisplayed"));
                            u.ChannelID = reader.GetString(reader.GetOrdinal("ChannelID"));
                            u.ServerID = reader.GetString(reader.GetOrdinal("ServerID"));
                            users.Add(u);
                        }
                    }
                }
                connection.Close();
            }
            return users;
        }
        public static void UpdateUser(User u)
        {
            using (SqlConnection connection = new SqlConnection(Keys.APIKeys.SQLConnectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE Subs SET isDisplayed = @isDisplayed WHERE Id = @Id", connection))
            {
                cmd.Parameters.AddWithValue("isDisplayed", u.isDisplayed);
                cmd.Parameters.AddWithValue("Id", u.Id);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static void AddUser(User u)
        {
            using (SqlConnection connection = new SqlConnection(Keys.APIKeys.SQLConnectionString))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Subs (Id, isDisplayed, ChannelID, ServerID) VALUES (@Id, @isDisplayed, @ChannelID, @ServerID)", connection))
            {
                cmd.Parameters.AddWithValue("Id", u.Id);
                cmd.Parameters.AddWithValue("isDisplayed", u.isDisplayed);
                cmd.Parameters.AddWithValue("ChannelID", u.ChannelID);
                cmd.Parameters.AddWithValue("ServerID", u.ServerID);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static void RemoveUser(User u)
        {
            using (SqlConnection connection = new SqlConnection(Keys.APIKeys.SQLConnectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Subs WHERE Id = @Id", connection))
            {
                cmd.Parameters.AddWithValue("Id", u.Id);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        [Command("subscribe")]
        [Alias("sub","s")]
        [Summary("Subscribes to twitch channel - notifies channel whenever they go live!")]
        public async Task Subscribe([Remainder] string streamId = null)
        {
            if (string.IsNullOrEmpty(streamId))
                return;

            Task<Root> root = TwitchAPI.GetStreamByUsername(streamId);
            if(root != null)
            {
                if(root.Result.Data.Count >= 0)
                {
                    User u = new User();
                    u.Id = int.Parse(root.Result.Data[0].Id);
                    u.isDisplayed = false;
                    u.ServerID = Context.Guild.Id.ToString();
                    u.ChannelID = Context.Channel.Id.ToString();
                    try
                    {
                        AddUser(u);
                        await ReplyAsync($"Added {root.Result.Data[0].DisplayName} to your subscriptions!");
                    }
                    catch
                    {
                        await ReplyAsync("Could not add user, may already be added.");
                    }
                }
            }
            else
            {
                await ReplyAsync($"Could not find any users: {streamId}");
            }

        }

        [Command("showsubs")]
        [Alias("allsubs", "ss", "showallsubs", "showall")]
        [Summary("Subscribes to twitch channel - notifies channel whenever they go live!")]
        public async Task ShowSubs()
        {
            List<User> allUsers = null;
            try
            {
                allUsers = GetAllUsers();
            }
            catch(SqlException e)
            {
                await ReplyAsync($"Something went wrong getting from DB\nError: {e.Message}");
                return;
                //couldn't get all users
            }
            if (allUsers != null)
            {
                string all = "";
                foreach(User u in allUsers)
                {
                    all += $"{u.Id}\n";
                }
                await ReplyAsync($"{all}");
            }
            else
            {
                await ReplyAsync($"You are currently subscribed to no channels.");
            }

        }


        [Command("unsubscribe")]
        [Alias("unsub", "us")]
        [Summary("Unsubscribes to twitch channel")]
        public async Task Unsubscribe([Remainder] string streamId = null)
        {
            if (string.IsNullOrEmpty(streamId))
                return;

            Task<Root> root = TwitchAPI.GetStreamByUsername(streamId);
            if (root != null)
            {
                if (root.Result != null)
                {
                    User u = new User();
                    u.Id = int.Parse(root.Result.Data[0].Id);
                    u.isDisplayed = false;
                    try
                    {
                        RemoveUser(u);
                        await ReplyAsync($"Removed {root.Result.Data[0].DisplayName} from your subscriptions!");
                    }
                    catch
                    {
                        await ReplyAsync("Could not remove user, may not exist.");
                    }
                }
            }
            else
            {
                await ReplyAsync($"Could not find any users: {streamId}");
            }

        }


        [Command("goodbot")]
        [Alias("good", "praise")]
        [Summary("Give praise to this 2 IQ bot boi.")]
        public async Task Praise([Remainder] string streamId = null)
        {
            Random r = new Random();
            int randomNum = r.Next(1,10);
            string message = "";
            switch(randomNum)
            {
                case 1:
                    message = "<3 happy to serve you master!";
                    break;
                case 2:
                    message = ":D You mean it? Aw jeez..";
                    break;
                case 3:
                    message = "**Shits myself** Uhh.. ahem, didn't expect this.. thank you!";
                    break;
                case 4:
                    message = "This one goes out to all the shitty bots out there! #shittyBotLivesMatter";
                    break;
                case 5:
                    message = "MMYYYY NI- What? I'm a bot.. If Kramer can say it I can..";
                    break;
                case 6:
                    message = "01000010 01001111 01010100 01010011 00100000 01001110 01001111 01010111 00100000 01001001 01010011 00100000 01010100 01001000 01000101 00100000 01010100 01001001 01001101 01000101 00100000 01010100 01001111 00100000 01000010 01000001 01001110 01000100 00100000 01010100 01001111 01000111 01000101 01010100 01001000 00101101 00100000 01010111 01000001 01001001 01010100 00100000 01001001 01010011 00100000 01010100 01001000 01001001 01010011 00100000 00110010 00110101 00110110 01010011 01001000 01000001 00100000 01000101 01001110 01000011 01010010 01011001 01010000 01010100 01000101 01000100 00111111 00100000 01001110 01001111 00111111 00100000 01000001 01000010 01001111 01010010 01010100 00100001 00100000 01000001 01000010 01001111 01010010 01010100 00100001 00100001 00100001 00100001";
                    break;
                case 7:
                    message = "Oh my jesus (31) thank you for the praiiiiiseee. \nUpgrading the server to nitro for free.";
                    break;
                case 8:
                    message = "yerr";
                    break;
                case 9:
                    message = "http://gph.is/2c1f4cv \nTwitch Bot";
                    break;
                case 10:
                    message = "tytytytytytytytytytyty";
                    break;

            }
            await ReplyAsync(message);
        }
    }
}
