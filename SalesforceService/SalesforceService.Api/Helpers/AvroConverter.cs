using Avro;
using Avro.Generic;
using Avro.IO;

namespace SalesforceService.Api.Helpers;

public static class AvroConverter
{
    // https://ssojet.com/serialize-and-deserialize/serialize-and-deserialize-avro-in-aspnet-mvc/ example used as reference
    public static byte[] SerializeToAvroBytes(Dictionary<string, Object?> data, Avro.Schema schema)
    {
        var recordSchema = schema as RecordSchema 
            ?? throw new InvalidOperationException("Schema is not a RecordSchema");

        var record = new GenericRecord(recordSchema);

        foreach (var field in recordSchema.Fields)
        {
            data.TryGetValue(field.Name, out var value);
            record.Add(field.Name, value);
        }

        foreach (var field in recordSchema.Fields)
        {
            data.TryGetValue(field.Name, out var value);

            Console.WriteLine(
                $"FIELD: {field.Name} | AVRO TYPE: {field.Schema.Tag} | C# TYPE: {(value == null ? "null" : value.GetType().ToString())} | VALUE: {value}"
            );
        }


        using var ms = new MemoryStream();
        var writer = new GenericDatumWriter<GenericRecord>(recordSchema);
        var encoder = new BinaryEncoder(ms);
        writer.Write(record, encoder);

        return ms.ToArray();
    }

    public static GenericRecord DeserializeAvroPayload(byte[] payload, Avro.Schema schema)
    {
        using var stream = new MemoryStream(payload);
        var reader = new GenericDatumReader<GenericRecord>(schema, schema);
        var decoder = new BinaryDecoder(stream);
        return reader.Read(null, decoder);
    }
}