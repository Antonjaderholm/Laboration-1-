// See https://aka.ms/new-console-template for more information

/*using System;
using System.Collections.Generic;

public class CustomerService
{
    public static void ProcessQueue(Queue<string> customers)
    {
        while (customers.Count > 0)
        {
            string customer = customers.Dequeue();
            Console.WriteLine($"Betjänar: {customer}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Skapa en kö med kunder
        Queue<string> customerQueue = new Queue<string>();
        customerQueue.Enqueue("Kalle");
        customerQueue.Enqueue("Lisa");
        customerQueue.Enqueue("Peter");

        // Behandla kunderna i kön
        CustomerService.ProcessQueue(customerQueue);

        Console.ReadKey();
    }
}*/


/*
using System;
using System.Collections.Generic;

public class QueueHelper
{
    public static (T, int) PeekAndCount<T>(Queue<T> queue)
    {
        // Kontrollera om kön är tom
        if (queue.Count == 0)
        {
            return (default(T), 0); // Returnera (default(T), 0) om kön är tom
        }

        // Hämta det första elementet utan att ta bort det
        T firstElement = queue.Peek();

        // Räkna antalet återstående element
        int remainingCount = queue.Count;

        // Returnera det första elementet och antalet återstående element
        return (firstElement, remainingCount);
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Skapa en kö
        Queue<int> myQueue = new Queue<int>();
        myQueue.Enqueue(1);
        myQueue.Enqueue(2);
        myQueue.Enqueue(3);
        myQueue.Enqueue(4);

        // Anropa PeekAndCount-metoden
        (int firstElement, int remainingCount) = QueueHelper.PeekAndCount(myQueue);

        // Skriv ut resultatet
        Console.WriteLine($"Första element: {firstElement}");
        Console.WriteLine($"Antal återstående element: {remainingCount}");

        Console.ReadKey();
    }
}
*/

/*using System;
using System.Collections.Generic;

public class BrowserHistory
{
    public static string GoBack(Stack<string> history)
    {
        // Kontrollera om stacken är tom
        if (history.Count == 0)
        {
            return "Inga tidigare webbadresser att gå tillbaka till.";
        }

        // Ta bort den översta webbadressen
        history.Pop();

        // Kontrollera om stacken är tom efter att ha tagit bort elementet
        if (history.Count == 0)
        {
            return "Inga tidigare webbadresser att gå tillbaka till.";
        }

        // Returnera den nya översta webbadressen
        return history.Peek();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Skapa en stack med webbadresser
        Stack<string> history = new Stack<string>();
        history.Push("www.google.com");
        history.Push("www.youtube.com");
        history.Push("www.instagram.com");

        // Anropa GoBack-metoden
        string currentUrl = BrowserHistory.GoBack(history);

        // Skriv ut den nya översta webbadressen
        Console.WriteLine($"Nuvarande webbadress: {currentUrl}");

        Console.ReadKey();
    }
}*/

/*using System;
using System.Collections.Generic;
public class PhoneBook
{
    private Dictionary<string, string> phoneNumbers = new Dictionary<string, string>();

    public void AddEntry(string name, string number)
    {
        if (phoneNumbers.ContainsKey(name))
        {
            Console.WriteLine($"Ett nummer för {name} finns redan.");
        }
        else
        {
            phoneNumbers.Add(name, number);
            Console.WriteLine($"Numret {number} lades till för {name}.");
        }
    }

    public void LookupNumber(string name)
    {
        if (phoneNumbers.ContainsKey(name))
        {
            Console.WriteLine($"Telefonnumbret för {name} är: {phoneNumbers[name]}");
        }
        else
        {
            Console.WriteLine($"inget numer hittades för {name}");
        }
    }
    class program
    {
        static void Main(string[] args)
        {
            PhoneBook phoneBook = new PhoneBook();

            while (true)
            {
                Console.WriteLine("1. Lägg till");
                Console.WriteLine("2. sök");
                Console.WriteLine("3. Avsluta");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Namn: ");
                        string name = Console.ReadLine();
                        Console.Write("Nummer: ");
                        string number = Console.ReadLine();
                        phoneBook.AddEntry(name, number);
                        break;
                    case "2":
                        Console.Write("Namn: ");
                        string searchName = Console.ReadLine();
                        phoneBook.LookupNumber(searchName);
                        break;
                    case "3":
                        Console.WriteLine("Avslutar.");
                        return;
                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                }
            }
        }
    }

}*/
/*using /*System;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        // Create a SortedList to store products
        SortedList<string, decimal> products = new SortedList<string, decimal>();

        // Call the method to add and print products
        AddAndPrintProducts(products);
    }

    // Method to add and print products
    public static void AddAndPrintProducts(SortedList<string, decimal> products)
    {
        Console.WriteLine("Enter products (or 'done' to finish):");

        // Loop for adding products
        while (true)
        {
            Console.Write("Product name: ");
            string name = Console.ReadLine();

            // Check if user wants to quit
            if (name.ToLower() == "done")
            {
                break;
            }

            Console.Write("Product price: ");
            string priceInput = Console.ReadLine();

            decimal price;
            if (decimal.TryParse(priceInput, out price))
            {
                products.Add(name, price);
            }
            else
            {
                Console.WriteLine("Invalid price. Please enter a valid decimal number.");
            }
        }

        // Print sorted products
        Console.WriteLine("\nSorted Products:");
        foreach (KeyValuePair<string, decimal> product in products)
        {
            Console.WriteLine($"{product.Key}: {product.Value}");
        }

        // Print a separate list of prices
        Console.WriteLine("\nPrices:");
        foreach (decimal price in products.Values)
        {
            Console.WriteLine(price);
        }
    }
}*/

/* SortedList<string, int> produktLista = new SortedList<string, int>();
 SortedList<string, int> läggTillProdukt(SortedList<string, int> SL, string s, int i)
{
  SL.Add(s, i);
  return SL;
}

läggTillProdukt(produktLista, "MANGO", 20);
läggTillProdukt(produktLista, "banan", 12);
läggTillProdukt(produktLista, "äpple", 7);

foreach (KeyValuePair<string, int> E in produktLista)
{
  Console.WriteLine(E);
}*/

/*using System;

public struct Rectangle
{
    public int Width;
    public int Height;

    // metoden för att räkna arean 
    public int Area()
    {
        return Width * Height;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // rektangel insats
        Rectangle rect = new Rectangle { Width = 5, Height = 10 };

        // räkna ut och printa arean 
        int area = rect.Area();
        Console.WriteLine($"Area of the rectangle: {area}");
    }
}*/


/*using System;

public struct Book
{
    public string Title;
    public string Author;
    public int NumberOfPages;

  
    public string GetDescription()
    {
        return $"Title: {Title}, Author: {Author}, Pages: {NumberOfPages}";
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Create a Book instance
        Book book = new Book
        {
            Title = "Usain bolt, en sann sverigedemokrat",
            Author = "Anton Jäderholm",
            NumberOfPages = 300
        };
        string description = book.GetDescription();
        Console.WriteLine(description);
        {

        }
    }
   

}*/

/*using System.Security.AccessControl;

public record Car(string Audi, string Rs7, bool Contains2026);

public class Program
{
    public static void Main(string[] args)
    {
        Car p1 = new Car("Audi", "Rs7", false);

        
        Car p2 = p1 with { Contains2026 = true };

        Console.WriteLine(p1);
        Console.WriteLine(p2);
    }
}*/

public record Person(string name, double price, int quantity);

public class Program
{
    public static void Main(string[] args)
    {
        
        Person p1 = new Person("Johan", 2.0, 4 );

        Person p2 = p1 with { quantity = 1 };

        Console.WriteLine(p1);
        Console.WriteLine(p2);
    }
}



