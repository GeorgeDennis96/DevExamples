using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DevExample.Private.MongoDB
{
    public class MongoDBClient
    {
        private MongoClient client;
        
        public MongoClient GetClient()
        {
            return client;
        }
        public MongoDBClient(string connectionString)
        {
            client = new MongoClient(connectionString);
        }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            var database = client.GetDatabase(databaseName);

            return database;
        }



    }
}
