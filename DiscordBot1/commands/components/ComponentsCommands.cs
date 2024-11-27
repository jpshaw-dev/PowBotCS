using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBot1.commands.components
{
    internal class ComponentsCommands : BaseCommandModule
    {
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

        [Command("dropdown-list")]
        public async Task DropdownList(CommandContext ctx)
        {
            List<DiscordSelectComponentOption> optionsList = new List<DiscordSelectComponentOption>();

            optionsList.Add(new DiscordSelectComponentOption("Option 1", "option1"));
            optionsList.Add(new DiscordSelectComponentOption("Option 2", "option2"));
            optionsList.Add(new DiscordSelectComponentOption("Option 3", "option3"));

            var options = optionsList.AsEnumerable();

            var dropdown = new DiscordSelectComponent("dropdown1", "Select...", options);

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle("This embed has a drop-down list"))
                .AddComponents(dropdown);

            await ctx.Channel.SendMessageAsync(message);
        }

        [Command("channel-list")]
        public async Task ChannelList(CommandContext ctx)
        {
            var channelComp = new DiscordChannelSelectComponent("channelList", "Select ...");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle("Channel List"))
                .AddComponents(channelComp);

            await ctx.Channel.SendMessageAsync(message);
        }

        [Command("mention-list")]
        public async Task MentionList(CommandContext ctx)
        {
            var mentionComp = new DiscordMentionableSelectComponent("mentionList1", "Mention a User");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle("Mention List"))
                .AddComponents(mentionComp);

            await ctx.Channel.SendMessageAsync(message);
        }
    }
}
