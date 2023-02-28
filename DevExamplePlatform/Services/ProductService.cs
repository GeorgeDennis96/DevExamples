using MongoDB.Driver;
using DevExample.Private.Models;
using DevExample.Private.MongoDB;
using System;
using System.Collections.Generic;

namespace DevExample.Platform.Services
{
    public class ProductService
    {
        public static ProductService Instance { get; private set; }
        private readonly string Database = "DevExamplePlatform";
        private readonly string Collection = "Products";

        private IMongoDatabase ProductDatabase;
        private IMongoCollection<ProductModel> ProductCollection;

        public ProductService()
        {
            Instance = this;
            MDBClient = new MongoDBClient(ConfigManager.GetVariable("mongodb-connection-string"));
            ProductDatabase = MDBClient.GetDatabase(Database);
            ProductCollection = ProductDatabase.GetCollection<ProductModel>(Collection);
        }

        public MongoDBClient MDBClient;


        public bool AddProduct(ProductModel model)
        {
            try
            {
                if (!ProductExists(model.Name))
                {
                    ProductCollection.InsertOne(model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public bool UpdateProduct(string Name, ProductModel newModel)
        {
            try
            {
                if (ProductExists(Name))
                {
                    ProductCollection.ReplaceOne<ProductModel>(a=>a.Name == Name,newModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public ProductModel GetProduct(string ProductName)
        {
            try
            {
                var model = ProductCollection.Find<ProductModel>(a => a.Name == ProductName).First();
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public List<ProductModel> GetAllProducts()
        {
            try
            {
                var models = ProductCollection.Find<ProductModel>(_ => true).ToList();
                return models;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public bool DeleteProduct(string ProductName)
        {
            try
            {
                var result = ProductCollection.DeleteOne<ProductModel>(a => a.Name == ProductName);
                if (result.DeletedCount > 0)
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
        public bool EnableProduct(string ProductName)
        {
            try
            {
                var model = GetProduct(ProductName);
                model.Enabled = true;
                var result = ProductCollection.ReplaceOne<ProductModel>(a => a.Name == ProductName, model);
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
        public bool DisableProduct(string ProductName)
        {
            try
            {
                var model = GetProduct(ProductName);
                model.Enabled = false;
                var result = ProductCollection.ReplaceOne<ProductModel>(a => a.Name == ProductName,model);
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

        public bool SetProductRequiredRole(string ProductName,string RequiredPermission)
        {
            try
            {
                var model = GetProduct(ProductName);
                model.RequiredPermission = RequiredPermission;
                var result = ProductCollection.ReplaceOne<ProductModel>(a => a.Name == ProductName, model);
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



        public bool ProductExists(string ProductName)
        {
            var product = GetProduct(ProductName);
            if(product != null)
            {
                return true;
            }
            return false;
        }
    }
}
