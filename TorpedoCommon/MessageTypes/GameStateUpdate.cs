using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class GameStateUpdate : BaseMessage
    {
        public override string Type => "GameStateUpdate";

        public GameStateUpdate() { }

        public Game GameState { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
