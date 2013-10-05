

namespace MeduTicTacToe.Domain
{
    public class Player
    {
        public Player()
        {
            IsWinner = false;
        }

        public bool IsWinner { get; set; }
        public int  WinCount { get; set; }
        public string WinnerName { get; set; }
        public string PlayerName { get; set; }
        public string PlayerLetter { get; set; }
        public bool IsPlayerTurn { get; set; }

        public static Player CreatePlayer(string playerName, string playerLetter, bool isTurn)
        {
            var player = new Player
                             {
                                 WinCount = 0,
                                 WinnerName = "",
                                 PlayerName = playerName,
                                 PlayerLetter = playerLetter,
                                 IsPlayerTurn = isTurn
                             };

            return player;
        }
    }
}