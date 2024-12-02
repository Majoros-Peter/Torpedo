using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class PlayerListResponse : BaseMessage
    {
        public override string Type => "PlayerListResponse";

        public PlayerListResponse() { }

        public List<string> players { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
