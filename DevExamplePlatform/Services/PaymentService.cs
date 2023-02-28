using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;
using System.Linq;

namespace DevExample.Platform.Services
{
    public class PaymentService
    {
        public static PaymentService Instance { get; private set; }
        public PaymentService()
        {
            Instance = this;
        }

        private string StripeAPIKey = ConfigManager.GetVariable("stripe-api-key");

        public List<Product> GetAllProducts()
        {
            StripeConfiguration.ApiKey = StripeAPIKey;
            bool hasMore = true; ;
            var service = new Stripe.ProductService();
            string? lastId = null;

            List<Product> products = new List<Product>();

            while (hasMore)
            {
                var options = new ProductListOptions
                {
                    Limit = 100,
                    StartingAfter = lastId,
                };
                var result = service.List(options);
                foreach (var product in result)
                {
                    products.Add(product);
                }
                hasMore = result.HasMore;
                lastId = result.LastOrDefault()?.Id;
            }
            return products;
        }

        public List<string> GetAllProductIDs(int limit = 5)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var options = new ProductListOptions
            {
                Limit = limit,
            };
            var service = new Stripe.ProductService();
            StripeList<Product> products = service.List(options);

            List<string> ids = new List<string>();

            foreach (var product in products)
            {
                ids.Add(product.Id);
            }
            return ids;
        }

        public List<Product> GetActiveProducts()
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var options = new ProductSearchOptions
            {
                Query = "active:'true'",
            };
            var service = new Stripe.ProductService();

            return service.Search(options).Data;
        }

        public Product GetProduct(string productId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;
            var service = new Stripe.ProductService();
            var product = service.Get(productId);
            return product;
        }

        public Price GetPrice(string priceId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var service = new PriceService();

            try
            {
                var price = service.Get(priceId);
                return price;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<Price> GetAllPrices(int limit = 5)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;
            bool hasMore = true; ;
            var service = new Stripe.PriceService();
            string? lastId = null;

            List<Price> prices = new List<Price>();

            while (hasMore)
            {
                var options = new PriceListOptions
                {
                    Limit = 100,
                    StartingAfter = lastId,
                };
                var result = service.List(options);
                foreach (var price in result)
                {
                    prices.Add(price);
                }
                hasMore = result.HasMore;
                lastId = result.LastOrDefault()?.Id;
            }
            return prices;
        }

        //public List<Subscription> GetUserSubscriptions(string userNameIdentifier)
        //{
        //    if (Services.AccountService.Instance.AccountExists(userNameIdentifier))
        //    {
        //        var 
                  
                  
                  
        //            Account = Services.AccountService.Instance.GetAccount(userNameIdentifier);

        //        StripeConfiguration.ApiKey = StripeAPIKey;

        //        var options = new SubscriptionListOptions
        //        {
        //            Limit = 100,
        //            Customer = DevExampleAccount.stripeCustomerId,
        //            Status = "all", 
        //        };
        //        var service = new Stripe.SubscriptionService();
        //        StripeList<Subscription> subscriptions = service.List(options);

        //        foreach (var sub in subscriptions)
        //        {
        //            if (sub.CustomerId != DevExampleAccount.stripeCustomerId)
        //            {
        //                return null;
        //            }
        //        }
        //        return subscriptions.Data;
        //    }
        //    return null;
        //}

        //public List<string> GetAllPriceIDs(int limit = 5)
        //{
        //    StripeConfiguration.ApiKey = StripeAPIKey;

        //    var options = new PriceListOptions { Limit = limit };
        //    var service = new PriceService();
        //    StripeList<Price> prices = service.List(options);

        //    List<string> ids = new List<string>();
        //    foreach (var priceIDs in prices)
        //    {
        //        ids.Add(priceIDs.Id);
        //    }
        //    return ids;
        //}
        //public string CreateCheckout(string priceID, string userNameIdentifier, string successURL = "http://localhost:5000/payment/successful", string cancelUrl = "http://localhost:5000/payment/failed")
        //{
        //    StripeConfiguration.ApiKey = StripeAPIKey;

        //    var account = AccountService.Instance.GetAccount(userNameIdentifier);
        //    string customerID = null;
        //    if (account.stripeCustomerId == null)
        //    {
        //        var newStripeCustomerID = CreateStripeAccount(userNameIdentifier);
        //        account.stripeCustomerId = newStripeCustomerID;
        //        AccountService.Instance.SetStripeCustomerID(userNameIdentifier, newStripeCustomerID);
        //    }
        //    else
        //    {
        //        customerID = account.stripeCustomerId;
        //    }

        //    var priceService = new PriceService();
        //    var price = priceService.Get(priceID);

        //    if (price != null)
        //    {
        //        if (price.Type == "recurring")
        //        {
        //            var options = new SessionCreateOptions();
        //            if (price.Recurring.UsageType == "metered")
        //            {
        //                options = new SessionCreateOptions
        //                {
        //                    Mode = "subscription",
        //                    SuccessUrl = successURL,
        //                    CancelUrl = cancelUrl,
        //                    LineItems = new List<SessionLineItemOptions>
        //                    {
        //                        new SessionLineItemOptions
        //                        {
        //                            Price = priceID,
        //                        },
        //                    },
        //                    Metadata = new Dictionary<string, string>() { { "usermameidentifier", userNameIdentifier } },
        //                    Customer = customerID,
        //                    SubscriptionData = new SessionSubscriptionDataOptions() { TrialPeriodDays = price.Recurring.TrialPeriodDays }
        //                };
        //            }
        //            else
        //            {
        //                options = new SessionCreateOptions
        //                {
        //                    Mode = "subscription",
        //                    SuccessUrl = successURL,
        //                    CancelUrl = cancelUrl,
        //                    LineItems = new List<SessionLineItemOptions>
        //                    {
        //                        new SessionLineItemOptions
        //                        {
        //                            Price = priceID,
        //                            Quantity = 1,
        //                        },
        //                    },
        //                    Metadata = new Dictionary<string, string>() { { "usermameidentifier", userNameIdentifier } },
        //                    Customer = customerID,
        //                    SubscriptionData = new SessionSubscriptionDataOptions() { TrialPeriodDays = price.Recurring.TrialPeriodDays }
        //                };
        //            }
        //            var service = new SessionService();
        //            var result = service.Create(options);
        //            return result.Url;
        //        }
        //        else if (price.Type == "one_time")
        //        {
        //            var options = new SessionCreateOptions
        //            {
        //                Mode = "payment",
        //                SuccessUrl = successURL,
        //                CancelUrl = cancelUrl,
        //                LineItems = new List<SessionLineItemOptions>
        //                    {
        //                        new SessionLineItemOptions
        //                        {
        //                            Price = priceID,
        //                            Quantity = 1
        //                        },
        //                    },
        //                Metadata = new Dictionary<string, string>() { { "usermameidentifier", userNameIdentifier } },
        //                Customer = customerID,
        //            };
        //            var service = new SessionService();
        //            var result = service.Create(options);
        //            return result.Url;
        //        }
        //    }
        //    return "Failed";
        //}

        public bool CancelSubscription(string subscriptionId, string customerId)
        { 
            StripeConfiguration.ApiKey = StripeAPIKey;

            var subscription = Services.PaymentService.Instance.GetSubscription(subscriptionId);

            if (customerId == subscription.CustomerId)
            {
                var service = new Stripe.SubscriptionService();
                var options = new Stripe.SubscriptionCancelOptions()
                {
                    InvoiceNow = true,
                };
                var sub = service.Cancel(subscriptionId, options);

                if (sub.Status == "canceled")
                {
                    //FinalizeAnInvoice(sub.LatestInvoiceId, customerId);
                    return true;
                }
            }
            return false; ;
        }

        public Stripe.Subscription GetSubscription(string subscriptionId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var service = new Stripe.SubscriptionService();
            var sub = service.Get(subscriptionId);
            return sub;
        }

        public List<Invoice> GetAllInvoices()
        {
            StripeConfiguration.ApiKey = StripeAPIKey;
            bool hasMore = true; ;
            var service = new InvoiceService();
            string? lastId = null;

            List<Invoice> invoices = new List<Invoice>();

            while (hasMore)
            {
                var options = new InvoiceListOptions
                {
                    Limit = 100,
                    StartingAfter = lastId,
                };
                var result = service.List(options);
                foreach (var invoice in result)
                {
                    invoices.Add(invoice);
                }
                hasMore = result.HasMore;
                lastId = result.LastOrDefault()?.Id;
            }
            return invoices;
        }

        public List<Invoice> GetUserInvoices(string customerId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;
            bool hasMore = true; ;
            var service = new InvoiceService();
            string? lastId = null;

            List<Invoice> invoices = new List<Invoice>();

            while (hasMore)
            {
                var options = new InvoiceListOptions
                {
                    Limit = 100,
                    Customer = customerId,
                    StartingAfter = lastId,
                };
                var result = service.List(options);
                foreach (var invoice in result)
                {
                    invoices.Add(invoice);
                }
                hasMore = result.HasMore;
                lastId = result.LastOrDefault()?.Id;
            }
            return invoices;
        }

        public bool FinalizeAnInvoice(string invoiceId, string customerId, bool autoAdvance = true)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var service = new InvoiceService();
            var invoice = service.Get(invoiceId);

            if (invoice.CustomerId == customerId)
            {
                var options = new InvoiceFinalizeOptions()
                {
                    AutoAdvance = autoAdvance
                };
                var finalInvoice = service.FinalizeInvoice(invoiceId, options);
                if (finalInvoice != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CreateUsageRecord(string customerId, string subscriptionItem, int quantity = 0)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var subscriptionItemService = new SubscriptionItemService();
            var subItem = subscriptionItemService.Get(subscriptionItem);
            if (subItem != null)
            {
                var sub = GetSubscription(subItem.Subscription);
                if (sub.CustomerId == customerId)
                {
                    var options = new UsageRecordCreateOptions
                    {
                        Quantity = quantity,
                        Timestamp = DateTime.Now,
                    };
                    var usageRecordService = new UsageRecordService();
                    var record = usageRecordService.Create(subscriptionItem, options);

                    if (record != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public long GetTotalMeteredUsage(string customerId, string subscriptionItemId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var options = new UsageRecordSummaryListOptions
            {
                Limit = 100,
            };
            var service = new UsageRecordSummaryService();
            StripeList<UsageRecordSummary> usageRecordSummaries = service.List(subscriptionItemId, options);
            var total = usageRecordSummaries.Data[0].TotalUsage;
            return total;
        }

        //public string CreateStripeProduct(StripeProductModel product, StripePriceModel price)
        //{
        //    StripeConfiguration.ApiKey = StripeAPIKey;

        //    if (price.Type == "one_time")
        //    {
        //        var options = new ProductCreateOptions()
        //        {
        //            Name = product.Name,
        //            Active = product.Active,
        //            Description = product.Description,
        //            StatementDescriptor = product.Statement_Descriptor,
        //            DefaultPriceData = new ProductDefaultPriceDataOptions()
        //            {
        //                Currency = price.Currency,
        //                UnitAmount = price.Unit_Amount,
        //            },
        //        };

        //        var service = new Stripe.ProductService();
        //        try
        //        {
        //            service.Create(options);
        //            return "Success";
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //            throw;
        //        }
        //    }
        //    else if (price.Type == "recurring")
        //    {
        //        var options = new ProductCreateOptions()
        //        {
        //            Name = product.Name,
        //            Active = product.Active,
        //            Description = product.Description,
        //            StatementDescriptor = product.Statement_Descriptor,
        //            DefaultPriceData = new ProductDefaultPriceDataOptions()
        //            {
        //                Currency = price.Currency,
        //                UnitAmount = price.Unit_Amount,
        //                Recurring = new ProductDefaultPriceDataRecurringOptions() { Interval = price.Interval }
        //            },
        //        };
        //        var service = new Stripe.ProductService();
        //        try
        //        {
        //            service.Create(options);
        //            return "Success";
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //            throw;
        //        }
        //    }
        //    return "Failed";
        //}

        //public string UpdateProduct(StripeUpdateProductModel updateModel)
        //{
        //    StripeConfiguration.ApiKey = StripeAPIKey;

        //    var service = new Stripe.ProductService();
        //    var options = new ProductUpdateOptions();

        //    if (updateModel.Name != String.Empty)
        //    {
        //        options.Name = updateModel.Name;
        //    }
        //    if (updateModel.Description != String.Empty)
        //    {
        //        options.Description = updateModel.Description;
        //    }
        //    if (updateModel.Default_Price_Id != String.Empty)
        //    {
        //        options.DefaultPrice = updateModel.Default_Price_Id;
        //    }

        //    try
        //    {
        //        service.Update(updateModel.Product_Id, options);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //        throw;
        //    }
        //}

        public string DeleteProduct(string productId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var service = new Stripe.ProductService();
            try
            {
                service.Delete(productId);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public string ToggleProduct(string productId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var product = GetProduct(productId);

            var ToggleActive = !product.Active;

            var options = new ProductUpdateOptions
            {
                Active = ToggleActive,
            };
            var service = new Stripe.ProductService();
            try
            {
                service.Update(productId, options);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        // Prices can not be deleted, instead they are archived
        public string TogglePrice(string priceId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var price = GetPrice(priceId);

            bool ToggleActive = !price.Active;

            var options = new PriceUpdateOptions
            {
                Active = ToggleActive,
            };
            var service = new PriceService();
            try
            {
                service.Update(priceId, options);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public string UpdatePriceNickname(string priceId, string newNickname)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;
            var options = new PriceUpdateOptions
            {
                Nickname = newNickname,
            };
            var service = new PriceService();
            try
            {
                service.Update(priceId, options);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        //public string CreateStripePrice(StripePriceModel price)
        //{
        //    StripeConfiguration.ApiKey = StripeAPIKey;

        //    var options = new PriceCreateOptions();

        //    options.Nickname = price.Nickname;
        //    options.Active = price.Active;
        //    options.Product = price.Product_Id;
        //    if (price.Type == "one_time")
        //    {
        //        options.UnitAmount = price.Unit_Amount;
        //        options.Currency = price.Currency;
        //    }
        //    else if (price.Type == "recurring")
        //    {
        //        options.UnitAmount = price.Unit_Amount;
        //        options.Currency = price.Currency;
        //        options.Recurring = new PriceRecurringOptions
        //        {
        //            Interval = price.Interval,
        //        };
        //    }
        //    else if (price.Type == "metered")
        //    {
        //        options.UnitAmount = price.Unit_Amount;
        //        options.Currency = price.Currency;
        //        options.Recurring = new PriceRecurringOptions
        //        {
        //            Interval = price.Interval,
        //            UsageType = "metered",
        //            AggregateUsage = price.Aggregate_Usage,
        //        };
        //    }

        //    var service = new PriceService();

        //    try
        //    {
        //        var result = service.Create(options);

        //        if (price.Is_Default_Price)
        //        {
        //            SetDefaultPrice(result.Id, result.ProductId);
        //        }

        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //        throw;
        //    }
        //}

        public List<Price> GetProductPrices(string productId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var options = new PriceSearchOptions
            {
                Query = "product:'" + productId + "'",
            };
            var service = new PriceService();
            var prices = service.Search(options);
            return prices.Data;
        }

        //public string CreateStripeAccount(string userNameIdentifier)
        //{
        //    var account = Services.AccountService.Instance.GetAccount(userNameIdentifier);

        //    StripeConfiguration.ApiKey = StripeAPIKey;

        //    var options = new CustomerCreateOptions
        //    {
        //        Email = account.email,
        //        Name = account.name
        //    };
        //    var service = new CustomerService();
        //    var result = service.Create(options);

        //    AccountService.Instance.SetStripeCustomerID(userNameIdentifier, result.Id);

        //    return result.Id;
        //}

        public string SetDefaultPrice(string priceId, string productId)
        {
            StripeConfiguration.ApiKey = StripeAPIKey;

            var options = new ProductUpdateOptions
            {
                DefaultPrice = priceId
            };
            var service = new Stripe.ProductService();
            try
            {
                service.Update(productId, options);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public int GetTotalActiveSubscribers()
        {
            int activeSubscribers = 0;

            var options = new SubscriptionListOptions
            {
                Status = "active"
            };
            var service = new Stripe.SubscriptionService();
            activeSubscribers = service.List(options).Count();

            return activeSubscribers;
        }

        private static decimal CalculateSubscriptionMonthlyRevenue(Subscription subscription)
        {
            decimal revenue = 0;
            foreach (var item in subscription.Items)
            {
                var multiplier = item.Plan.Interval switch
                {
                    "day" => 30M,
                    "week" => 4M,
                    "month" => 1M,
                    "year" => 1M / 12M,
                };
                revenue += multiplier * item.Quantity * item.Price.UnitAmountDecimal.GetValueOrDefault();
            }
            return revenue / 100M;
        }

        private static decimal CalculateCustomerMonthlyRevenue(Customer customer)
        {
            var subscriptions = customer.Subscriptions;
            var revenue = 0M;
            foreach (var subscription in subscriptions)
            {
                revenue += CalculateSubscriptionMonthlyRevenue(subscription);
            }
            if (customer.Discount is { Coupon.PercentOff: { } percentOff })
            {
                revenue *= 1 - percentOff / 100M;
            }
            return revenue;
        }

        public static async Task<decimal> CalculateMonthlyRecurringRevenue()
        {
            string? lastId = null;
            var customerClient = new CustomerService();
            decimal revenue = 0M;
            bool hasMore = true;
            while (hasMore)
            {
                var customers = await customerClient.ListAsync(
                    new CustomerListOptions
                    {
                        Limit = 100,
                        Expand = new List<string> { "data.subscriptions" },
                        StartingAfter = lastId
                    });

                revenue += customers.Sum(CalculateCustomerMonthlyRevenue);
                hasMore = customers.HasMore;
                if (hasMore)
                {
                    lastId = customers.LastOrDefault()?.Id;
                    if (lastId is null)
                    {
                        throw new InvalidOperationException("No last id was returned.");
                    }
                }
            }
            return revenue;
        }

        //public async Task<decimal> GetLastMonthsRevenue()
        //{
        //    string? lastId = null;
        //    var invoiceService = new InvoiceService();
        //    decimal revenue = 0M;
        //    bool hasMore = true;

        //    var intervalStart = DateTime.Now.AddDays(-DateTime.Now.Day + 1).AddMonths(-1).AtMidnight();
        //    var lastDayOfMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //    var intervalEnd = DateTime.Now.AddDays(-DateTime.Now.Day + lastDayOfMonth).AddMonths(-1).AtMidnight();

        //    while (hasMore)
        //    {
        //        var invoices = await invoiceService.ListAsync(
        //            new InvoiceListOptions
        //            {
        //                Limit = 100,
        //                Status = "paid",
        //                StartingAfter = lastId,
        //                Created = new DateRangeOptions()
        //                {
        //                    GreaterThanOrEqual = intervalStart,
        //                    LessThanOrEqual = intervalEnd,
        //                },
        //            });

        //        revenue += CalculateRevenueFromInvoices(invoices);
        //        hasMore = invoices.HasMore;
        //    }
        //    return revenue;
        //}

        //public decimal CalculateRevenueFromInvoices(StripeList<Invoice> invoices)
        //{
        //    StripeConfiguration.ApiKey = StripeAPIKey;

        //    var service = new Stripe.SubscriptionService();

        //    decimal revenue = 0;

        //    var intervalStart = DateTime.Now.AddDays(-DateTime.Now.Day + 1).AddMonths(-1).AtMidnight();
        //    var lastDayOfMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //    var intervalEnd = DateTime.Now.AddDays(-DateTime.Now.Day + lastDayOfMonth).AddMonths(-1).AtMidnight();

        //    foreach (var invoice in invoices)
        //    {
        //        if (invoice.SubscriptionId != null)
        //        {
        //            var subscription = service.Get(invoice.SubscriptionId);

        //            if (subscription != null)
        //            {
        //                revenue =+ CalculateSubscriptionMonthlyRevenue(subscription);
        //            }
        //        }
        //    }
        //    return revenue;
        //}
    }
}
