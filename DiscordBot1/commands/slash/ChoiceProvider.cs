using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace DiscordBot1.commands.slash
{
    public class FolderChoiceProvider : IChoiceProvider
    {
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
        {
            var baseFolderPath = "C:/Users/jpsha/Downloads/Season_2";

            // Check if the base folder exists
            if (!Directory.Exists(baseFolderPath))
                return Enumerable.Empty<DiscordApplicationCommandOptionChoice>(); // Return an empty list if the directory doesn't exist

            // Get all subfolder names
            var subfolders = Directory.GetDirectories(baseFolderPath)
                                      .Select(Path.GetFileName) // Extract folder names only
                                      .ToList();

            // Create DiscordApplicationCommandOptionChoice for each folder
            var choices = subfolders.Select(folderName =>
                new DiscordApplicationCommandOptionChoice(folderName, folderName)); // Value is also the folder name

            return choices;
        }
    }
}
