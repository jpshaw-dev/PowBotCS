using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;



namespace DiscordBot1.commands.slash
{
    [SlashCommandGroup("arcane", "Arcane based commands")]
    public class ArcanePics : ApplicationCommandModule
    {
        [SlashCommand("random", "Random Arcane Picture (S2 only)")]
        public async Task RandomArcane(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var baseFolderPath = Path.Combine(AppContext.BaseDirectory, "resources", "Season_2"); 

            Console.WriteLine($"Looking for folder at: {baseFolderPath}");

            if (!Directory.Exists(baseFolderPath))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("Base folder not found! Please check the folder path."));
                return;
            }

            
            var subfolders = Directory.GetDirectories(baseFolderPath);

            
            if (subfolders.Length == 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("No subfolders found in the base folder."));
                return;
            }

            
            var random = new Random();
            var randomFolder = subfolders[random.Next(subfolders.Length)];

            
            var files = Directory.GetFiles(randomFolder);

            
            if (files.Length == 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($"No files found in the selected folder: {Path.GetFileName(randomFolder)}"));
                return;
            }

            
            var randomFile = files[random.Next(files.Length)];

            
            using (var fileStream = new FileStream(randomFile, FileMode.Open, FileAccess.Read))
            {
                
                var builder = new DiscordWebhookBuilder()
                    .WithContent($"Here is your random image for the character(s): {Path.GetFileName(randomFolder)}!") 
                    .AddFile(Path.GetFileName(randomFile), fileStream); 

                
                await ctx.EditResponseAsync(builder);
            }


        }

        [SlashCommand("character", "Random Picture of a Specific Character")]
        public async Task RandomImageFromFolder(InteractionContext ctx,
        [ChoiceProvider(typeof(FolderChoiceProvider))][Option("Character", "The character you want to pick a random image for")] string folderName)
        {
            try
            {
                
                await ctx.DeferAsync();

                var baseFolderPath = Path.Combine(AppContext.BaseDirectory, "resources", "Season_2");


                if (!Directory.Exists(baseFolderPath))
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent("Base folder not found! Please check the folder path."));
                    return;
                }

                
                var selectedFolderPath = Path.Combine(baseFolderPath, folderName);

                
                if (!Directory.Exists(selectedFolderPath))
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent($"The folder '{folderName}' does not exist in Season_2."));
                    return;
                }

                
                var files = Directory.GetFiles(selectedFolderPath);

                
                if (files.Length == 0)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent($"No files found in the folder: {folderName}."));
                    return;
                }

                
                var random = new Random();
                var randomFile = files[random.Next(files.Length)];

                
                using (var fileStream = new FileStream(randomFile, FileMode.Open, FileAccess.Read))
                {
                    
                    var builder = new DiscordWebhookBuilder()
                        .WithContent($"Here is your random image for the character(s): {folderName}!") 
                        .AddFile(Path.GetFileName(randomFile), fileStream); 

                    
                    await ctx.EditResponseAsync(builder);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("An error occurred while trying to send a random image."));
            }
        }


    }
}

