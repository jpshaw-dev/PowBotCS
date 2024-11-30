using DiscordBot1.database;
using DiscordBot1.other;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot1.commands.prefix
{
    public class TestCommands : BaseCommandModule
    {
        //[Command("store")]
        //public async Task StoreCommand(InteractionContext ctx)
        //{
        //    Console.WriteLine("store command started");

        //    var DBEngine = new DBEngine();

        //    var userInfo = new DUser
        //    {
        //        userName = ctx.User.Username,
        //        serverName = ctx.Guild.Name,
        //        serverID = ctx.Guild.Id
        //    };

        //    var isStored = await DBEngine.StoreUserAsync(userInfo);

        //    if (isStored == true)
        //    {
        //        await ctx.Channel.SendMessageAsync("Sucessfully stored user in database");
        //    }
        //    else
        //    {
        //        await ctx.Channel.SendMessageAsync("Something went wrong");
        //    }
        //}

        [Command("test")]
        public async Task MyFirstCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Hello dummy, {ctx.User.Username}");
        }

        [Command("add")]
        public async Task Add(CommandContext ctx, int n1, int n2)
        {
            int result = n1 + n2;
            await ctx.Channel.SendMessageAsync(result.ToString());
        }

        [Command("sub")]
        public async Task Sub(CommandContext ctx, int n1, int n2)
        {
            int result = n1 - n2;
            await ctx.Channel.SendMessageAsync(result.ToString());
        }

        [Command("nou")]
        [Cooldown(2, 30, CooldownBucketType.User)]
        public async Task NoU(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("no u XD");
        }

        [Command("embed")]
        public async Task Embed(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder
            {
                Title = "test",
                Description = $"{ctx.User.Username} is stinkyyy",
                Color = DiscordColor.Purple
            };


            await ctx.Channel.SendMessageAsync(embed: message);
        }


        [Command("cardgame")]
        public async Task CardGame(CommandContext ctx)
        {
            var userCard = new CardSystem();

            var userCardEmbed = new DiscordEmbedBuilder
            {
                Title = $"Your card is {userCard.SelectedCard}",
                Color = DiscordColor.Purple
            };

            await ctx.Channel.SendMessageAsync(embed: userCardEmbed);

            var botCard = new CardSystem();

            var botCardEmbed = new DiscordEmbedBuilder
            {
                Title = $"The bot drew a {botCard.SelectedCard}",
                Color = DiscordColor.Violet
            };

            await ctx.Channel.SendMessageAsync(embed: botCardEmbed);

            if (userCard.SelectedNumber > botCard.SelectedNumber)
            {
                var winMessage = new DiscordEmbedBuilder
                {
                    Title = "Congratulations! You win! Massive Dubs in the Chat!!!!",
                    Color = DiscordColor.Green
                };
                await ctx.Channel.SendMessageAsync(embed: winMessage);
            }
            else if (userCard.SelectedCard == botCard.SelectedCard)
            {
                var drawMessage = new DiscordEmbedBuilder
                {
                    Title = "DRAW! wowie what are the chances!",
                    Color = DiscordColor.Violet
                };
                await ctx.Channel.SendMessageAsync(embed: drawMessage);
            }
            else
            {
                var loseMessage = new DiscordEmbedBuilder
                {
                    Title = "WOzu right now X3",
                    Color = DiscordColor.Red
                };
                await ctx.Channel.SendMessageAsync(embed: loseMessage);
            }
        }

        
    }
}
