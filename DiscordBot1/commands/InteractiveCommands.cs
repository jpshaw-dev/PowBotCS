using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using PowBot;

namespace DiscordBot1.commands
{
    public class InteractiveCommands : BaseCommandModule
    {
        [Command("helloint")]
        public async Task InteractCommand(CommandContext ctx)
        {
            var interactivity = Program.Client.GetInteractivity();

            var messageToRet = await interactivity.WaitForMessageAsync(messsage => messsage.Content == "hello");

            if (messageToRet.Result.Content == "hello")
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} said hello, what a fucking idiot");
            }
        }

        [Command("reacttomes")]
        public async Task ReactToMessage(CommandContext ctx)
        {
            var interactivity = Program.Client.GetInteractivity();

            var messageToReact = await interactivity.WaitForReactionAsync(message => message.Message.Id == 1310292393370718219);

            if(messageToReact.Result.Message.Id == 1310292393370718219)
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} used the emoji {messageToReact.Result.Emoji.Name}");
            }
        }
    }
}
