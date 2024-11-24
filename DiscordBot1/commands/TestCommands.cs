using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot1.commands
{
    public class TestCommands : BaseCommandModule
    {
        [Command("test")]
        public async Task MyFirstCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Hello dummy, {ctx.User.Username }");
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
        public async Task NoU(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("no u XD");
        }
    }
}
