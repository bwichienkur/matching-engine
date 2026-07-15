using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace EDDY.IS.MatchingEngine.MongoDB
{
    public class MongoContainer<T>
    { 
        [BsonId]
        public string Id { set; get; }

        public T Item { get; set; }
    }

    public class MongoSerialize
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        //zList = MongoSerialize.GetCacheItem<List<VW_Matching_ClientCampusRelationshipInfo>>(MatchingCacheItem.REClientCampusProductMapping.ToString(), ((int)MatchingCacheItem.REClientCampusProductMapping).ToString());
        //MongoSerialize.SaveCacheItem<List<VW_Matching_ClientCampusRelationshipInfo>>(MatchingCacheItem.REClientCampusProductMapping.ToString(), new MongoContainer<List<VW_Matching_ClientCampusRelationshipInfo>>() { Item = zList, Id = ((int)MatchingCacheItem.REClientCampusProductMapping).ToString() });
        public static bool SaveCacheItem<T>(string name, MongoContainer<T> item)
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("me");

            var collection = _database.GetCollection<MongoContainer<T>>(name);

            if (collection == null)
            {
                _database.CreateCollection(name);
                collection.InsertOne(item);
            }
            else
            {
                collection.ReplaceOne(d => d.Id == item.Id, item, new UpdateOptions() { BypassDocumentValidation = true, IsUpsert = true });
            }           

            return true;
        }

        public static T GetCacheItem<T>(string name, string Id)
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("me");

            var collection = _database.GetCollection<MongoContainer<T>>(name);

            if (collection != null)
            {
                var filter = Builders<MongoContainer<T>>.Filter.Eq("_id", Id);
                var result = collection.Find(filter);

                if(result != null && result.Any())
                    return result.First().Item;
            }

            return default(T);
        }

    }
}
