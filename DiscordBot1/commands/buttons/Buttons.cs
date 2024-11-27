using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBot1.commands.buttons
{
    public class Buttons : BaseCommandModule
    {
        [Command("help")]
        public async Task HelpCommand(CommandContext ctx)
        {
            var testsButton = new DiscordButtonComponent(ButtonStyle.Primary, "testsButton", "Tests");

            var calculatorButton = new DiscordButtonComponent(ButtonStyle.Success, "calcButton", "Calculator");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Black)
                .WithTitle("Help Section")
                .WithDescription("Please press a button to view its commands"))
                .AddComponents(testsButton, calculatorButton);

            await ctx.Channel.SendMessageAsync(message);
        }
    }
}
