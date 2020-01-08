using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreFlora
{
    public class PhysicalAddressConverter : JsonConverter<PhysicalAddress>
    {
        public override PhysicalAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(PhysicalAddress));            
            return PhysicalAddress.Parse(reader.GetString().Replace(":", "").Replace("-", ""));
        }

        public override void Write(Utf8JsonWriter writer, PhysicalAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(BitConverter.ToString(value.GetAddressBytes()).Replace('-', ':'));
        }
    }
}
