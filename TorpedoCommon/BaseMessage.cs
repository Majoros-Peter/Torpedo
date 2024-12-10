using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TorpedoCommon
{
    public abstract class BaseMessage
    {
        public string Type => GetType().Name;

        public BaseMessage() { }

        public abstract string ToJson();

        public ArraySegment<byte> ToArraySegment()
        {
            var bytes = Encoding.UTF8.GetBytes(this.ToJson());
            return new ArraySegment<byte>(bytes, 0, bytes.Length);
        }

        internal static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}
