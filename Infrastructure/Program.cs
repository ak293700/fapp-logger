// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using MongoDB.Bson;

Console.WriteLine("Hello, World!");

string message = """{"FullName": "Alexandre Akdeniz","Roles": ["Admin", "Consolidator"]}""";
string serialized = JsonSerializer.Serialize(message);
Console.WriteLine(serialized);

string? deserialized = JsonSerializer.Deserialize<string>(serialized);


BsonDocument bsonDocument = BsonDocument.Parse(deserialized);

Console.WriteLine(bsonDocument);

foreach (BsonElement bsonElement in bsonDocument)
{
    Console.WriteLine(bsonElement);
}

