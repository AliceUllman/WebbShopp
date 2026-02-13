using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public static class AdminTools
    {
        public static void EditOrder(MyDbContext db, Order order)
        {
            Console.WriteLine("Redigera Order");
            //Have not implemented order editing yet
        }
        public static void DeleteOrder(MyDbContext db, Order order)
        {
            Console.WriteLine($"Order {order.Id} Har raderats");
            db.Remove(order);
            db.SaveChanges();
        }

        public static void EditDeliveryMethod(MyDbContext db, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine("Nytt Namn:");
            deliveryMethod.Name = Console.ReadLine();
            db.SaveChanges();
        }
        public static void CreateNewDeliveryMethod(MyDbContext db)
        {
            Console.WriteLine("Leveransmetod:");
            string name = Console.ReadLine();
            DeliveryMethod deliveryMethod = new DeliveryMethod { Name = name };
            db.DeliveryMethods.Add(deliveryMethod);
            db.SaveChanges();
        }
        public static void DeleteDeliveryMethod(MyDbContext db, DeliveryMethod deliveryMethod)
        {
            var ordersToUpdate = db.Orders.Where(o => o.DeliveryMethod == deliveryMethod);
            foreach (var order in ordersToUpdate) order.DeliveryMethodId = -1; 
            
            db.Remove(deliveryMethod);//if there is no entry in the database then this will bug out
            db.SaveChanges();
        }

        public static void CreateNewPaymentMethod(MyDbContext db)
        {
            Console.WriteLine("Betalningsmetod:");
            string name = Console.ReadLine();
            PaymentMethod paymentMethod = new PaymentMethod { Name = name };
            db.PaymentMethods.Add(paymentMethod);
            db.SaveChanges();
        }
        public static void EditPaymentMethod(MyDbContext db, PaymentMethod paymentMethod)
        {
            Console.WriteLine("Nytt Namn:");
            paymentMethod.Name = Console.ReadLine();
            db.SaveChanges();
        }
        public static void DeletePaymentMethod(MyDbContext db, PaymentMethod paymentMethod)
        {
            var ordersToUpdate = db.Orders.Where(o => o.PaymentMethod == paymentMethod);
            foreach (var order in ordersToUpdate) order.PaymentMethodId = -1; 
            
            db.Remove(paymentMethod);
            db.SaveChanges();
        }

        public static void CreateNewSupplier(MyDbContext db)
        {
            Console.WriteLine("Leverantör Namn:");
            string name = Console.ReadLine();
            Supplier supplier = new Supplier { Name = name };
            db.Suppliers.Add(supplier);
            db.SaveChanges();
        }
        public static void EditSupplier(MyDbContext db, Supplier supplier )
        {
            Console.WriteLine("Nytt Namn:");
            supplier.Name = Console.ReadLine();
            db.SaveChanges();
        }
        public static void DeleteSupplier(MyDbContext db, Supplier supplier)
        {
            var productsToUpdate = db.Products.Where(p => p.Supplier == supplier);
            foreach (var product in productsToUpdate) product.SupplierId = -1; 

            db.Remove(supplier);
            db.SaveChanges();
        }

        public static void CreateNewCategory(MyDbContext db)
        {
            Console.WriteLine("Kategori Namn:");
            string name = Console.ReadLine();
            Category category = new Category { Name = name };
            db.Categories.Add(category);
            db.SaveChanges();
        }
        public static void EditCategory(MyDbContext db, Category category )
        {
            Console.WriteLine("Nytt Namn:");
            category.Name = Console.ReadLine();
            db.SaveChanges();
        }
        public static void DeleteCategory(MyDbContext db, Category category)
        {
            var productsToUpdate = db.Products.Where(p => p.Category == category);

            foreach (var product in productsToUpdate) product.CategoryId = -1; 
            
            db.Remove(category);
            db.SaveChanges();
        }

        public static void CreateNewProduct(MyDbContext db)
        {
            Product product = new Product();
            EditName(product);
            EditDescription(product);
            EditPrice(product);
            EditStock(product);
            EditFeatured(product);
            EditHidden(product);
            EditSupplier(product, db);
            EditCategory(product, db);
            db.Products.Add(product);

            EditProduct(db, product);
        }
        public static void EditProduct(MyDbContext db, Product product)
        {
            EditSelect.Reset();

            var productProps = new List<string> {
            $"Namn: {product.Name}",
            $"Beskrivning: {product.Description}",
            $"Pris: {product.Price}",
            $"I Lager: {product.InStock}",
            $"Gömd: {product.Hidden}",
            $"Utvald: {product.FeaturedProduct}",
            $"kategori: {product.Category?.Name}",
            $"Leverantör: {product.Supplier?.Name}",
            "Spara och avslut",
            "Avslut"};

            bool runPropSelect = true;

            while (runPropSelect)
            {
                Console.Clear();             
                SelectHelper.SelectString(productProps);
                
                
                if (!EditSelect.Selecting)
                {
                    switch (EditSelect.Row)
                    {
                        case 0:                      
                            EditName(product);
                            productProps[EditSelect.Row] = $"Namn: {product.Name}";
                            break;
                        case 1:
                            EditDescription(product);
                            productProps[EditSelect.Row] = $"Beskrivning: {product.Description}";
                            break;
                        case 2:
                            EditPrice(product);
                            productProps[EditSelect.Row] = $"Pris: {product.Price}";
                            break;
                        case 3:
                            EditStock(product);
                            productProps[EditSelect.Row] = $"I Lager: {product.InStock}";
                            break;
                        case 4://Hidden
                            EditHidden(product);
                            productProps[4] = $"Gömd: {product.Hidden}";//SelectString inside EditHidden changes EditSelect.Row so i need explicit index
                            break;
                        case 5: //Featured
                            EditFeatured(product);
                            productProps[5] = $"Utvald: {product.FeaturedProduct}";
                            break;
                        case 6: //Category
                            EditCategory(product, db);
                            productProps[6] = $"kategori: {product.Category?.Name}";
                            break;
                        case 7: //Supplier
                            EditSupplier(product, db);
                            productProps[7] = $"Leverantör: {product.Supplier?.Name}";
                            break;
                        case 8: //Spara
                            db.SaveChanges();
                            runPropSelect = false;
                            EditSelect.Selecting = true;
                            break;
                        case 9: //Gå tillbaka                 
                            runPropSelect = false;
                            EditSelect.Selecting = true;
                            break;
                    }
                }               
            }
            return;
        }  
        public static void DeleteProduct(MyDbContext db, Product product)
        {           
            db.Remove(product);
            db.SaveChanges();
        }

        static private void EditName(Product product)
        {
            Console.WriteLine("Product Namn:");
            product.Name = Console.ReadLine();
            EditSelect.Selecting = true;
        }
        static private void EditDescription(Product product) 
        {
            Console.WriteLine("Product Beskrivning:");
            product.Description = Console.ReadLine();
            EditSelect.Selecting = true;
        }
        static private void EditPrice(Product product) 
        {
            decimal price;
            Console.WriteLine("Product Pris:");
            while (!decimal.TryParse(Console.ReadLine(), out price))
            //Loop while TryParse returns false, if the parse succeeds then assign the value to price' via the out parameter
            {
                Console.Clear();
                Console.WriteLine("Ogiltigt pris, försök igen:");
                Console.WriteLine("Product Pris:");
            }
            product.Price = price;
            EditSelect.Selecting = true;
        }
        static private void EditStock(Product product) 
        {
            int inStock;
            Console.WriteLine("Hur många exemplar av product finns i föråd?");
            while (!int.TryParse(Console.ReadLine(), out inStock))
            {
                Console.Clear();
                Console.WriteLine("Ogiltig mängd, försök igen:");
                Console.WriteLine("Hur många exemplar av product finns i föråd?");
            }
            product.InStock = inStock;
            EditSelect.Selecting = true;
        }
        static private void EditHidden(Product product) 
        {
            string choiceH = SelectHelper.SelectString(new List<string> { "Ja", "Nej" }, "Vill du Gömma Product?");
            product.Hidden = choiceH == "Ja"; //give the bool the value from the condition result
            EditSelect.Selecting = true;
        }
        static private void EditFeatured(Product product) 
        {
            string choiceF = SelectHelper.SelectString(new List<string> { "Ja", "Nej" }, "Är det en Utvald produkt?");
            product.FeaturedProduct = choiceF == "Ja";
            EditSelect.Selecting = true;
        }
        static private void EditCategory(Product product, MyDbContext db) 
        {
            product.Category = SelectHelper.SelectCategory(db);
            EditSelect.Selecting = true;
        }
        static private void EditSupplier(Product product, MyDbContext db) 
        {
            product.Supplier = SelectHelper.SelectSupplier(db);
            EditSelect.Selecting = true;
        }

    }
}
