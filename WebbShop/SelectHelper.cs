using Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public class SelectHelper
    {
        static Product SelectProduct(MyDbContext db)
        {
            EditSelect.Reset();
            var products = db.Products
                 .Include(p => p.Category)
                 .Include(p => p.Supplier)
                 .ToList();

            while (EditSelect.Selecting)
            {
                Console.Clear();
                PrintAllAndFeatureSelect(products, EditSelect.Row, product => product.Name);
                EditSelect.HandleSelectionInput(products.Count, 0);
            }
            var selectedProduct = products[EditSelect.Row];
            
            return selectedProduct;
        }
        public static PaymentMethod SelectPaymentMethod(MyDbContext db)
        {
            EditSelect.Reset();
            var paymentMethods = db.PaymentMethods.ToList();

            while (EditSelect.Selecting)
            {
                Console.Clear();
                PrintAllAndFeatureSelect(paymentMethods, EditSelect.Row, c => c.Name);
                EditSelect.HandleSelectionInput(paymentMethods.Count, 0);
            }
            var paymentMethod = paymentMethods[EditSelect.Row];
            
            return paymentMethod;
        }
        public static Supplier SelectSupplier(MyDbContext db)
        {
            EditSelect.Reset();
            var suppliers = db.Suppliers.ToList();

            while (EditSelect.Selecting)
            {
                Console.Clear();
                PrintAllAndFeatureSelect(suppliers, EditSelect.Row, c => c.Name);
                EditSelect.HandleSelectionInput(suppliers.Count, 0);
            }
            var selectedSupplier = suppliers[EditSelect.Row];
            
            return selectedSupplier;
        }
        public static Category SelectCategory(MyDbContext db)
        {
            EditSelect.Reset();
            var categories = db.Categories.ToList();

            while (EditSelect.Selecting)
            {
                Console.Clear();
                PrintAllAndFeatureSelect(categories, EditSelect.Row, c => c.Name);
                EditSelect.HandleSelectionInput(categories.Count, 0);
            }
            var selectedCategory = categories[EditSelect.Row];
            
            return selectedCategory;
        }
        public static string SelectString(List<string> myList, string prompt)
        {
            EditSelect.Reset();

            while (EditSelect.Selecting)
            {
                Console.Clear();
                Console.WriteLine(prompt);                
                PrintAllAndFeatureSelect(myList, EditSelect.Row, c => c);
                EditSelect.HandleSelectionInput(myList.Count, 0);
            }
            var selectedString = myList[EditSelect.Row];
            
            return selectedString;
        }
        public static string SelectString(List<string> myList)
        {
            EditSelect.Reset();

            while (EditSelect.Selecting)
            {
                Console.Clear();
                PrintAllAndFeatureSelect(myList, EditSelect.Row, c => c);
                EditSelect.HandleSelectionInput(myList.Count, 0);  
            }
            var selectedString = myList[EditSelect.Row];
            
            return selectedString;
        }
        public static void PrintAllAndFeatureSelect<T>(List<T> Entities, int selected, Func<T, string> getText)   //Func<i want T in, i want string out> -> i send product in => and this is how i get string out        
        {
            int Width = Entities.Max(s => getText(s).Length) + 2;

            for (int i = 0; i < Entities.Count; i++)
            {
                var text = getText(Entities[i]).PadRight(Width);
                if (i == selected)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine(text);
                    Console.ResetColor();
                }
                else Console.WriteLine(text);
            }
        }
    }
}
