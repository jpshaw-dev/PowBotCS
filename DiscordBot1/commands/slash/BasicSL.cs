using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot1.commands.slash
{
    public class BasicSL : ApplicationCommandModule
    {
        [SlashCommand("test", "This is a test slash command")]
        public async Task FirstSlashCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();

             

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Test test numba 2 worked! :D"));
        }

        [SlashCommand("embedder", "This will put text in a embed")]
        public async Task SlashCommandParams(InteractionContext ctx, [Option("Text", "Type in anything")] string testParam, [Option("User", "Enter User")] DiscordUser user)
        {
            await ctx.DeferAsync();

            var member = (DiscordMember)user;

            var embedMsg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Purple,
                Title = "Test Embed",
                Description = $"{ testParam } {member.Nickname}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMsg));
        }

        
        [SlashCommand("discordParams", "This slash commands allows for the passing of discord parameters")]
        public async Task DiscordParams(InteractionContext ctx, [Option("user", "Pass in a user")] DiscordUser user, [Option("file", "Upload a file here")] DiscordAttachment file)
        {
            await ctx.DeferAsync();

            var member = (DiscordMember)user;

            var embed = new DiscordEmbedBuilder
            {
                Title = "test embed",
                Color = DiscordColor.Blurple,
                Description = $"{member.Nickname} {file.FileName} {file.FileSize}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
