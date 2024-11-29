using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class AuthResponse: BaseMessage
    {
        public override string Type => "AuthResponse";

        public AuthResponse() { }

        public bool Success { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
