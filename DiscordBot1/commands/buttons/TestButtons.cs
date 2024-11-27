using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBot1.commands.buttons
{
    public class TestButtons : BaseCommandModule
    {
        [Command("button")]
        public async Task Buttons(CommandContext ctx)
        {
            var button1 = new DiscordButtonComponent(ButtonStyle.Primary, "button1", "Button1");
            var button2 = new DiscordButtonComponent(ButtonStyle.Primary, "button2", "Button2");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle("Test Embed"))
                .AddComponents(button1, button2);

            await ctx.Channel.SendMessageAsync(message);
        }


    }
}
