using Azure;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public class ShoppingCart
    {
        public List<OrderItem> ItemsInCart { get; set; } = new List<OrderItem>(); 
        public void AddToCartQuantity(Product product)
        {
            int quantity;
            while (!int.TryParse(Console.ReadLine(), out quantity))
            {
                Console.Clear();
                Console.WriteLine("Ogiltig mängd, försök igen:");
                Console.WriteLine("Hur många producter?");
            }
            var orderItem = new OrderItem
            {
                Product = product,
                Quantity = quantity
            };
            ItemsInCart.Add(orderItem);
        }
        public void AddToCart(Product product)
        {
            var existingItem = ItemsInCart.FirstOrDefault(i => i.Product == product);
            if (existingItem == null)
            {
                var orderItem = new OrderItem
                {
                    Product = product,
                    Quantity = 1
                };
                ItemsInCart.Add(orderItem);
            }
            else
            {
                existingItem.Quantity += 1;
            }
        }
        public void DeleteFromCart(OrderItem orderItem) 
        {
            ItemsInCart.Remove(orderItem);
        }
        public void ChangeAmount(OrderItem orderItem, int amount) 
        {
            orderItem.Quantity = orderItem.Quantity + amount;
        }
        public decimal GetTotalPrice() 
        {
            decimal total = 0;
            foreach (var item in ItemsInCart)
            {
                total += item.Product.Price * item.Quantity;
            }
            return total;
        }
        public void ClearCart() 
        { 
            ItemsInCart.Clear();
        }
        public List<string> BuildCartRows()
        {
            List<string> sl = new List<string>();
            foreach (var item in ItemsInCart)
            {
                sl.Add($"{item.Product.Name} x{item.Quantity} - {item.Product.Price * item.Quantity} kr");
            }
            return sl;
        }
       
        public void EditItem(int ItemIndex, ControlStateMachine sm)
        {
            var item = ItemsInCart[ItemIndex];//this is the item selected, and it is conected to a product
            var cartActions = new List<string> { "Öka/Minska Mängd", "Ta Bort", "Gå Tillbaka" };
            string selectedAction = SelectHelper.SelectString(cartActions, $"du har valt {item.Product.Name}\nAlternativ:\n");
            switch (selectedAction)
            {
                case "Öka/Minska Mängd":
                    int amount;
                    Console.WriteLine("Hur mycket?");
                    int.TryParse(Console.ReadLine(), out amount);
                    ChangeAmount(item, amount);
                    break;
                case "Ta Bort":
                    DeleteFromCart(item);
                    break;
                case "Gå Tillbaka":
                    sm.ControlTransition(ControlStateMachine.ControlTrigger.WindowToPage);
                    break;
            }
        }
        public void CheckOut(MyDbContext db)
        {
            string selectedAction = SelectHelper.SelectString(new List<string> { "Ja", "Nej" }, $"Vill du gå till kassa?\n");
            if (selectedAction == "Ja")
            {
                CheckOut checkout = new CheckOut(this, db);
                checkout.StartCheckout(db);
                
            }
            
        }
    }
}
