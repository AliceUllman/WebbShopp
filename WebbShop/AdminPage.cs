using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public class AdminPage : Page
    {
        public List<Window> AdminWindows { get; set; }
        public List<Window> EditorWindows { get; set; }
        public Window SelectedAdminWindow { get; set; }
        public MyDbContext DB { get; set; } //scuffed solution for getting the database connection to admin tools

        public AdminPage(string title, List<Window> adminWindows, List<Window> editorWindows, MyDbContext db) : base(title, windowsPerRow: 6) 
        {
            AdminWindows = adminWindows;
            EditorWindows = editorWindows;
            DB = db;
        }

        public override void Render()
        {
            CursorPosition.Reset();
            TitleWindow.Render();
            CursorPosition.NextRow(TitleWindow.Height);
            RenderWindows(MarkedWindowIndex);
            CursorPosition.NextRow(GetTallestWindowHeight() + 3);
            try 
            {
                EditorWindows[MarkedWindowIndex].Render();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Handle the case where MarkedWindowIndex is out of range
                EditorWindows[0].Render();
            }
        }
        public int GetTallestWindowHeight()
        {
            int maxHeight = 0;
            foreach (var window in AdminWindows)
            {
                if (window.Height > maxHeight)
                {
                    maxHeight = window.Height;
                }
            }
            
            return maxHeight;
        }
        protected override void RenderWindows(int selectedWindow)
        {

            for (int i = 0; i < AdminWindows.Count; i++)
            {
                if (i > 0 && i % WindowsPerRow == 0)
                {
                    CursorPosition.NextRow(AdminWindows[i - 1].Height);
                }
                AdminWindows[i].Render(i == selectedWindow);
            }

        }

        public void UpdateAdminWindows()
        {
            var adminWindows = new List<Window>();

            adminWindows.Add(new Window(DB.Products.Select(p => p.Name).ToList(), "Alla Producter:"));
            adminWindows.Add(new Window(DB.Categories.Select(c => c.Name).ToList(), "Alla Kategorier:"));
            adminWindows.Add(new Window(DB.Suppliers.Select(s => s.Name).ToList(), "Alla Leverantörer:"));
            adminWindows.Add(new Window(DB.PaymentMethods.Select(pm => pm.Name).ToList(), "Alla Betalnings Metoder:"));
            adminWindows.Add(new Window(DB.DeliveryMethods.Select(d => d.Name).ToList(), "Alla Leverans kontract:"));
            adminWindows.Add(new Window(DB.Orders.Select(o => $"Order Id:{o.Id} Datum: {o.OrderDate}").ToList(), "Alla Ordrar:"));

            AdminWindows = adminWindows;
        }

        

        public override void Navigate(ConsoleKey key, ControlStateMachine sm)
        {
            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.RightArrow)
            {
                MarkedWindowIndex = (MarkedWindowIndex + 1) % AdminWindows.Count;
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                MarkedWindowIndex = (MarkedWindowIndex - 1 + AdminWindows.Count) % AdminWindows.Count;
            }
            else if (key == ConsoleKey.Enter)
            {
                SelectedAdminWindow = AdminWindows[MarkedWindowIndex];
                SelectedAdminWindow.Selected = true;
                SelectedAdminWindow.MakeMarkVisible();
                sm.ControlTransition(ControlStateMachine.ControlTrigger.PageToWindow);
            }
            else if (key == ConsoleKey.Escape)
            {
                sm.ControlTransition(ControlStateMachine.ControlTrigger.PageToMenu);
                ToggleWindowMark(sm);
            }
        }

    }
}
