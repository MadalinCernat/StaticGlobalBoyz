using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.DataAccessLibrary.Data
{
    public class MongoDbDataAccess
    {
        private readonly IConfiguration _config;
        public IMongoDatabase db;
        public MongoDbDataAccess(DbInfo dbInfo, IConfiguration config)
        {
            _config = config;
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(_config.GetConnectionString(dbInfo.ConnStringName)));
            settings.SslSettings = new SslSettings() 
            { 
                EnabledSslProtocols = SslProtocols.Tls12 
            };
            var client = new MongoClient();
            db = client.GetDatabase(dbInfo.DbName);
        }
        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }
        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }
        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.Find(filter).FirstOrDefault();
        }
        public void UpsertRecord<T>(string table, Guid id, T record)
        {
            BsonBinaryData binData = new BsonBinaryData(id, GuidRepresentation.Standard);
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", binData),
                record,
                new ReplaceOptions { IsUpsert = true });
        }
        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    }
}
