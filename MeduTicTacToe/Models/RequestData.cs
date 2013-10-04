using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeduTicTacToe.Models
{
    public class RequestData
    {
        public string gameName { get; set; }
        public string playerName { get; set; }
        public Cell[][] rows { get; set; }
    }
}