using Lab5.Models;

namespace Lab5.Data
{
    public class DbInitializer
    {   
        public static void Initialize(DealsFinderDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Customers.Any())
            {
                return;
            }
            var customers = new Customer[]
            {
                new Customer{FirstName="Peter", LastName="Huynh", BirthDate=DateTime.Parse("1872-05-02")},
                new Customer{FirstName="Hang", LastName="Pham", BirthDate=DateTime.Parse("2007-06-07")},
                new Customer{FirstName="Yen", LastName="Huynh", BirthDate=DateTime.Parse("2006-05-09")},
                new Customer{FirstName="Annie", LastName="Pham", BirthDate=DateTime.Parse("2005-04-12")}
            };
            foreach (Customer customer in customers)
            {
                context.Customers.Add(customer);               
            }
            context.SaveChanges();

            var foodDeliveryServices = new FoodDeliveryService[]
            {
                new FoodDeliveryService{Id="A1", Title="Matcha", Fee=320},
                new FoodDeliveryService{Id="A2", Title="Cold brew", Fee=350},
                new FoodDeliveryService{Id="B1", Title="Ice cap", Fee=360},
                new FoodDeliveryService{Id="C1", Title="Muffin", Fee=360}
            };
            foreach (FoodDeliveryService foodDeliveryService in foodDeliveryServices)
            {
                context.FoodDeliveryServices.Add(foodDeliveryService);
            }
            context.SaveChanges();

            var subscriptions = new Subscription[]
            {
               new Subscription{CustomerId=1, FoodDeliveryServiceId="A1"},
               new Subscription{CustomerId=1, FoodDeliveryServiceId="B1"},
               new Subscription{CustomerId=1, FoodDeliveryServiceId="C1"},
               new Subscription{CustomerId=2, FoodDeliveryServiceId="A1"},
               new Subscription{CustomerId=3, FoodDeliveryServiceId="A1"},
               new Subscription{CustomerId=4, FoodDeliveryServiceId="A2"}
            };
            foreach(Subscription subscription in subscriptions)
            {
                context.Subscriptions.Add(subscription);
            }
            context.SaveChanges();
        }

    }
}
