using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        List<string> Subscriptions = new List<string>();
        CommandService _commands;
        public Commands(CommandService _commands)
        {
            this._commands = _commands;
            initTesting();
        }

        private void initTesting()
        {
            Subscriptions.Add("nutt_milk");
        }

        private async Task PollSubscriptions()
        {
            while (true)
            {
                if (Subscriptions.Count > 0)
                {
                    foreach (string twitchId in Subscriptions)
                    {
                        Task<Root> root = TwitchAPI.GetStreamById(twitchId);
                        if (root != null)
                        {
                            if (root.Result.Data.Count >= 0 && root.Result.Data[0].IsLive)
                            {
                                await ReplyAsync($"{root.Result.Data[0].DisplayName} Is live!");
                            }
                        }
                    }
                }
                Thread.Sleep(10000);
            }
        }
        [Command("start")]
        [Alias("sta", "st")]
        [Summary("null")]
        public async Task StartBot([Remainder] string streamID = null)
        {
            await PollSubscriptions();
        }

            [Command("subscribe")]
        [Alias("sub","s")]
        [Summary("Subscribes to twitch channel - notifies channel whenever they go live!")]
        public async Task Subscribe([Remainder] string streamID = null)
        {
            if (string.IsNullOrEmpty(streamID))
                return;

            Task<Root> root = TwitchAPI.GetStreamById(streamID);
            if(root != null)
            {
                if(root.Result.Data.Count >= 0)
                {
                    string test = root.Result.Data[0].DisplayName;
                    Subscriptions.Add(root.Result.Data[0].Id);
                    await ReplyAsync($"Added {root.Result.Data[0].DisplayName} to your subscriptions!");
                }
            }
            else
            {
                await ReplyAsync($"Could not find any users: {streamID}");
            }

        }
    }
}
