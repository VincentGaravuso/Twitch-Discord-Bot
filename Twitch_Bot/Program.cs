using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Twitch_Bot.Keys;

namespace Twitch_Bot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private SocketUserMessage message;
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string token = APIKeys.DiscordToken;
            

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            
            await _client.StartAsync();

            await Task.Run(() => PollSubscriptions());

            await Task.Delay(-1);

        }
        private void saveChannelId()
        {
            //var t = message.Channel.Id;
        }
        private async Task PollSubscriptions()
        {
            while (true)
            {
                List<User> users = Commands.GetAllUsers();
                if (message != null)
                {
                    //saveChannelIdToDb here
                    if (users.Count() > 0)
                    {
                        foreach (User u in users)
                        {
                            Task<StreamByIdRoot> streamDetailsTask = TwitchAPI.GetStreamById(u.Id.ToString());
                            if (streamDetailsTask.Result != null)
                            {
                                StreamByIdRoot streamDetails = streamDetailsTask.Result;
                                try
                                {
                                    if(streamDetails.streams.Count == 0)
                                    {
                                        if (u.isDisplayed)
                                        {
                                            u.isDisplayed = false;
                                        }
                                    }
                                    else if (streamDetails.streams[0].Type.ToLower() == "live")
                                    {
                                        if (!u.isDisplayed)
                                        {
                                           u.isDisplayed = true;
                                            //Todo add game to display
                                            Task<UserInformationRoot> userInfo = TwitchAPI.GetUserInfoById(u.Id.ToString());
                                            Task<GameInfoRoot> gameInfo = TwitchAPI.GetGameInfoById(streamDetails.streams[0].GameId);

                                            DateTime timeStarted = streamDetails.streams[0].StartedAt;
                                            string timeStartedString = "";
                                            try
                                            {
                                                timeStartedString = timeStarted.ToString("MM/dd/yyyy hh:mm tt");
                                            }
                                            catch
                                            {
                                                timeStartedString = "Error getting time";
                                            }


                                            string thumbnailUrl = streamDetails.streams[0].ThumbnailUrl;
                                            thumbnailUrl = thumbnailUrl.Replace("-{width}x{height}", "");
                                            string gameThumbnailUrl = gameInfo.Result.Data[0].BoxArtUrl;
                                            gameThumbnailUrl = gameThumbnailUrl.Replace("-{width}x{height}", "");
                                            EmbedBuilder emb = new EmbedBuilder();
                                            emb.WithAuthor($"{streamDetails.streams[0].UserName} just went live!", $"{userInfo.Result.Data[0].ProfileImageUrl}", $"https://twitch.tv/{streamDetails.streams[0].UserName}")
                                                .WithColor(Color.Red)
                                                .WithTitle($"{streamDetails.streams[0].Title}").WithUrl($"https://twitch.tv/{streamDetails.streams[0].UserName}")
                                                .AddField("Playing", $"{gameInfo.Result.Data[0].Name}", true)
                                                .AddField("Viewers", $"{streamDetails.streams[0].ViewerCount}", true)
                                                .WithFooter($"Started at: {timeStartedString}")
                                                .ImageUrl = thumbnailUrl;
                                            emb.WithThumbnailUrl(gameThumbnailUrl);
                                            await message.Channel.SendMessageAsync(embed: emb.Build());
                                        }
                                    }
                                    Commands.UpdateUser(u);
                                }
                                catch
                                {
                                    await message.Channel.SendMessageAsync("Something is wrong with the database @vinny.....");
                                }

                            }
                        }
                    }
                }
                Thread.Sleep(10000);
            }
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
