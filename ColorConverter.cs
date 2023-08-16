using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFML.Graphics;

public class ColorConverter : JsonConverter<Color>
{
    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        var array = (JArray)token;

        if (array.Count != 3)
            throw new JsonSerializationException("Invalid color format. RGB values must be provided as an array of three integers.");

        var r = array[0].Value<byte>();
        var g = array[1].Value<byte>();
        var b = array[2].Value<byte>();

        return new Color(r, g, b);
    }

    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.R);
        writer.WriteValue(value.G);
        writer.WriteValue(value.B);
        writer.WriteEndArray();
    }
}
