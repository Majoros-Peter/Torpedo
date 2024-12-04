using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class PlaceShipsMessage : BaseMessage
    {
        public PlaceShipsMessage() { }

        public int GameId { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
