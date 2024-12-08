using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon
{
    public class Game
    {
        const int BOARD_SIZE = 10;

        public int Id { get; set; }

        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public bool SetupPhase { get; set; } = true;
        public bool isPlayer1Next { get; set; } = true;

        public List<Tuple<int, int>> Player1Ships { get; set; }
        public List<Tuple<int, int>> Player2Ships { get; set; }

        public bool[] Player1Shots { get ; set; } = new bool[BOARD_SIZE * BOARD_SIZE];
        public bool[] Player2Shots { get; set; } = new bool[BOARD_SIZE * BOARD_SIZE];
    }
}
