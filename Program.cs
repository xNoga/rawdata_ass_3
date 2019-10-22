using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace assignment_3
{
    class Program
    {
        // public static List<KeyValuePair<int, Category>> test = new List<KeyValuePair<int, Category>>();        
        //     test.Add(new KeyValuePair<int, Category>(1, new Category(1, "Beverages")));
        //     test.Add(new KeyValuePair<int, Category>(2, new Category(2, "Condiments")));
        //     test.Add(new KeyValuePair<int, Category>(3, new Category(3, "Confections")));
        //     test.Add(new KeyValuePair<int, Category>(4, new Category(4, "Seafood")));
        //     test.Add(new KeyValuePair<int, Category>(5, new Category(5, "Some more")));
        //     test.Add(new KeyValuePair<int, Category>(6, new Category(6, "We get the point")));
        public static List<Category> categories = new List<Category> {
            new Category(1, "Beverages"),
            new Category(2, "Condiments"),
            new Category(3, "Confections"),
        };
        static void Main(string[] args)
        {
            MyTcpListener tcpl = new MyTcpListener();
            tcpl.StartServerAsync().Wait();
        }
    }
}
