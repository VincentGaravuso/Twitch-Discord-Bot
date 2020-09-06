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
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\TwitchBot.mdf; Integrated Security=True"))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users", connection))
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
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\TwitchBot.mdf; Integrated Security=True"))
            using (SqlCommand cmd = new SqlCommand("UPDATE Users SET isDisplayed = @isDisplayed WHERE Id = @Id", connection))
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
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\TwitchBot.mdf; Integrated Security=True"))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (Id, isDisplayed) VALUES (@Id, @isDisplayed)", connection))
            {
                cmd.Parameters.AddWithValue("Id", u.Id);
                cmd.Parameters.AddWithValue("isDisplayed", u.isDisplayed);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static void RemoveUser(User u)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\TwitchBot.mdf; Integrated Security=True"))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection))
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
            catch
            {
                await ReplyAsync($"Something went wrong getting from DB");
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
                if (root.Result.Data.Count >= 0)
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
    }
}
