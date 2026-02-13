using Database;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebbShop;

public class Page
{      
    public Window TitleWindow { get; set; }
    public ProductWindow SelectedWindow { get; set; }
    public List<ProductWindow> ProductWindows { get; set; }
    
    public int WindowsPerRow { get; set; }
    public string Title { get; set; }
    public int MarkedWindowIndex { get; set; }

    public Page(string title, List<ProductWindow> productWindows = null, int windowsPerRow = 3)
    {
        Title = title;
        ProductWindows = productWindows;
        TitleWindow = new Window(new List<string> { }, title);
        WindowsPerRow = windowsPerRow;       
    }


    public virtual void Navigate(ConsoleKey key, ControlStateMachine sm)
    {
        //Pause wait for input
        key = Console.ReadKey(true).Key;

        //Change Index through input
        MatrixMath(key);
        
        if (key == ConsoleKey.Enter)
        {
            SelectedWindow = ProductWindows[MarkedWindowIndex];
            SelectedWindow.Selected = true;
            sm.ControlTransition(ControlStateMachine.ControlTrigger.PageToWindow);
        }
        else if (key == ConsoleKey.Escape)
        {
            sm.ControlTransition(ControlStateMachine.ControlTrigger.PageToMenu);
            ToggleWindowMark(sm); //Make it so no window is visibly marked
        }
    }
    public void ToggleWindowMark(ControlStateMachine sm)
    {
        if (sm.CurrentState == ControlStateMachine.ControlState.Page)
            MarkedWindowIndex = 0;
        else
            MarkedWindowIndex = - 1;
    }
    public void MatrixMath(ConsoleKey key)
    {
        int row = MarkedWindowIndex / WindowsPerRow;
        int column = MarkedWindowIndex % WindowsPerRow;

        if (key == ConsoleKey.RightArrow)
        {
            column++;

            if (row * WindowsPerRow + column >= ProductWindows.Count || column >= WindowsPerRow)
            { 
                column = 0;
            }
            MarkedWindowIndex = row * WindowsPerRow + column;
        }

        if (key == ConsoleKey.LeftArrow)
        {
            column--;

            if (column < 0)
            {
                column = WindowsPerRow - 1;
                if (row * WindowsPerRow + column >= ProductWindows.Count)
                {
                    column = (ProductWindows.Count - 1) % WindowsPerRow;
                }     
            }

            MarkedWindowIndex = row * WindowsPerRow + column;
        }

        if (key == ConsoleKey.UpArrow)
        {
            row--;

            if (row < 0)
            {
                row = (ProductWindows.Count - 1) / WindowsPerRow;
                if (row * WindowsPerRow + column >= ProductWindows.Count )
                    row--;
            }

            MarkedWindowIndex = row * WindowsPerRow + column;
        }

        if (key == ConsoleKey.DownArrow)
        {
            row++;


            if (row * WindowsPerRow + column >= ProductWindows.Count)
                row = 0;

            MarkedWindowIndex = row * WindowsPerRow + column;
        }
    }

    public virtual void Render()
    {
        CursorPosition.Reset();
        TitleWindow.Render();
        CursorPosition.NextRow(TitleWindow.Height);
        RenderWindows(MarkedWindowIndex);
    }

    protected virtual void RenderWindows(int selectedWindow)
    {
        
        for (int i = 0; i < ProductWindows.Count; i++)
        {
            if (i > 0 && i % WindowsPerRow == 0) 
            { 
                CursorPosition.NextRow(ProductWindows[i - 1].Height); 
            }
            ProductWindows[i].Render(i == selectedWindow);
        }
        
    }
    public static List<ProductWindow> BuildProductWindows(List<Product> products, bool detailedWidows = false)
    {
        var windows = new List<ProductWindow>();
        foreach (var product in products)
        {
            windows.Add(new ProductWindow(product, detailedWidows));
        }
        return windows;
    }
}
