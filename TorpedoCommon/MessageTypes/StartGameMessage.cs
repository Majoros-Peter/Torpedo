using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class StartGameMessage : BaseMessage
    {
        public StartGameMessage() { }

        public string Player2Name { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
