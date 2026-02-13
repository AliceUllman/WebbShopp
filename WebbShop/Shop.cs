using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static WebbShop.ControlStateMachine;

namespace WebbShop
{
    public class Shop
    {
        public int MyProperty { get; set; }
        private Menu Menu { get; set; }
        private Page MainPage { get; set; }
        private Page AllCategoryPage { get; set; }
        private List<Page> CategoryPages { get; set; }
        private CartPage ShoppingCartPage { get; set; }
        private ShoppingCart ShoppingCart { get; set; }
        private Page AdminPage { get; set; }
        private Page CurrentPage;
        private MyDbContext DB { get; set; }
        public Shop(MyDbContext db) 
        { 
            DB = db;
                      
            MainPage = BuildMainPage();
            AllCategoryPage = BuildAllCategoryPage();
            CategoryPages = BuildCategoryPages();

            ShoppingCart = new ShoppingCart();
            ShoppingCartPage = BuildShoppingCartPage(); 
            AdminPage = BuildAdminPage();
            
            Menu = BuildMenu();
            CurrentPage = MainPage;
        }

        public void Run()
        {
            Console.CursorVisible = false;
            
            ConsoleKey key = ConsoleKey.NoName;

            var sm = new ControlStateMachine();

            bool running = true;
            while (running)
            {

                switch (sm.CurrentState)
                {
                    case ControlState.Menu:

                        Console.Clear();                  

                        Menu.RenderMenu();
                        
                        CurrentPage.Render();

                        Console.SetCursorPosition(0, Menu.Options.Count + 10);
                        Console.WriteLine($"{sm.CurrentState}");

                        Menu.Navigate(key, sm, CurrentPage);
                        CurrentPage = Menu.SelectedOption.LinkedPage;
                        break;

                    case ControlState.Page:
                        Console.Clear();
                        
                        Menu.RenderMenu();
                        CurrentPage.Render();

                        Console.SetCursorPosition(0, Menu.Options.Count + 10);
                        Console.WriteLine(sm.CurrentState);

                        if (CurrentPage is CartPage cart)
                        {
                            cart.Navigate(key, sm, DB);
                        }
                        else if (CurrentPage is AdminPage adminPage) 
                        {
                            adminPage.Navigate(key, sm);
                        }
                        else CurrentPage.Navigate(key, sm);

                        if(key == ConsoleKey.Escape)
                        {
                            Menu.SelectedOption.IsMarked = true;
                        }
                        break;

                    case ControlState.Window:

                        Console.Clear();
                        Menu.RenderMenu();
                        CurrentPage.Render();

                        Console.SetCursorPosition(0, Menu.Options.Count + 10);
                        Console.WriteLine(sm.CurrentState);
                        
                        if (CurrentPage is CartPage cartPage)
                        {
                            cartPage.Window.HandleCartInput(key, sm, cartPage);
                            
                        }
                        else if (CurrentPage.SelectedWindow is ProductWindow window)
                        {
                            window.HandleProductInput(key, sm, ShoppingCart);
                            ShoppingCartPage.UpdateCartWindows();
                        }
                        else if (CurrentPage is AdminPage adminPage)
                        {
                            adminPage.SelectedAdminWindow.HandleAdminInput(key, sm, adminPage);
                        }

                        break;

                    case ControlState.Editor:

                        Console.Clear();
                        Menu.RenderMenu();
                        CurrentPage.Render();

                        Console.SetCursorPosition(0, Menu.Options.Count + 10);
                        Console.WriteLine(sm.CurrentState);

                        if (CurrentPage is AdminPage adminP)
                        {
                            adminP.SelectedAdminWindow.HandleAdminEditorInput(key, sm, adminP);
                        }

                        UppdateEverything();
                        break;
                }
            }
        }

        public void UppdateEverything()
        {
            MainPage = BuildMainPage();
            AllCategoryPage = BuildAllCategoryPage();
            CategoryPages = BuildCategoryPages();
        }
        public Menu BuildMenu() 
        { 
            
            var myOptions = new List<MenuOption>();
            myOptions.Add(new MenuOption(MainPage, "Hem"));

            var categoryOption = new MenuOption(AllCategoryPage, "Product Kategorier");
            categoryOption.AddSubOptionsForPages(CategoryPages);

            myOptions.Add(categoryOption);

            myOptions.Add(new MenuOption(ShoppingCartPage, "Varukorg"));
            myOptions.Add(new MenuOption(AdminPage, "Admin"));
            var menu = new Menu(myOptions, 4);

            CursorPosition.MenuSpace = menu.Width + 2;

            return menu;
        }
        public Page BuildMainPage() 
        {
            var pageProducts = GetFeaturedProducts(DB);
            Page mainPage = new Page("Webb Shop 9000!", Page.BuildProductWindows(pageProducts, true), 3);
            mainPage.MarkedWindowIndex = -1;//set mark to invalid index so that no product is marked at start
            return mainPage;
        }
        public List<Page> BuildCategoryPages() 
        { 
            var categoryPages = new List<Page>();

            var categories = DB.Categories.ToList();//Get all categories from DB to prevent multiple calls

            foreach (var category in categories)
            {
                var categoryProducts = GetAllCategoryProducts(DB, category);
                var categoryPage = new Page(category.Name, Page.BuildProductWindows(categoryProducts), 6);
                categoryPages.Add(categoryPage);
            }
            return categoryPages;        
        }
        public Page BuildAllCategoryPage()
        {
            var pageProducts = GetVisibleProducts(DB);

            Page mainPage = new Page("Alla producter:", Page.BuildProductWindows(pageProducts), 6);
            return mainPage;
        }
        public CartPage BuildShoppingCartPage() 
        {
            var emptyCartWindow = new Window(new List<string> { }, "Din varukorg är tom!");
            var checkOutWindow = new Window(new List<string> { $"Total: {ShoppingCart.GetTotalPrice()} kr" }, "Kan inte gå till kassa");
            return new CartPage("Din Varukorg:", emptyCartWindow, checkOutWindow, ShoppingCart);
        }
        public AdminPage BuildAdminPage() 
        {       
            var adminWindows = new List<Window>();

            adminWindows.Add(new Window(DB.Products.Select(p => p.Name).ToList(), "Alla Producter:"));
            adminWindows.Add(new Window(DB.Categories.Select(c => c.Name).ToList(), "Alla Kategorier:"));
            adminWindows.Add(new Window(DB.Suppliers.Select(s => s.Name).ToList(), "Alla Leverantörer:"));
            adminWindows.Add(new Window(DB.PaymentMethods.Select(pm => pm.Name).ToList(), "Alla Betalnings Metoder:"));
            adminWindows.Add(new Window(DB.DeliveryMethods.Select(d => d.Name).ToList(), "Alla Leverans kontract:"));
            adminWindows.Add(new Window(DB.Orders.Select(o => $"Order Id:{o.Id} Datum: {o.OrderDate}").ToList(), "Alla Ordrar:"));

            var editorWindows = new List<Window>();

            editorWindows.Add(new Window(new List<string> { "Lägg till ny product", "Redigera product", "Ta bort product" }, "Product Editor:"));
            editorWindows.Add(new Window(new List<string> { "Lägg till ny kategori", "Redigera kategori", "Ta bort kategori" }, "Kategori Editor:"));
            editorWindows.Add(new Window(new List<string> { "Lägg till ny leverantör", "Redigera leverantör", "Ta bort leverantör" }, "Leverantör Editor:"));
            editorWindows.Add(new Window(new List<string> { "Lägg till ny betalnings metod", "Redigera betalnings metod", "Ta bort betalnings metod" }, "Betalnings Metod Editor:"));
            editorWindows.Add(new Window(new List<string> { "Lägg till ny Leverans metod", "Redigera Leverans metod", "Ta bort Leverans metod" }, "Leverans Editor:"));
            editorWindows.Add(new Window(new List<string> { "Redigera order", "Ta bort order" }, "Order Editor:"));

            return new AdminPage("Admin", adminWindows, editorWindows, DB);
        }


        static public List<Product> GetFeaturedProducts(MyDbContext db)
        {
            return db.Products
                 .Where(p => p.FeaturedProduct == true)
                 .Where(p => p.Hidden != true)
                 .Include(p => p.Category)
                 .Include(p => p.Supplier)
                 .ToList();
        }
        static public List<Product> GetVisibleProducts(MyDbContext db)
        {
            return db.Products
                 .Where(p => p.Hidden != true)
                 .Include(p => p.Category)
                 .Include(p => p.Supplier)
                 .ToList();


        }
        static public List<Product> GetAllCategoryProducts(MyDbContext db, Category category)
        {
            return db.Products
                 .Where (p => p.Category == category)
                 .Where(p => p.Hidden != true)
                 .Include(p => p.Category)
                 .Include(p => p.Supplier)
                 .ToList();
        }
    }
}
