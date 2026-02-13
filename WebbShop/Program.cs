using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Collections.Generic;
using System.Globalization;

namespace WebbShop
{
    public class Program
    {
       
        static void Main(string[] args)
        {
            //if you get out of range exception, uncomment the line below to set the console window to the maximum size of your screen
            //it could cause the program to be a bit laggy, but it should work, otherwise just make the console window full screen manually
            //Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

            string connect = config.GetConnectionString("Default");

            using (var db = new MyDbContext(connect))
            {
                Shop myShop = new Shop(db);

                myShop.Run();
            }
        }
    
    }
}
