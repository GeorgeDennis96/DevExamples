using DevExample.Private.Models;
using DevExample.Private.MongoDB;
using MongoDB.Driver;
using System;

namespace DevExample.Platform.Services
{
    public class AccountService
    {
        public static AccountService Instance { get; private set; }
        private readonly string Database = "DevExamplePlatform";
        private readonly string Collection = "Accounts";
        public AccountService()
        {
            Instance = this;
            MDBClient = new MongoDBClient(ConfigManager.GetVariable("mongodb-connection-string"));
        }

        public MongoDBClient MDBClient;

        public bool AddAccount(AccountModel account)
        {
            var db = MDBClient.GetDatabase(Database);
            var accountsCollection = db.GetCollection<AccountModel>(Collection);
            try
            {
                account.firstVisit = DateTime.Now.ToString();
                accountsCollection.InsertOne(account);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool ConfirmAcceptTerms(string NameIdentifier)
        {
            var db = MDBClient.GetDatabase(Database);
            var regionsCollection = db.GetCollection<AccountModel>(Collection);
            try
            {
                var account = GetAccount(NameIdentifier);
                var filter = Builders<AccountModel>.Filter.Eq("nameIdentifier", NameIdentifier);
                account.acceptedTerms = true;
                regionsCollection.ReplaceOne<AccountModel>(a => a.nameIdentifier == account.nameIdentifier, account);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public AccountModel GetAccount(string NameIdentifier)
        {
            try
            {
                var db = MDBClient.GetDatabase(Database);
                var accountsCollection = db.GetCollection<AccountModel>(Collection);
                var account = accountsCollection.Find<AccountModel>(a => a.nameIdentifier == NameIdentifier).First();
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public bool AccountExists(string NameIdentifier)
        {
            var account = GetAccount(NameIdentifier);
            if(account != null)
            {
                return true;
            }
            return false;
        }
        public bool IsIronMan(string NameIdentifier)
        {
            var account = GetAccount(NameIdentifier);
            if (account != null)
            {
                if (account.ironMan)
                {
                    return true;
                }
            }
            return false;
        }
        public bool UpdateProfilePicture(string NameIdentifier, FileModel file)
        {
            if (AccountExists(NameIdentifier))
            {
                var account = GetAccount(NameIdentifier);
                var db = MDBClient.GetDatabase(Database);
                var accountsCollection = db.GetCollection<AccountModel>(Collection);

                account.profilePictureSource = file;
                account.isDefaultProfilePicture = false;

                accountsCollection.ReplaceOne<AccountModel>(a => a.nameIdentifier == account.nameIdentifier, account);
                return true;
            }
            return false;
        }
        public bool SetDefaultProfilePicture(string NameIdentifier)
        {
            if (AccountExists(NameIdentifier))
            {
                var db = MDBClient.GetDatabase(Database);
                var accountsCollection = db.GetCollection<AccountModel>(Collection);

                var account = GetAccount(NameIdentifier);
                account.profilePictureSource = null;
                account.isDefaultProfilePicture = true;

                accountsCollection.ReplaceOne<AccountModel>(a => a.nameIdentifier == account.nameIdentifier, account);

                return true;
            }
            return false;
        }
        public bool SetStripeCustomerID(string NameIdentifier, string customerId)
        {
            if (AccountService.Instance.AccountExists(NameIdentifier))
            {
                var db = MDBClient.GetDatabase(Database);
                var accountsCollection = db.GetCollection<AccountModel>(Collection);

                var account = AccountService.Instance.GetAccount(NameIdentifier);
                account.stripeCustomerId = customerId;

                var result = accountsCollection.ReplaceOne<AccountModel>(a => a.nameIdentifier == account.nameIdentifier, account);

                if (result.IsAcknowledged)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
