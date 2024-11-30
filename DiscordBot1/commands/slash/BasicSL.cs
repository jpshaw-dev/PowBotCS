using DiscordBot1.database;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot1.commands.slash
{
    public class BasicSL : ApplicationCommandModule
    {

        private readonly DBEngine _dbEngine;

        // Constructor injection of DBEngine


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

        [SlashCommand("modal", "Show a modal")]
        public async Task Modal(InteractionContext ctx)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Test modal")
                .WithCustomId("modal1")
                .AddComponents(new TextInputComponent("Random", "RandomTextBox", "Type something here"));

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.Modal, modal);
        }

        [SlashCommand("profile", "See your profile!")]
        public async Task StoreCommand(InteractionContext ctx)
        {

            Console.WriteLine("profilecommandstart");
            await ctx.DeferAsync();



            var DBEngine = new DBEngine();

            var userDetails = new DUser
            {
                UserName = ctx.Interaction.User.Username,
                GuildID = ctx.Guild.Id,
                AvatarURL = ctx.Interaction.User.AvatarUrl,
                Level = 1,
                XP = 0,
                XPLimit = 100
            };


            var doesExist = await DBEngine.CheckUserExistsAsync(ctx.Interaction.User.Username, ctx.Guild.Id);

            if (doesExist)
            {
                var retreivedUser = await DBEngine.GetUserAsync(ctx.Interaction.User.Username, ctx.Guild.Id);

                if (retreivedUser.Item1)
                {
                    var user = retreivedUser.Item2;
                    var profileEmbed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Purple,
                        Title = $"{user.UserName}",                             

                    };

                    profileEmbed.WithThumbnail(user.AvatarURL);
                    profileEmbed.AddField("Level:", user.Level.ToString());
                    profileEmbed.AddField("XP:", $"{user.XP} / {user.XPLimit}");

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(profileEmbed));

                }
                else
                {
                    Console.WriteLine("Didnt find user");
                    var errorMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "Something went wrong getting your profile"
                    };
                }

            }
            else
            {
                var isStored = await DBEngine.StoreUserAsync(userDetails);

                if (isStored)
                {
                    var sucsessMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Green,
                        Title = "Sucessfully created your profile. Please use the profile command again to see it!"
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(sucsessMessage));
                }
                else
                {
                    var failureMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "Something went wrong, please contact the yapy!"
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(failureMessage));
                }
            }
        }

    }
}
