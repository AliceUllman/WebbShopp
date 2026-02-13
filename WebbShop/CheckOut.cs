using Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop;

public class CheckOut
{
    public ShoppingCart ShoppingCart { get; set; }
    public MyDbContext DB { get; set; }
    public CheckOut(ShoppingCart shoppingCart, MyDbContext db)
    {
        ShoppingCart = shoppingCart;
        DB = db;
    }

    public void StartCheckout(MyDbContext db)
    {
        Console.Clear();
        var cartItems = ShoppingCart.ItemsInCart;
        if (!cartItems.Any())
        {
            Console.WriteLine("Din kundvagn är tom.");
            return;
        }

        decimal totalAmount = ShoppingCart.GetTotalPrice(); ;
       
        Order order = FillOrderDetails(totalAmount);

        ConfirmOrderDetails(order);
    }
    private void ConfirmOrderDetails(Order order)
    {
        bool orderConfirmed = false;
        while (!orderConfirmed)
        {
            Console.Clear();

            Console.WriteLine("Bekräfta dina orderdetaljer:");
            Console.WriteLine($"Kund: {order.Customer.FirstName} {order.Customer.LastName}");
            Console.WriteLine($"Totalbelopp: {order.Cost}");
            Console.WriteLine($"Betalningsmetod: {order.PaymentMethod.Name}");//error here
            Console.WriteLine($"Leveransmetod: {order.DeliveryMethod.Name}");
            Console.WriteLine($"Leveransadress: {order.ShippingAddress.Street}, {order.ShippingAddress.City.Name}, {order.ShippingAddress.PostCode}");
            Console.WriteLine("Skriv \"Köp\" för att bekräfta köp, eller \"Avbryt\" för att avbryta");
            string confirm = Console.ReadLine();
            

            if (confirm == "Köp")
            {
                Console.Clear();
                order.Finished = true;
                DB.Orders.Add(order);
                DB.SaveChanges();
                ShoppingCart.ItemsInCart.Clear();
                Console.Clear();

                RenderConfirmation(order);
                orderConfirmed = true;
            }
            else if (confirm == "Avbryt")
            {
                Console.Clear();
                Console.WriteLine("Ordern har avbrutits.");
                orderConfirmed = true;
            }
        }
    }
    private void RenderConfirmation(Order order)
    {
        Window window = new Window(new List<string>
        {
            "",
            $"Din order har lagts och kommer att levereras till {order.ShippingAddress.Street}, {order.ShippingAddress.City.Name}.",
            $"Totalbelopp: {order.Cost}kr",
            "",
            "Tryck på valfri tangent för att fortsätta..."
        },
        $"Tack för ditt köp, {order.Customer.FirstName}!");
        window.Render();
        Console.ReadKey(true);
    }
    private Order FillOrderDetails(decimal totalAmount)
    {
        var cartItems = ShoppingCart.ItemsInCart;

        Order order = new Order
        {
            OrderDate = DateTime.Now,
            Cost = totalAmount,
            Finished = false
        };
        foreach (var item in cartItems)
        {
            order.OrderItems.Add(item);
            item.OrderId = order.Id;
        }

        FillCustomerDetails(order);
        FillAddressDetails(order);
        SelectPaymentMethod(order);
        SelectDeliveryMethod(order);
        
        return order;
    }
    private void FillCustomerDetails(Order order)
    {
        Console.WriteLine("Skriv Förnamn");
        string firstNameInput = Console.ReadLine();
        Console.WriteLine("Skriv Efternamn");
        string lastNameInput = Console.ReadLine();

        Customer customer = new Customer
        {
            FirstName = firstNameInput,
            LastName = lastNameInput,
            Orders = new List<Order> { order }
        };

        order.Customer = customer;
        var customers = DB.Customers.ToList();
        customers.Add(customer);
        
    }
    private void FillAddressDetails(Order order)
    {

        Address address = new Address();
        Console.WriteLine("Skriv Gatunamn:");
        address.Street = Console.ReadLine();
        Console.WriteLine("Skriv Postkod:");
        address.PostCode = int.TryParse(Console.ReadLine(), out int postCode) ? postCode : 0; //sets postCode to 0 if parsing fails, i need a way to adapt for incorrect input
        FillCityDetails(address);

        Console.WriteLine("Skriv Land:");
        address.Country = Console.ReadLine();

        DB.Addresses.Add(address);

        order.ShippingAddress = address;
        order.BillingAddress = address;
    }
   
    private void FillCityDetails(Address address)
    {
        Console.WriteLine("Skriv Stad:");
        var cityInput = Console.ReadLine();//I need a check for incorrect input

        if (DB.Cities.Any(c => c.Name == cityInput))
        {
            address.City = DB.Cities.First(c => c.Name == cityInput);           
        }
        else
        {
            City newCity = new City { Name = cityInput };
            DB.Cities.Add(newCity);

            address.City = newCity;
        }
    }
    private void SelectPaymentMethod(Order order)
    {
        if (!DB.PaymentMethods.Any())
        {
            Console.WriteLine("Inga betalningsmetoder tillgängliga. Kontakta supporten.");
            return;
        }
        List<string> pmStringList = DB.PaymentMethods
            .Where(dm => dm.Name != "Default/Bortagen")
            .Select(p => p.Name)
            .ToList();

        string paymentMethodName = SelectHelper.SelectString(pmStringList, "Välj betalningsmetod:");

        var paymentMethods = DB.PaymentMethods.ToList();

        order.PaymentMethodId = paymentMethods
            .Where(pm => pm.Name == paymentMethodName)
            .Select(pm => pm.Id)
            .FirstOrDefault();
        order.PaymentMethod = paymentMethods
            .Where(pm => pm.Name == paymentMethodName)
            .FirstOrDefault();
    }
    private void SelectDeliveryMethod(Order order)
    {
        if (!DB.DeliveryMethods.Any())
        {
            Console.WriteLine("Inga leveransmetoder tillgängliga. Kontakta supporten.");
            return;
        }

        
        var dmStringList = DB.DeliveryMethods
            .Where(dm => dm.Name != "Default/Bortagen") 
            .Select(dm => dm.Name)
            .ToList();

        string deliveryMethodName = SelectHelper.SelectString(dmStringList, "Välj leveransmetod:");

        var deliveryMethods = DB.DeliveryMethods.ToList();

        order.DeliveryMethodId = deliveryMethods
            .Where(dm => dm.Name == deliveryMethodName)
            .Select(dm => dm.Id)
            .FirstOrDefault();
        order.DeliveryMethod = deliveryMethods
            .Where(dm => dm.Name == deliveryMethodName)
            .FirstOrDefault();
    }
}
