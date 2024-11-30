using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot1.database
{
    
    public class DUser
    {
        public string UserName { get; set; }
        public ulong GuildID { get; set; }
        public string AvatarURL { get; set; }
        public double XP { get; set; }
        public int Level { get; set; }
        public int XPLimit { get; set; }
    }
}

