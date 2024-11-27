using DiscordBot1.commands;
using DiscordBot1.commands.buttons;
using DiscordBot1.commands.components;
using DiscordBot1.commands.prefix;
using DiscordBot1.commands.slash;
using DiscordBot1.config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace PowBot
{
    internal class Program
    {
        public static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }
        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(discordConfig);

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += Client_Ready;
            Client.VoiceStateUpdated += VoiceChannelHandler;
            Client.ComponentInteractionCreated += ComponentInteraction;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,

            };

            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();

            Commands.CommandErrored += CommandEventHandler;

            slashCommandsConfig.RegisterCommands<BasicSL>(788138081474576436);
            slashCommandsConfig.RegisterCommands<CalculatorSL>(788138081474576436);
            slashCommandsConfig.RegisterCommands<arcanePics>(788138081474576436);
            Commands.RegisterCommands<TestCommands>();
            Commands.RegisterCommands<InteractiveCommands>();
            Commands.RegisterCommands<TestButtons>();
            Commands.RegisterCommands<Buttons>();
            Commands.RegisterCommands<ComponentsCommands>();

            

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task ComponentInteraction(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            // Dropdown Events
            switch (args.Id)
            {
                case "dropdown1":
                    var dropOptions = args.Values;
                    foreach(var option in dropOptions)
                    {
                        var member = (DiscordMember)args.User;
                        switch (option)
                        {
                            case "option1":
                                await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{member.Nickname} has selected option 1"));
                                break;

                            case "option2":
                                await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{member.Nickname} has selected option 2"));
                                break;

                            case "option3":
                                await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{member.Nickname} has selected option 3"));
                                break;
                        }
                    }
                    break;

                case "channelList":
                    var chanOptions = args.Values;
                    foreach(var channel in chanOptions)
                    {
                        var member = (DiscordMember)args.User;
                        var selecChan = await Client.GetChannelAsync(ulong.Parse(channel));
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{member.Nickname} selected the channel {selecChan.Name}"));
                    }
                    break;

                case "mentionList1":
                    var mentionOptions = args.Values;
                    foreach(var user in mentionOptions)
                    {
                        var selectedUser = await Client.GetUserAsync(ulong.Parse(user));
                        

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{selectedUser.Mention} was mentioned"));
                    }
                    break;
            }




            // Button Events
            switch (args.Interaction.Data.CustomId)
            {
                case "button1":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} has pressed button 1. What an idiot"));
                    break;

                case "button2":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} has pressed button 2. What a sexy sex sexer sex :3"));
                    break;

                case "testsButton":
                    await args.Interaction.DeferAsync();
                    var testsCommandEmbed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Black,
                        Title = "Test commands for that idiot yapy",
                        Description = "!test -> Send a basic test message \n" +
                                       "!embed -> Calls you a nice name :) \n" +
                                       "!cardgame -> Play a simple cardgame.                            Highest number wins!"
                    };

                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(testsCommandEmbed));
                    break;

                case "calcButton":
                    await args.Interaction.DeferAsync();
                    var calcCommandEmbed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Black,
                        Title = "Some stupid lil calulator idk its just for learning",
                        Description = "/calculator add -> adds two numbers \n" +
                                    "/calculator subtract -> subtracts two numbers \n" +
                                    "/calculator multiply -> you get it honestly idk why im actually typing descriptions for all these."
                    };

                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(calcCommandEmbed));
                    break;
            }
        }

        private static async Task CommandEventHandler(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception is ChecksFailedException exception)
            {
                string timeLeft = string.Empty;
                foreach (var check in exception.FailedChecks)
                {
                    var coolDown = (CooldownAttribute)check;
                    timeLeft = coolDown.GetRemainingCooldown(e.Context).ToString(@"hh\:mm\:ss");
                }

                var coolDownMessage = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = "Please wait for the cooldown to end.",
                    Description = $"Time Left: {timeLeft}"
                };

                await e.Context.Channel.SendMessageAsync(embed: coolDownMessage);
            }
        }

        private static async Task VoiceChannelHandler(DiscordClient sender, DSharpPlus.EventArgs.VoiceStateUpdateEventArgs e)
        {
            if(e.Before is null)
            {
                await e.Channel.SendMessageAsync($"{e.User.Username} is a lil loser in the vc!!! laugh at them!!");
            }
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}