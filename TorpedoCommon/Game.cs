using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon
{
    public class Game
    {
        public int Id { get; set; }

        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public bool SetupPhase { get; set; } = true;
        public bool isPlayer1Next { get; set; } = true;
    }
}
