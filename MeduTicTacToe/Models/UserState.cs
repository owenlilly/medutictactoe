using MeduTicTacToe.Domain;


namespace MeduTicTacToe.Models
{
    public class UserState
    {
        public bool   gameOver { get; set; }
        public int    winCount { get; set; }
        public string winnerName { get; set; }
        public string playerName { get; set; }
        public string gameName { get; set; }
        public bool   isTurn { get; set; }
        public Cell[][] rows { get; set; }
    }
}