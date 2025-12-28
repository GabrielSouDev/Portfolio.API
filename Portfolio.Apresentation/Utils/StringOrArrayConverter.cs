using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfolio.API.Utils;
public class StringOrArrayConverter : JsonConverter<IEnumerable<string>>
{
    public override IEnumerable<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Caso 1: veio uma string
        if (reader.TokenType == JsonTokenType.String)
        {
            var single = reader.GetString();
            // Retorna uma lista com 1 item (se não nulo)
            return single is null ? Array.Empty<string>() : new[] { single.Trim() };
        }

        // Caso 2: veio um array
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray) break;

                // Aceita apenas strings dentro do array
                if (reader.TokenType == JsonTokenType.String)
                {
                    var value = reader.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                        list.Add(value.Trim());
                }
                else
                {
                    // Se tiver tipos diferentes dentro do array, você pode decidir ignorar ou lançar exceção.
                    // Aqui vamos ignorar silenciosamente.
                }
            }
            return list;
        }

        // Qualquer outro tipo (objeto, número, null) → retorna vazio
        // (Se preferir, lance JsonException para falhar cedo)
        return Array.Empty<string>();
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<string> value, JsonSerializerOptions options)
    {
        // Na escrita, sempre padronizamos como array de strings
        writer.WriteStartArray();
        foreach (var s in value ?? Enumerable.Empty<string>())
        {
            writer.WriteStringValue(s);
        }
        writer.WriteEndArray();
    }
}
