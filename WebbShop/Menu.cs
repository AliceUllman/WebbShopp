using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WebbShop;

public class Menu
{
    public List<MenuOption> Options { get; set; }
    public int Width { get; set; }
    public int MarkedOption {  get; set; } = 0;   
    public MenuOption SelectedOption { get; set; }
    private List<MenuOption> NavigationList { get; set; }
    private int SubPadding { get; set; }

    public Menu(List<MenuOption> options = null, int subPadding = 2) 
    {
        Options = options;
        SubPadding = subPadding;
        Width = GetMenuWidth();      
        Options[0].IsSelected = true;     
        SelectedOption = Options[0];
        NavigationList = UpdateVisibleOptions();
        NavigationList[0].IsMarked = true;
    }
    
    public void Navigate(ConsoleKey key, ControlStateMachine sm, Page currentPage)
    {

        NavigationList[MarkedOption].IsMarked = true;

        key = Console.ReadKey(true).Key;

        NavigationList[MarkedOption].IsMarked = false;

        if (key == ConsoleKey.DownArrow) MarkedOption = (MarkedOption + 1) % NavigationList.Count;
        else if (key == ConsoleKey.UpArrow) MarkedOption = (MarkedOption - 1 + NavigationList.Count) % NavigationList.Count;
        else if (key == ConsoleKey.Enter)
        {
            //Find previous selection
            var previouslySelected = NavigationList.FirstOrDefault(o => o.IsSelected);

            if (previouslySelected != null) previouslySelected.IsSelected = false;

            SelectedOption = NavigationList[MarkedOption];
            NavigationList[MarkedOption].IsSelected = true;
            
            if (!NavigationList[MarkedOption].IsSub)
            {
                NavigationList = UpdateVisibleOptions();
                MarkedOption = NavigationList.IndexOf(SelectedOption);
            }

            sm.ControlTransition(ControlStateMachine.ControlTrigger.MenuToPage);
            currentPage.ToggleWindowMark(sm); 
            //ClearMarks();
        }

        NavigationList[MarkedOption].IsMarked = true; 

        if (sm.CurrentState == ControlStateMachine.ControlState.Page) ClearMarks();
    }

    
    public void ClearMarks()
    {
        MarkedOption = 0;
        foreach (var option in Options)
        {
            option.IsMarked = false;
        }
    } 
    public void ReMark() 
    {
        NavigationList[0].IsMarked = true;
    }

    public void RenderMenu() 
    {           

        int OptionWidth = Options.Max(o => o.Name.Length);
        int line = 0;
        foreach (MenuOption option in Options) 
        {
            Console.SetCursorPosition(0, line);
            RenderOption(option, OptionWidth);
            line ++;
            //Render sub options only for selected parent options
            if (ShouldExpand(option))
            {
                Console.SetCursorPosition(0, line);
                int SubOptionWidth = option.SubOptions.Max(o => o.Name.Length);
                foreach (MenuOption subOption in option.SubOptions)
                {
                    line++;
                    RenderOption(subOption, SubOptionWidth, true);
                }
            }
        }
    }
    private int GetMenuWidth()
    {
        int maxWidth = 0;

        foreach (var option in Options)
        {
            int width = option.Name.Length;
            if (width > maxWidth) maxWidth = width;

            foreach (var sub in option.SubOptions)
            {
                width = sub.Name.Length + SubPadding;
                if (width > maxWidth) maxWidth = width;
            }
        }
        return maxWidth;
    }
    private void RenderOption(MenuOption option, int width, bool isSubOption = false)
    {
        string padding = isSubOption ? new string(' ', SubPadding) : ""; // indent subOption-options

        Console.Write(padding);
        // Decide colors
        if (option.IsMarked && option.IsSelected)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.White;
        }
        else if (option.IsMarked)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
        }
        else if (option.IsSelected)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        Console.WriteLine(option.Name.PadRight(width));

        // Reset after each line
        Console.ResetColor();
    }
    private bool ShouldExpand(MenuOption option)
    {
        if (option.IsSelected && !option.SubOptions.IsNullOrEmpty()) return true;

        //return true if any subOption is selected, to keep parent options expanded while navigating through sub options
        return option.SubOptions.Any(sub => sub.IsSelected); 
    }
    private List<MenuOption> UpdateVisibleOptions()
    {
        //Build a navigational list of options that include the sub options of selected parent options
        //so that it does not include the sub options of unselected parent options,
        //to allow navigation through the menu with correct indexing corresponding to what is rendered on the screen
        var visList = new List<MenuOption>();

        foreach (var option in Options)
        {
            visList.Add(option);

            
            if (ShouldExpand(option))
            {
                foreach (var subOption in option.SubOptions)
                {
                    visList.Add(subOption);
                }
            }
        }
        return visList;
    }

}
