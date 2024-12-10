using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class GameOver : BaseMessage
    {
        public GameOver() { }

        public string Winner { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
