using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGamblingBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Message by " + Context.User.Username);
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];
            var embed = new EmbedBuilder();
            embed.WithTitle("Choice for " + Context.User.Username);
            embed.WithDescription(selection);
            embed.WithColor(new Color(255, 255, 0));
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            DataStorage.AddPairToStorage(Context.User.Username + DateTime.Now.ToLongTimeString(), selection);
        }

        [Command("secret")]
        public async Task RevealSecret([Remainder]string arg = "")
        {
            await Context.User.SendMessageAsync(Utilities.GetAlert("SECRET"));
        }
        
        [Command("purge")]
        [Alias("clear")]
        [Summary("Downloads and removes X messages from the current channel.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task PurgeAsync(int amount)
        {
            // Check if the amount provided by the user is positive.
            if (amount <= 0)
            {
                await ReplyAsync("The amount of messages to remove must be positive.");
                return;
            }

            // Download X messages starting from Context.Message, which means
            // that it won't delete the message used to invoke this command.
            var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount).FlattenAsync();

            // Note:
            // FlattenAsync() might show up as a compiler error, because it's
            // named differently on stable and nightly versions of Discord.Net.
            // - Discord.Net 1.x: Flatten()
            // - Discord.Net 2.x: FlattenAsync()

            // Ensure that the messages aren't older than 14 days,
            // because trying to bulk delete messages older than that
            // will result in a bad request.
            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            // Get the total amount of messages.
            var count = filteredMessages.Count();

            // Check if there are any messages to delete.
            if (count == 0)
                await ReplyAsync("Nothing to delete.");

            else
            {
                // The cast here isn't needed if you're using Discord.Net 1.x,
                // but I'd recommend leaving it as it's what's required on 2.x, so
                // if you decide to update you won't have to change this line.
                await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
                await ReplyAsync($"Done. Removed {count} {(count > 1 ? "messages" : "message")}.");
            }
        }
        [Command("data")]
        public async Task GetData()
        {
            await Context.Channel.SendMessageAsync("Data has " + DataStorage.GetPairsCount() + " pairs.");
        }
        [Command("answer")]
        [Alias("ans")]
        public async Task CheckAns(int answer)
        {
            int solution = RepeatingTimer.MathProblemSolver();
            Console.WriteLine(solution);
            Console.WriteLine(answer);
            if (solution == answer)
            {
                await Context.Channel.SendMessageAsync(Context.User.Username + "got the right answer!!");
            }
            else
            {
                await Context.Channel.SendMessageAsync(Context.User.Username + "wrong answer");
            }
        }
    }
}
