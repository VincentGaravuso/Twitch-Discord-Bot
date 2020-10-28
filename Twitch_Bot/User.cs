using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    [Table(Name= "Users")]
    public class User
    {
        [Column(IsPrimaryKey =true)]
        public int Id { get; set; }

        [Column]
        public bool isDisplayed { get; set; }
        [Column]
        public string ChannelID { get; set; }
        [Column]
        public string ServerID { get; set; }
    }
}
