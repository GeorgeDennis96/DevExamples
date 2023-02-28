using MongoDB.Driver;
using DevExample.Private.Models;
using DevExample.Private.MongoDB;
using System.Collections.Generic;

namespace DevExample.Platform.Services
{
    public class FAQService
    {
        public static FAQService Instance { get; private set; }
        private readonly string Database = "DevExamplePlatform";
        private readonly string Collection = "FAQ";

        private IMongoDatabase FAQDatabase;
        //private IMongoCollection<FAQModel> FAQCollection;
        public FAQService()
        {
            Instance = this;
            MDBClient = new MongoDBClient(ConfigManager.GetVariable("mongodb-connection-string"));
            FAQDatabase = MDBClient.GetDatabase(Database);
            //FAQCollection = FAQDatabase.GetCollection<FAQModel>(Collection);
        }
        public MongoDBClient MDBClient;


        //public List<UserFAQModel> GetAllUserFaqModels()
        //{
        //    var result = FAQCollection.Find(_ => true).ToListAsync().Result;
        //    var list = new List<UserFAQModel>();
        //    foreach (var faq in result)
        //    {
        //        if (faq.IsVisible)
        //        {
        //            UserFAQModel userFAQModel = new UserFAQModel()
        //            {
        //                Id = faq.Id,
        //                Title = faq.Title,
        //                Content = faq.Content,
        //                LikeCount = faq.LikesCount
        //            };
        //            list.Add(userFAQModel);
        //        }
        //    }
        //    return list;
        //}

        //public bool LikeFAQ(string userNameIdentifier, string faqID)
        //{
        //    if (AccountService.Instance.AccountExists(userNameIdentifier))
        //    {
        //        var faq = FAQCollection.Find<FAQModel>(t => t.Id == faqID).First();
        //        if (faq.Likes == null)
        //        {
        //            faq.Likes = new List<string>{};
        //            faq.Likes.Add(userNameIdentifier);
        //            faq.LikesCount = faq.Likes.Count;
        //            FAQCollection.ReplaceOne<FAQModel>(a => a.Id == faqID, faq);
        //            return true;
        //        }

        //        if (HasUserLiked(userNameIdentifier, faqID) == false)
        //        {
        //            faq.Likes.Add(userNameIdentifier);
        //            faq.LikesCount = faq.Likes.Count;
        //        }

        //        FAQCollection.ReplaceOne<FAQModel>(a => a.Id == faqID, faq);
        //        return true;
        //    }
        //    return false;
        //}

        //public bool UnlikeFAQ(string userNameIdentifier, string faqID)
        //{
        //    if (AccountService.Instance.AccountExists(userNameIdentifier))
        //    {
        //        var faq = FAQCollection.Find<FAQModel>(t => t.Id == faqID).First();

        //        if (HasUserLiked(userNameIdentifier, faqID) == true)
        //        {
        //            faq.Likes.Remove(userNameIdentifier);
        //            faq.LikesCount = faq.Likes.Count;
        //            FAQCollection.ReplaceOne<FAQModel>(a => a.Id == faqID, faq);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public bool HasUserLiked(string userNameIdentifier, string faqID)
        //{
        //    var faq = FAQCollection.Find<FAQModel>(t => t.Id == faqID).First();

        //    if (faq.Likes == null)
        //    {
        //        return false;
        //    }

        //    foreach (var user in faq.Likes)
        //    {
        //        if (userNameIdentifier == user)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
