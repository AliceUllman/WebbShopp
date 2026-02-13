using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop;

public class MenuOption
{
    public Page LinkedPage { get; set; }
    public string Name { get; set; }
    public List<MenuOption> SubOptions { get; set; } = new List<MenuOption>();
    public bool IsMarked { get; set; } 
    public bool IsSelected { get; set; }
    public bool IsSub { get; set; }
    public MenuOption(Page linkedPage, string name) 
    { 
        LinkedPage = linkedPage;
        Name = name;
    }
    public void AddSubOptionsForPages(List<Page> pages)
    {
        foreach (var page in pages)
        {
            var subOption = new MenuOption(page, page.Title);
            subOption.IsSub = true;
            this.SubOptions.Add(subOption);
        }
    }
}
