

namespace MeduTicTacToe.Domain
{
    public class GameBoard
    {
        public string GameName { get; set; }
        public bool GameRunning { get; set; }
        public bool GameOver { get; set; }
        public string WinnerName { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public RowCollection Rows { get; set; }

        public static GameBoard CreateBoard(string gameName)
        {
            var board = new GameBoard
                            {
                                GameName = gameName,
                                GameRunning = false,
                                GameOver = false,
                                Player1 = null,
                                Player2 = null,
                                Rows = new RowCollection
                                           {
                                               rows = new Cell[][]
                                                          {
                                                              new Cell[] {new Cell(), new Cell(), new Cell(),},
                                                              new Cell[] {new Cell(), new Cell(), new Cell(),},
                                                              new Cell[] {new Cell(), new Cell(), new Cell(),}
                                                          }
                                           },
                                WinnerName = ""
                            };

            return board;
        }
    }

    public class RowCollection
    {
        public Cell[][] rows { get; set; }

        public Cell this[int i, int i1]
        {
            get { return rows[i][i1]; }
        }
    }

    public class Cell
    {
        public string id { get; set; }
        public string letter { get; set; }
    }
}