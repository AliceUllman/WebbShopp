using Database;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WebbShop
{
    public class Window
    {
        public string Head { get; set; }
        public List<string> Body { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MarkedLineIndex { get; set; }
        public bool Selected { get; set; }

        public string SelectedLineValue { get; set; }
        public Window(List<string> body, string head = "")
        {
            Head = head;
            Body = body;
            Width = body.DefaultIfEmpty("").Max(s => s.Length);
            if (Width < Head.Length + 4) Width = Head.Length + 4;
            Height = Body.Count();
            MakeMarkInvisible();
        }
       
        public void HandleCartInput(ConsoleKey key, ControlStateMachine sm, CartPage cartPage)
        {
            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow) MarkedLineIndex = (MarkedLineIndex + 1) % Body.Count;

            else if (key == ConsoleKey.UpArrow) MarkedLineIndex = (MarkedLineIndex - 1 + Body.Count) % Body.Count;

            else if (key == ConsoleKey.Escape)
            { 
                sm.ControlTransition(ControlStateMachine.ControlTrigger.WindowToPage);
                MakeMarkInvisible();
            }
            else if (key == ConsoleKey.Enter)
            {
                if (!cartPage.ShoppingCart.ItemsInCart.IsNullOrEmpty())
                {
                    cartPage.ShoppingCart.EditItem(MarkedLineIndex, sm);
                    cartPage.UpdateCartWindows();
                }                
            }
        }
        public void HandleAdminInput(ConsoleKey key, ControlStateMachine sm, AdminPage adminPage)
        {
            key = Console.ReadKey(true).Key;
            if (Body.Count != 0)
            {
                if (key == ConsoleKey.DownArrow) MarkedLineIndex = (MarkedLineIndex + 1) % Body.Count;
                else if (key == ConsoleKey.UpArrow) MarkedLineIndex = (MarkedLineIndex - 1 + Body.Count) % Body.Count;
            }
            
            if (key == ConsoleKey.Escape)
            {
                sm.ControlTransition(ControlStateMachine.ControlTrigger.WindowToPage);
                MakeMarkInvisible();
            }

            else if (key == ConsoleKey.Enter)
            {
                try 
                {
                    adminPage.EditorWindows[adminPage.MarkedWindowIndex].SelectedLineValue = Body[MarkedLineIndex]; 
                }
                catch (ArgumentOutOfRangeException) 
                { 
                    
                }

                sm.ControlTransition(ControlStateMachine.ControlTrigger.WindowToEditor);
                adminPage.SelectedAdminWindow = adminPage.EditorWindows[adminPage.MarkedWindowIndex];
                adminPage.EditorWindows[adminPage.MarkedWindowIndex].MakeMarkVisible();
            }
            
        }
        public void HandleAdminEditorInput(ConsoleKey key, ControlStateMachine sm, AdminPage adminPage)
        {
            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow) MarkedLineIndex = (MarkedLineIndex + 1) % Body.Count;
            else if (key == ConsoleKey.UpArrow) MarkedLineIndex = (MarkedLineIndex - 1 + Body.Count) % Body.Count;
            else if (key == ConsoleKey.Escape)
            {
                MakeMarkInvisible();
                adminPage.SelectedAdminWindow = adminPage.AdminWindows[adminPage.MarkedWindowIndex];
                adminPage.SelectedAdminWindow.MakeMarkVisible();
                sm.ControlTransition(ControlStateMachine.ControlTrigger.EditorToWindow);

            }
            else if (key == ConsoleKey.Enter)
            {
                string selectedLine = Body[MarkedLineIndex];
                string selectedWindow = Head;
                AdminSwitch(selectedWindow, selectedLine, adminPage.DB);

                adminPage.UpdateAdminWindows();
                MakeMarkVisible();
            }
        }

        public void AdminSwitch(string someting, string sometingElse, MyDbContext db) 
        {
            Console.Clear();
            switch (someting) 
            { 
                case "Product Editor:":

                    Product product = db.Products.FirstOrDefault(p => p.Name == SelectedLineValue);

                    switch (sometingElse)
                    {
                        case "Lägg till ny product":
                            AdminTools.CreateNewProduct(db);
                            break;
                        case "Ta bort product":
                            AdminTools.DeleteProduct( db , product);
                            break;
                        case "Redigera product":
                            AdminTools.EditProduct(db, product);
                            break;
                        default:                         
                            break;
                    }
                    break;
                case "Kategori Editor:":

                    Category category = db.Categories.FirstOrDefault(c => c.Name == SelectedLineValue);

                    switch (sometingElse)
                    {
                        case "Lägg till ny kategori":
                            AdminTools.CreateNewCategory(db);
                            break;
                        case "Ta bort kategori":
                            AdminTools.DeleteCategory(db, category);
                            break;
                        case "Redigera kategori":
                            AdminTools.EditCategory(db, category);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Leverantör Editor:":

                    Supplier supplier = db.Suppliers.FirstOrDefault(s => s.Name == SelectedLineValue);

                    switch (sometingElse)
                    {
                        case "Lägg till ny leverantör":
                            AdminTools.CreateNewSupplier(db);
                            break;
                        case "Ta bort leverantör":
                            AdminTools.DeleteSupplier(db, supplier);
                            break;
                        case "Redigera leverantör":
                            AdminTools.EditSupplier(db, supplier);           
                            break;
                        default:
                            break;
                    }
                    break;
                case "Betalnings Metod Editor:":

                    PaymentMethod paymentMethod = db.PaymentMethods.FirstOrDefault(pm => pm.Name == SelectedLineValue);

                    switch (sometingElse)
                    {
                        case "Lägg till ny betalnings metod":
                            AdminTools.CreateNewPaymentMethod(db);
                            break;
                        case "Ta bort betalnings metod":
                            AdminTools.DeletePaymentMethod(db, paymentMethod);
                            break;
                        case "Redigera betalnings metod":
                            AdminTools.EditPaymentMethod(db, paymentMethod);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Leverans Editor:":

                    DeliveryMethod deliveryMethod = db.DeliveryMethods.FirstOrDefault(dm => dm.Name == SelectedLineValue);

                    switch (sometingElse) 
                    {
                        case "Lägg till ny Leverans metod":
                            AdminTools.CreateNewDeliveryMethod(db);
                            break;
                        case "Redigera Leverans metod":
                            AdminTools.EditDeliveryMethod(db, deliveryMethod);
                            break;
                        case "Ta bort Leverans metod":
                            AdminTools.DeleteDeliveryMethod(db, deliveryMethod);
                            break;
                    }
                    break;
                case "Order Editor:":
                    
                    switch (sometingElse)
                    {
                        case "Redigera order":
                            break;
                        case "Ta bort order":
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        public void MakeMarkInvisible() { MarkedLineIndex = - 1; }
        public void MakeMarkVisible() { MarkedLineIndex = 0; }
       
        public void Render(bool colorHead = false)
        {
            int x = CursorPosition.X;
            int y = CursorPosition.Y;
            Console.SetCursorPosition(x, y);

            //Print top frame
            Console.WriteLine('┌' + new string('─', Width + 2) + '┐');
    
            //Print Header
            RenderLine(x, y + 1, Head, colorHead ? ConsoleColor.Blue : null);

            //Draw body
            RenderBody(x, y);

            //Print bottom frame
            Console.SetCursorPosition(x, y + Height +  2);
            Console.Write('└' + new string('─', Width + 2) + '┘');

            //Set next window position
            CursorPosition.X = x + Width + 4;
        }
        protected virtual void RenderBody(int x, int y) 
        {
            for (int i = 0; i < Body.Count; i++)
            {
                if (MarkedLineIndex == i) RenderLine(x, y + 2 + i, Body[i], ConsoleColor.Black, ConsoleColor.White);
                else RenderLine(x, y + 2 + i, Body[i]);
            }
        }
        protected void RenderLine(int x, int y, string text, ConsoleColor? textColor = null, ConsoleColor? backgroundColor = null)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("│ ");

            if (textColor != null) Console.ForegroundColor = textColor.Value;
            if (backgroundColor != null) Console.BackgroundColor = backgroundColor.Value;

            Console.Write(text);

            Console.ResetColor();

            Console.Write(new string(' ', Width - text.Length + 1));
            Console.Write("│");
        }
        
    }
    
}
