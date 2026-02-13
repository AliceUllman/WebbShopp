using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public class ProductWindow : Window
    {
        public Product Product { get; }
        public bool ShowDetails { get; set; }       
        
        public ProductWindow(Product product, bool showDetails = false) 
            : base(showDetails ? DetailedInfo(product): MinimalInfo(product), product.Name)
        {
            Product = product;
            ShowDetails = showDetails;
        }
        public void HandleProductInput(ConsoleKey key, ControlStateMachine sm, ShoppingCart shoppingCart)
        {
            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Escape) 
            { 
                sm.ControlTransition(ControlStateMachine.ControlTrigger.WindowToPage);
                this.Selected = false;
            }
            if (key == ConsoleKey.Enter) 
            {
                sm.ControlTransition(ControlStateMachine.ControlTrigger.WindowToPage);
                shoppingCart.AddToCart(Product);
                this.Selected = false;
            }                      
        }
        protected override void RenderBody(int x, int y)
        {
            //Override to make the last line green if window is marked
            for (int i = 0; i < Body.Count; i++)
            {
                if (Selected) RenderLine(x, y + 2 + i, Body[i], i == Body.Count - 1 ? ConsoleColor.Green : null);
                else RenderLine(x, y + 2 + i, Body[i]);
            }
        }

       
        private static List<string> MinimalInfo(Product product)
        {
            return new List<string> {
            $"Kategori: {product.Category.Name}",
            $"Pris: {product.Price}",
            product.InStock > 0 ? $"I Lager: {product.InStock}": "Slut i lager",
            "",
            "Lägg i varukorg"};
        }
        private static List<string> DetailedInfo(Product product)
        {
            return new List<string> {
            "",
            "Beskrivning:",
            {product.Description},
            $"Pris: {product.Price}",
            product.InStock > 0 ? $"I Lager: {product.InStock}": "Slut i lager",
            $"kategori: {product.Category?.Name}",
            $"Leverantör: {product.Supplier?.Name}",
            "",
            "Lägg i varukorg"};
        }
        private static List<string> FullInfo(Product product)
        {
            return new List<string> {
            "",
            "Beskrivning:",
            {product.Description},
            $"Pris: {product.Price}",
            product.InStock > 0 ? $"I Lager: {product.InStock}": "Slut i lager",
            $"Gömd: {product.Hidden}",
            $"Utvald: {product.FeaturedProduct}",
            $"kategori: {product.Category?.Name}",
            $"Leverantör: {product.Supplier?.Name}"};
        }
    }
}
