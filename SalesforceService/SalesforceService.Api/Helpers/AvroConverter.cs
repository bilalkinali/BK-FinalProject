using Avro.Generic;
using Avro.IO;

namespace SalesforceService.Api.Helpers;

public static class AvroConverter
{
    public static byte[] SerializeJsonToAvro(string jsonPayload, Avro.Schema schema)
    {
        return [];
    }

    public static GenericRecord DeserializeAvroPayload(byte[] payload, Avro.Schema schema)
    {
        using var stream = new MemoryStream(payload);
        var reader = new GenericDatumReader<GenericRecord>(schema, schema);
        var decoder = new BinaryDecoder(stream);
        return reader.Read(null, decoder);
    }
}