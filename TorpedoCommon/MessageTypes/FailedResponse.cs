using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoCommon.MessageTypes
{
    public class FailedResponse : BaseMessage
    {
        public FailedResponse() { }

        public string Message { get; set; }

        public override string ToJson()
        {
            return Serialize(this);
        }
    }
}
