using ATMApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.App
{
    class Entry
    {
        static void Main(string[] args)
        {
            
            ATMAppServices atmApp = new ATMAppServices();
            try
            {
                atmApp.CreateDB();
            }
            catch
            {
                Console.WriteLine("The DataBase already exists");
            }
            atmApp.CreateTable();
            atmApp.InsertUsers();
            atmApp.Run();              
           
        }
    }
}
