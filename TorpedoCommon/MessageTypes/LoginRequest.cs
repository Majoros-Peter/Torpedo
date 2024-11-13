using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class LoginRequest : BaseMessage
    {
        public override string Type => "LoginRequest";

        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
        
    }
}
