using MongoDB.Driver;
using SpRepo.Abstraction;

namespace SpRepo.Implementation;

public class MongoAtlasClient : MongoClient, IMongoAtlasClient
{
    public MongoAtlasClient(MongoClientSettings settings) : base(settings){}
}