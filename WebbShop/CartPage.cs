using Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;


namespace WebbShop
{
    public class CartPage : Page
    {
        public ShoppingCart ShoppingCart { get; set; }
        public Window Window { get; set; }
        public Window CheckOutWindow { get; set; }

        public CartPage(string title, Window window, Window checkOutWindow, ShoppingCart cart) 
            : base(title)
        {
            Window = window;
            CheckOutWindow = checkOutWindow;
            ShoppingCart = cart;
        }

        public void Navigate(ConsoleKey key, ControlStateMachine sm, MyDbContext db)
        {

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow)
            {
                MarkedWindowIndex = (MarkedWindowIndex + 1) % 2;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                MarkedWindowIndex = (MarkedWindowIndex - 1 + 2) % 2;
            }
            else if (key == ConsoleKey.Enter)
            {
                if (MarkedWindowIndex == 0)
                {
                    if (!ShoppingCart.ItemsInCart.IsNullOrEmpty())
                    {
                        sm.ControlTransition(ControlStateMachine.ControlTrigger.PageToWindow);
                    }
                }
                else if (MarkedWindowIndex == 1)
                {
                    ShoppingCart.CheckOut(db);
                }
                    
            }
            else if (key == ConsoleKey.Escape)
            {
                sm.ControlTransition(ControlStateMachine.ControlTrigger.PageToMenu);
                ToggleWindowMark(sm);
            }
        }
        

        public void UpdateCartWindows()
        {
            if (ShoppingCart.ItemsInCart.Count == 0)
            {
                Window =  new Window(new List<string> { }, "Din varukorg är tom!");
                CheckOutWindow = new Window(new List<string> { $"Total: {ShoppingCart.GetTotalPrice()} kr", "Kan inte gå till kassa" });
            }
            else
            {
                Window = new Window(ShoppingCart.BuildCartRows(), "Din varukorg");
                CheckOutWindow = new Window(new List<string> { $"Total: {ShoppingCart.GetTotalPrice()} kr" }, "Gå till kassa");
            }
                
        }
        public override void Render()
        {
            CursorPosition.Reset();
            TitleWindow.Render();
            CursorPosition.NextRow(TitleWindow.Height);
            Window.Render(MarkedWindowIndex == 0);
            CursorPosition.NextRow(Window.Height);
            CheckOutWindow.Render(MarkedWindowIndex == 1);
        }
    }
}
