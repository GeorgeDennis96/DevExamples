using MongoDB.Driver;
using DevExample.Private.Models;
using DevExample.Private.MongoDB;
using System;
using System.Collections.Generic;

namespace DevExample.Platform.Services
{
    public class SubscriptionService
    {
        public static SubscriptionService Instance { get; private set; }
        private readonly string Database = "DevExamplePlatform";
        private readonly string Collection = "Subscriptions";

        private IMongoDatabase SubscriptionDatabase;
        private IMongoCollection<SubscriptionModel> SubscriptionCollection;

        public SubscriptionService()
        {
            Instance = this;
            MDBClient = new MongoDBClient(ConfigManager.GetVariable("mongodb-connection-string"));
            SubscriptionDatabase = MDBClient.GetDatabase(Database);
            SubscriptionCollection = SubscriptionDatabase.GetCollection<SubscriptionModel>(Collection);
        }

        public MongoDBClient MDBClient;

        public bool AddSubscription(string userNameIdentifier, string productName)
        {
            try
            {
                var product = ProductService.Instance.GetProduct(productName);
                if (product != null) {
                    if (!SubscriptionExists(userNameIdentifier,productName))
                    {
                        var subscription = new SubscriptionModel();
                        subscription.startDate = DateTime.Now;
                        subscription.userNameIdentifier = userNameIdentifier;
                        subscription.productName = productName;
                        subscription.SubscriptionType = product.SubscriptionType;

                        SubscriptionCollection.InsertOne(subscription);
                        return true;
                    }
                    else
                    {
                        ReSubscription(userNameIdentifier, productName);
                        return true;
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool UpdateSubscription(string userNameIdentifier, SubscriptionModel model)
        {
            try
            {
                if (SubscriptionExists(userNameIdentifier,model.productName))
                {
                    SubscriptionCollection.ReplaceOne<SubscriptionModel>(a => a.userNameIdentifier == userNameIdentifier && a.productName == model.productName, model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public SubscriptionModel GetSubscription(string userNameIdentifier, string productName)
        {
            try
            {
                var model = SubscriptionCollection.Find<SubscriptionModel>(a => a.userNameIdentifier == userNameIdentifier && a.productName == productName).First();
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public List<SubscriptionModel> GetAllSubscriptions(string userNameIdentifier)
        {
            try
            {
                var models = SubscriptionCollection.Find<SubscriptionModel>(x => x.userNameIdentifier == userNameIdentifier).ToList();
                return models;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }


        public bool Unsubscribe(string userNameIdentifier, string productName)
        {
            try
            {
                var model = GetSubscription(userNameIdentifier, productName);
                model.cancelled = true;
                var result = SubscriptionCollection.ReplaceOne<SubscriptionModel>(a => a.userNameIdentifier == userNameIdentifier && a.productName == productName, model);
                if (result.ModifiedCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public bool ReSubscription(string userNameIdentifier, string productName)
        {
            try
            {
                var model = GetSubscription(userNameIdentifier, productName);
                model.cancelled = false;
                var result = SubscriptionCollection.ReplaceOne<SubscriptionModel>(a => a.userNameIdentifier == userNameIdentifier && a.productName == productName, model);
                if (result.ModifiedCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public bool EnableSubscription(string userNameIdentifier, string productName)
        {
            try
            {
                var model = GetSubscription(userNameIdentifier, productName);
                model.paused = true;
                var result = SubscriptionCollection.ReplaceOne<SubscriptionModel>(a => a.userNameIdentifier == userNameIdentifier && a.productName == productName, model);
                if (result.ModifiedCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }
        public bool DisableSubscription(string userNameIdentifier, string productName)
        {
            try
            {
                var model = GetSubscription(userNameIdentifier,productName);
                model.paused = false;
                var result = SubscriptionCollection.ReplaceOne<SubscriptionModel>(a => a.userNameIdentifier == userNameIdentifier && a.productName == productName, model);
                if (result.ModifiedCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }


        public bool SubscriptionExists(string userNameIdentifier, string productName)
        {
            var Subscription = GetSubscription(userNameIdentifier,productName);
            if (Subscription != null)
            {
                return true;
            }
            return false;
        }
    }
}
