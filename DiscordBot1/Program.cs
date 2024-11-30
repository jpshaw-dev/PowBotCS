using DiscordBot1.commands.slash;
using DiscordBot1.config;
using DiscordBot1.database;
using DotNetEnv;
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
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        static async Task Main(string[] args)
        {

            Env.Load();
            DotNetEnv.Env.TraversePath().Load();
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            string token = Environment.GetEnvironmentVariable("TOKEN");
            
            
                                                                                                                                                                                                       
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            
            Client = new DiscordClient(discordConfig);

            
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(1)
            });

            
            Client.Ready += Client_Ready;
            Client.VoiceStateUpdated += VoiceChannelHandler;
            Client.ComponentInteractionCreated += InteractionEventHandler;
            Client.ModalSubmitted += ModalEventHandler;
            Client.MessageCreated += MessageCreatedHandler;

            //Commands Configuration
            //var commandsConfig = new CommandsNextConfiguration()
            //{
            //    StringPrefixes = new string[] { configJsonFile.prefix },
            //    EnableMentionPrefix = true,
            //    EnableDms = true,
            //    EnableDefaultHelp = false,
            //};

            //Commands = Client.UseCommandsNext(commandsConfig);

            // Command Registration

            var slashCommandsConfig = Client.UseSlashCommands();
            slashCommandsConfig.RegisterCommands<BasicSL>(788138081474576436);  // Register BasicSL
            slashCommandsConfig.RegisterCommands<CalculatorSL>(788138081474576436);
            slashCommandsConfig.RegisterCommands<ArcanePics>(788138081474576436);

            // Bot Online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }


        private static async Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            Console.WriteLine("Bot is connected and ready.");
        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                try
                {
                    {
                        var DBEngine = new DBEngine();

                        //leveling up

                        var userToCheck = await DBEngine.GetUserAsync(e.Author.Username, e.Guild.Id);

                        if (userToCheck.Item2.XP >= userToCheck.Item2.XPLimit)
                        {
                            await DBEngine.LevelUpAsync(e.Author.Username, e.Guild.Id);
                        }
                        else
                        {
                            var addedXP = await DBEngine.AddXPAsync(e.Author.Username, e.Guild.Id);
                        }

                        if (DBEngine.isLevelledUp)
                        {
                            var user = await DBEngine.GetUserAsync(e.Author.Username, e.Guild.Id);
                            var userNick = (DiscordMember)e.Author;
                            var leveledUpEmbed = new DiscordEmbedBuilder
                            {
                                Color = DiscordColor.Green,
                                Title = $"{userNick.Nickname} has Levelled Up!",
                                Description = $"Level: {user.Item2.Level}"
                            };

                            await e.Channel.SendMessageAsync(embed: leveledUpEmbed);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in MessageCreatedHandler: " + ex.Message);
                }
            }
        }

        private static async Task ModalEventHandler(DiscordClient sender, ModalSubmitEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.ModalSubmit)
            {
                var member = (DiscordMember)e.Interaction.User;
                switch (e.Interaction.Data.CustomId)
                {
                    case "modal1":
                        var values = e.Values;
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{member.Nickname} submitted a modal with the input: {values.Values.First()}"));
                        break;
                }
            }
        }

        private static async Task InteractionEventHandler(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            // Handle Dropdown Events
            switch (args.Id)
            {
                case "dropdown1":
                    var dropOptions = args.Values;
                    foreach (var option in dropOptions)
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

                    // Handle other events such as channel selection, user mentions, etc.
            }

            // Handle Button Events
            switch (args.Interaction.Data.CustomId)
            {
                case "button1":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} has pressed button 1."));
                    break;

                case "button2":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} has pressed button 2."));
                    break;

                    // Other button events can be handled similarly
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
            if (e.Before is null)
            {
                //await e.Channel.
                await e.Channel.SendMessageAsync($"{e.User.Username} joined the voice channel what a fucking loser!");
            }
        }


    }
}
