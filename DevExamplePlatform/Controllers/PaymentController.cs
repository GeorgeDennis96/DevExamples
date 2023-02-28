using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DevExample.Platform.Controllers
{
    [Route("api/stripe")]
    public class PaymentController : Controller
    {
        // You can find your endpoint's secret in your webhook settings
        const string secret = "";

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret);
                Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
                Session session = null;
                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        session = stripeEvent.Data.Object as Session;

                        // Save an order in your database, marked as 'awaiting payment'
                        CreateOrder(session);

                        // Check if the order is paid (for example, from a card payment)
                        //
                        // A delayed notification payment will have an `unpaid` status, as
                        // you're still waiting for funds to be transferred from the customer's
                        // account.
                        var orderPaid = session.PaymentStatus == "paid";



                        if (orderPaid)
                        {
                            // Fulfill the purchase
                            FulfillOrder(session);

                            // Send an email to the customer confirming the payment
                            EmailCustomerAboutSuccessfulPayment(session);
                        }

                        break;
                    case Events.CheckoutSessionAsyncPaymentSucceeded:
                        session = stripeEvent.Data.Object as Session;

                        // Fulfill the purchase
                        FulfillOrder(session);

                        break;
                    case Events.CheckoutSessionAsyncPaymentFailed:
                        session = stripeEvent.Data.Object as Session;

                        // Send an email to the customer asking them to retry their order
                        EmailCustomerAboutFailedPayment(session);

                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Something failed {ex}");
                return BadRequest();
            }
        }


        private void FulfillOrder(Session session)
        {
            var userNameIdentifier = session.Metadata["usermameidentifier"];


            if (session.Mode == "subscription")
            {
                Console.WriteLine(session.CustomerId+ " - has subscribed to price - "+session.Subscription.Id);

                // Handle fulfillment

            }
            if (session.Mode == "payment")
            {
                Console.WriteLine(session.CustomerId+ " - has payed for price - " + session.LineItems.Data[0].Price.Id.ToString());

                // Handle fulfillment

            }
        }

        private void CreateOrder(Session session)
        {


            Console.WriteLine("CREATE ORDER!");
        }

        private void EmailCustomerAboutFailedPayment(Session session)
        {

        }

        private void EmailCustomerAboutSuccessfulPayment(Session session)
        {

        }
    }
}
