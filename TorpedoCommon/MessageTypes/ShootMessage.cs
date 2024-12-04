using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class ShootMessage : BaseMessage
    {
        public ShootMessage() { }

        public int GameId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
