// See https://aka.ms/new-console-template for more information
using System;
using System.IO;

/*public class Program
{
    static void Main(string[] args)
    {
        // 1. Skriv ut pathen där programmet körs
        Console.WriteLine($"Programmet körs i: {Directory.GetCurrentDirectory()}");

        // 2. Skapa en ny directory, "new folder"
        string newFolderName = "new folder";
        string newFolderPath = Path.Combine(Directory.GetCurrentDirectory(), newFolderName);
        Directory.CreateDirectory(newFolderPath);

        // 3. Skapa en fil, test.txt, i "new folder"
        string testFileName = "test.txt";
        string testFilePath = Path.Combine(newFolderPath, testFileName);
        File.Create(testFilePath).Close();

        Console.WriteLine($"Ny mapp skapad: {newFolderPath}");
        Console.WriteLine($"Ny fil skapad: {testFilePath}");
    }
}
    */
/*using System;

public class Program
{
    static void Main(string[] args)
    {
        // Få OS-versionen
        OperatingSystem os = Environment.OSVersion;

        // Skriv ut OS-versionen
        Console.WriteLine(os);
    }
}
*/

/*using System;
using System.IO;

public class Program
{
    static void Main(string[] args)
    {
        // Hämta sökvägen till den skapade filen
        string newFolderName = "new folder";
        string testFileName = "test.txt";
        string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), newFolderName, testFileName);

        // Hämta filtillägget
        string fileExtension = Path.GetExtension(testFilePath);

        // Skriv ut filtillägget och pathen separat
        Console.WriteLine($"Filtillägg: {fileExtension}");
        Console.WriteLine($"Sökväg: {Path.GetDirectoryName(testFilePath)}");
    }
}
*/
/*
public class Product
{
    public string Name { get; }
    public decimal Price { get; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}

public class Customer
{
    public string Name { get; }
    private string Password { get; }
    private List<(Product Product, int Quantity)> _cart;

    public Customer(string name, string password)
    {
        Name = name;
        Password = password;
        _cart = new List<(Product, int)>();
    }

    public bool VerifyPassword(string password)
    {
        return Password == password;
    }

    public void AddToCart(Product product, int quantity)
    {
        var existingItem = _cart.Find(item => item.Product.Name == product.Name);
        if (existingItem.Product != null)
        {
            _cart.Remove(existingItem);
            _cart.Add((existingItem.Product, existingItem.Quantity + quantity));
        }
        else
        {
            _cart.Add((product, quantity));
        }
    }

    public List<(Product Product, int Quantity)> GetCart()
    {
        return new List<(Product, int)>(_cart);
    }

    public decimal GetTotalPrice()
    {
        return _cart.Sum(item => item.Product.Price * item.Quantity);
    }

    public override string ToString()
    {
        var cartString = string.Join("\n", _cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name}\nKundvagn:\n{cartString}\nTotalt: {GetTotalPrice():C}";
    }




    class Program
    {
        private static List<Customer> customers = new List<Customer>
    {
        new Customer("Knatte", "123"),
        new Customer("Fnatte", "321"),
        new Customer("Tjatte", "213")
    };

        private static List<Product> products = new List<Product>
    {
        new Product("Korv", 10m),
        new Product("Dricka", 15m),
        new Product("Äpple", 5m)
    };

        private static Customer currentCustomer;

        static void Main(string[] args)
        {
            while (true)
            {
                if (currentCustomer == null)
                {
                    ShowMainMenu();
                }
                else
                {
                    ShowShoppingMenu();
                }
            }
        }

        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Välkommen till affären!");
            Console.WriteLine("1. Logga in");
            Console.WriteLine("2. Registrera ny kund");
            Console.WriteLine("3. Avsluta");

            switch (Console.ReadLine())
            {
                case "1":
                    Login();
                    break;
                case "2":
                    RegisterNewCustomer();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }
        }

        static void Login()
        {
            Console.Write("Ange namn: ");
            string name = Console.ReadLine();
            Console.Write("Ange lösenord: ");
            string password = Console.ReadLine();

            currentCustomer = customers.FirstOrDefault(c => c.Name == name);

            if (currentCustomer == null)
            {
                Console.WriteLine("Kunden finns inte. Vill du registrera en ny kund? (j/n)");
                if (Console.ReadLine().ToLower() == "j")
                {
                    RegisterNewCustomer();
                }
            }
            else if (!currentCustomer.VerifyPassword(password))
            {
                Console.WriteLine("Fel lösenord. Försök igen.");
                currentCustomer = null;
            }
            else
            {
                Console.WriteLine($"Välkommen, {currentCustomer.Name}!");
            }

            Console.ReadKey();
        }

        static void RegisterNewCustomer()
        {
            Console.Write("Ange namn: ");
            string name = Console.ReadLine();
            Console.Write("Ange lösenord: ");
            string password = Console.ReadLine();

            if (customers.Any(c => c.Name == name))
            {
                Console.WriteLine("En kund med det namnet finns redan.");
            }
            else
            {
                Customer newCustomer = new Customer(name, password);
                customers.Add(newCustomer);
                currentCustomer = newCustomer;
                Console.WriteLine("Ny kund registrerad och inloggad!");
            }

            Console.ReadKey();
        }

        static void ShowShoppingMenu()
        {
            Console.Clear();
            Console.WriteLine($"Inloggad som: {currentCustomer.Name}");
            Console.WriteLine("1. Handla");
            Console.WriteLine("2. Se kundvagn");
            Console.WriteLine("3. Gå till kassan");
            Console.WriteLine("4. Logga ut");

            switch (Console.ReadLine())
            {
                case "1":
                    Shop();
                    break;
                case "2":
                    ViewCart();
                    break;
                case "3":
                    Checkout();
                    break;
                case "4":
                    currentCustomer = null;
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }
        }

        static void Shop()
        {
            Console.Clear();
            Console.WriteLine("Tillgängliga produkter:");
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i].Name} - {products[i].Price:C}");
            }

            Console.Write("Välj produkt (nummer): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= products.Count)
            {
                Console.Write("Ange antal: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    currentCustomer.AddToCart(products[choice - 1], quantity);
                    Console.WriteLine("Produkt tillagd i kundvagnen.");
                }
                else
                {
                    Console.WriteLine("Ogiltigt antal.");
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }

            Console.ReadKey();
        }

        static void ViewCart()
        {
            Console.Clear();
            Console.WriteLine(currentCustomer.ToString());
            Console.ReadKey();
        }

        static void Checkout()
        {
            Console.Clear();
            Console.WriteLine(currentCustomer.ToString());
            Console.WriteLine("Tack för ditt köp!");
            currentCustomer = null;
            Console.ReadKey();
        }
    }
}
*/
/*
public class Product
{
    public string Name { get; }
    public decimal Price { get; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}


public abstract class Customer
{
    public string Name { get; }
    protected string Password { get; }
    protected List<(Product Product, int Quantity)> _cart;
    public abstract double DiscountPercentage { get; }

    public Customer(string name, string password)
    {
        Name = name;
        Password = password;
        _cart = new List<(Product, int)>();
    }

    public bool VerifyPassword(string password)
    {
        return Password == password;
    }

    public void AddToCart(Product product, int quantity)
    {
        var existingItem = _cart.Find(item => item.Product.Name == product.Name);
        if (existingItem.Product != null)
        {
            _cart.Remove(existingItem);
            _cart.Add((existingItem.Product, existingItem.Quantity + quantity));
        }
        else
        {
            _cart.Add((product, quantity));
        }
    }

    public List<(Product Product, int Quantity)> GetCart()
    {
        return new List<(Product, int)>(_cart);
    }

    public decimal GetTotalPrice()
    {
        decimal totalPrice = _cart.Sum(item => item.Product.Price * item.Quantity);
        return totalPrice * (1 - (decimal)DiscountPercentage);
    }

    public override string ToString()
    {
        var cartString = string.Join("\n", _cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name} ({GetType().Name})\nRabatt: {DiscountPercentage:P0}\nKundvagn:\n{cartString}\nTotalt (med rabatt): {GetTotalPrice():C}";
    }

    public string ToFileString()
    {
        return $"{GetType().Name},{Name},{Password}";
    }

    public static Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        switch (parts[0])
        {
            case "GoldCustomer":
                return new GoldCustomer(parts[1], parts[2]);
            case "SilverCustomer":
                return new SilverCustomer(parts[1], parts[2]);
            case "BronzeCustomer":
                return new BronzeCustomer(parts[1], parts[2]);
            default:
                throw new ArgumentException("Invalid customer type");
        }
    }
}

public class GoldCustomer : Customer
{
    public override double DiscountPercentage => 0.15;
    public GoldCustomer(string name, string password) : base(name, password) { }
}

public class SilverCustomer : Customer
{
    public override double DiscountPercentage => 0.10;
    public SilverCustomer(string name, string password) : base(name, password) { }
}

public class BronzeCustomer : Customer
{
    public override double DiscountPercentage => 0.05;
    public BronzeCustomer(string name, string password) : base(name, password) { }
}


class Program
{
    private static List<Customer> customers = new List<Customer>();
    private static List<Product> products = new List<Product>
    {
        new Product("Mat", 10m),
        new Product("Godis", 15m),
        new Product("Dricka", 5m)
    };

    private static Customer currentCustomer;
    private static string customersFile = "customers.txt";
    private static Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>
    {
        { "SEK", 1m },
        { "USD", 0.096m },
        { "EUR", 0.088m }
    };
    private static string currentCurrency = "SEK";

    static void Main(string[] args)
    {
        LoadCustomers();
        while (true)
        {
            if (currentCustomer == null)
            {
                ShowMainMenu();
            }
            else
            {
                ShowShoppingMenu();
            }
        }
    }

    static void ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("Välkommen till affären!");
        Console.WriteLine("1. Logga in");
        Console.WriteLine("2. Registrera ny kund");
        Console.WriteLine("3. Byt valuta");
        Console.WriteLine("4. Avsluta");

        switch (Console.ReadLine())
        {
            case "1":
                Login();
                break;
            case "2":
                RegisterNewCustomer();
                break;
            case "3":
                ChangeCurrency();
                break;
            case "4":
                SaveCustomers();
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Ogiltigt val. Försök igen.");
                break;
        }
    }

    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();

        currentCustomer = customers.FirstOrDefault(c => c.Name == name);

        if (currentCustomer == null)
        {
            Console.WriteLine("Kunden finns inte. Vill du registrera en ny kund? (j/n)");
            if (Console.ReadLine().ToLower() == "j")
            {
                RegisterNewCustomer();
            }
        }
        else if (!currentCustomer.VerifyPassword(password))
        {
            Console.WriteLine("Fel lösenord. Försök igen.");
            currentCustomer = null;
        }
        else
        {
            Console.WriteLine($"Välkommen, {currentCustomer.Name}!");
        }

        Console.ReadKey();
    }

    static void RegisterNewCustomer()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();

        if (customers.Any(c => c.Name == name))
        {
            Console.WriteLine("En kund med det namnet finns redan.");
        }
        else
        {
            Console.WriteLine("Välj kundnivå:");
            Console.WriteLine("1. Bronze");
            Console.WriteLine("2. Silver");
            Console.WriteLine("3. Gold");
            string choice = Console.ReadLine();

            Customer newCustomer;
            switch (choice)
            {
                case "1":
                    newCustomer = new BronzeCustomer(name, password);
                    break;
                case "2":
                    newCustomer = new SilverCustomer(name, password);
                    break;
                case "3":
                    newCustomer = new GoldCustomer(name, password);
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Sätter Bronze som standard.");
                    newCustomer = new BronzeCustomer(name, password);
                    break;
            }

            customers.Add(newCustomer);
            currentCustomer = newCustomer;
            Console.WriteLine("Ny kund registrerad och inloggad!");
            SaveCustomers();
        }

        Console.ReadKey();
    }

    static void ShowShoppingMenu()
    {
        Console.Clear();
        Console.WriteLine($"Inloggad som: {currentCustomer.Name}");
        Console.WriteLine("1. Handla");
        Console.WriteLine("2. Se kundvagn");
        Console.WriteLine("3. Gå till kassan");
        Console.WriteLine("4. Logga ut");

        switch (Console.ReadLine())
        {
            case "1":
                Shop();
                break;
            case "2":
                ViewCart();
                break;
            case "3":
                Checkout();
                break;
            case "4":
                currentCustomer = null;
                break;
            default:
                Console.WriteLine("Ogiltigt val. Försök igen.");
                break;
        }
    }

    static void Shop()
    {
        Console.Clear();
        Console.WriteLine("Tillgängliga produkter:");
        for (int i = 0; i < products.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {products[i].Name} - {ConvertCurrency(products[i].Price):F2} {currentCurrency}");
        }

        Console.Write("Välj produkt (nummer): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= products.Count)
        {
            Console.Write("Ange antal: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                currentCustomer.AddToCart(products[choice - 1], quantity);
                Console.WriteLine("Produkt tillagd i kundvagnen.");
            }
            else
            {
                Console.WriteLine("Ogiltigt antal.");
            }
        }
        else
        {
            Console.WriteLine("Ogiltigt val.");
        }

        Console.ReadKey();
    }

    static void ViewCart()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.ReadKey();
    }

    static void Checkout()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.WriteLine($"Totalt att betala: {ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currentCurrency}");
        Console.WriteLine("Tack för ditt köp!");
        currentCustomer = null;
        Console.ReadKey();
    }

    static void LoadCustomers()
    {
        if (File.Exists(customersFile))
        {
            customers = File.ReadAllLines(customersFile)
                .Select(line => Customer.FromFileString(line))
                .ToList();
        }
    }

    static void SaveCustomers()
    {
        File.WriteAllLines(customersFile, customers.Select(c => c.ToFileString()));
    }

    static void ChangeCurrency()
    {
        Console.WriteLine("Välj valuta:");
        foreach (var currency in exchangeRates.Keys)
        {
            Console.WriteLine(currency);
        }

        string newCurrency = Console.ReadLine().ToUpper();
        if (exchangeRates.ContainsKey(newCurrency))
        {
            currentCurrency = newCurrency;
            Console.WriteLine($"Valuta ändrad till {currentCurrency}");
        }
        else
        {
            Console.WriteLine("Ogiltig valuta.");
        }

        Console.ReadKey();
    }

    static decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[currentCurrency];
    }
}*/
/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Product
{
    public string Name { get; }
    public decimal Price { get; }
    public string Category { get; }

    public Product(string name, decimal price, string category)
    {
        Name = name;
        Price = price;
        Category = category;
    }
}

public abstract class Customer
{
    public string Name { get; }
    protected string Password { get; }
    protected List<(Product Product, int Quantity)> _cart;
    public abstract double DiscountPercentage { get; }

    public Customer(string name, string password)
    {
        Name = name;
        Password = password;
        _cart = new List<(Product, int)>();
    }

    public bool VerifyPassword(string password)
    {
        return Password == password;
    }

    public void AddToCart(Product product, int quantity)
    {
        var existingItem = _cart.Find(item => item.Product.Name == product.Name);
        if (existingItem.Product != null)
        {
            _cart.Remove(existingItem);
            _cart.Add((existingItem.Product, existingItem.Quantity + quantity));
        }
        else
        {
            _cart.Add((product, quantity));
        }
    }

    public List<(Product Product, int Quantity)> GetCart()
    {
        return new List<(Product, int)>(_cart);
    }

    public decimal GetTotalPrice()
    {
        decimal totalPrice = _cart.Sum(item => item.Product.Price * item.Quantity);
        return totalPrice * (1 - (decimal)DiscountPercentage);
    }

    public override string ToString()
    {
        var cartString = string.Join("\n", _cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name} ({GetType().Name})\nRabatt: {DiscountPercentage:P0}\nKundvagn:\n{cartString}\nTotalt (med rabatt): {GetTotalPrice():C}";
    }

    public string ToFileString()
    {
        return $"{GetType().Name},{Name},{Password}";
    }

    public static Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        switch (parts[0])
        {
            case "GoldCustomer":
                return new GoldCustomer(parts[1], parts[2]);
            case "SilverCustomer":
                return new SilverCustomer(parts[1], parts[2]);
            case "BronzeCustomer":
                return new BronzeCustomer(parts[1], parts[2]);
            default:
                throw new ArgumentException("Invalid customer type");
        }
    }
}

public class GoldCustomer : Customer
{
    public override double DiscountPercentage => 0.15;
    public GoldCustomer(string name, string password) : base(name, password) { }
}

public class SilverCustomer : Customer
{
    public override double DiscountPercentage => 0.10;
    public SilverCustomer(string name, string password) : base(name, password) { }
}

public class BronzeCustomer : Customer
{
    public override double DiscountPercentage => 0.05;
    public BronzeCustomer(string name, string password) : base(name, password) { }
}

class Program
{
    private static List<Customer> customers = new List<Customer>();
    private static List<Product> products = new List<Product>
    {
        new Product("Bröd", 25m, "Mat"),
        new Product("Pasta", 15m, "Mat"),
        new Product("Ris", 20m, "Mat"),
        new Product("Läsk", 15m, "Dricka"),
        new Product("Juice", 20m, "Dricka"),
        new Product("Vatten", 10m, "Dricka"),
        new Product("Choklad", 30m, "Godis"),
        new Product("Lösgodis", 25m, "Godis"),
        new Product("Chips", 35m, "Godis")
    };

    private static Customer currentCustomer;
    private static string customersFile = "customers.txt";
    private static Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>
    {
        { "SEK", 1m },
        { "USD", 0.096m },
        { "EUR", 0.088m }
    };
    private static string currentCurrency = "SEK";

    static void Main(string[] args)
    {
        LoadCustomers();
        while (true)
        {
            if (currentCustomer == null)
            {
                ShowMainMenu();
            }
            else
            {
                ShowShoppingMenu();
            }
        }
    }

    static void ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("Välkommen till affären!");
        Console.WriteLine("1. Logga in");
        Console.WriteLine("2. Registrera ny kund");
        Console.WriteLine("3. Byt valuta");
        Console.WriteLine("4. Avsluta");

        switch (Console.ReadLine())
        {
            case "1":
                Login();
                break;
            case "2":
                RegisterNewCustomer();
                break;
            case "3":
                ChangeCurrency();
                break;
            case "4":
                SaveCustomers();
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Ogiltigt val. Försök igen.");
                break;
        }
    }

    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();

        currentCustomer = customers.FirstOrDefault(c => c.Name == name);

        if (currentCustomer == null)
        {
            Console.WriteLine("Kunden finns inte. Vill du registrera en ny kund? (j/n)");
            if (Console.ReadLine().ToLower() == "j")
            {
                RegisterNewCustomer();
            }
        }
        else if (!currentCustomer.VerifyPassword(password))
        {
            Console.WriteLine("Fel lösenord. Försök igen.");
            currentCustomer = null;
        }
        else
        {
            Console.WriteLine($"Välkommen, {currentCustomer.Name}!");
        }

        Console.ReadKey();
    }

    static void RegisterNewCustomer()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();

        if (customers.Any(c => c.Name == name))
        {
            Console.WriteLine("En kund med det namnet finns redan.");
        }
        else
        {
            Console.WriteLine("Välj kundnivå:");
            Console.WriteLine("1. Bronze");
            Console.WriteLine("2. Silver");
            Console.WriteLine("3. Gold");
            string choice = Console.ReadLine();

            Customer newCustomer;
            switch (choice)
            {
                case "1":
                    newCustomer = new BronzeCustomer(name, password);
                    break;
                case "2":
                    newCustomer = new SilverCustomer(name, password);
                    break;
                case "3":
                    newCustomer = new GoldCustomer(name, password);
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Sätter Bronze som standard.");
                    newCustomer = new BronzeCustomer(name, password);
                    break;
            }

            customers.Add(newCustomer);
            currentCustomer = newCustomer;
            Console.WriteLine("Ny kund registrerad och inloggad!");
            SaveCustomers();
        }

        Console.ReadKey();
    }

    static void ShowShoppingMenu()
    {
        Console.Clear();
        Console.WriteLine($"Inloggad som: {currentCustomer.Name}");
        Console.WriteLine("1. Handla");
        Console.WriteLine("2. Se kundvagn");
        Console.WriteLine("3. Gå till kassan");
        Console.WriteLine("4. Logga ut");

        switch (Console.ReadLine())
        {
            case "1":
                Shop();
                break;
            case "2":
                ViewCart();
                break;
            case "3":
                Checkout();
                break;
            case "4":
                currentCustomer = null;
                break;
            default:
                Console.WriteLine("Ogiltigt val. Försök igen.");
                break;
        }
    }

    static void Shop()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Välj kategori:");
            Console.WriteLine("1. Mat");
            Console.WriteLine("2. Dricka");
            Console.WriteLine("3. Godis");
            Console.WriteLine("4. Återgå till huvudmenyn");

            string categoryChoice = Console.ReadLine();
            string selectedCategory;

            switch (categoryChoice)
            {
                case "1":
                    selectedCategory = "Mat";
                    break;
                case "2":
                    selectedCategory = "Dricka";
                    break;
                case "3":
                    selectedCategory = "Godis";
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    Console.ReadKey();
                    continue;
            }

            ShopInCategory(selectedCategory);
        }
    }

    static void ShopInCategory(string category)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Tillgängliga produkter i kategorin {category}:");
            var categoryProducts = products.Where(p => p.Category == category).ToList();

            for (int i = 0; i < categoryProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categoryProducts[i].Name} - {ConvertCurrency(categoryProducts[i].Price):F2} {currentCurrency}");
            }

            Console.WriteLine($"{categoryProducts.Count + 1}. Återgå till kategorimenyn");

            Console.Write("Välj produkt (nummer): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categoryProducts.Count + 1)
            {
                if (choice == categoryProducts.Count + 1)
                {
                    return;
                }

                Console.Write("Ange antal: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    currentCustomer.AddToCart(categoryProducts[choice - 1], quantity);
                    Console.WriteLine("Produkt tillagd i kundvagnen.");
                }
                else
                {
                    Console.WriteLine("Ogiltigt antal.");
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }

            Console.ReadKey();
        }
    }

    static void ViewCart()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.ReadKey();
    }

    static void Checkout()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.WriteLine($"Totalt att betala: {ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currentCurrency}");
        Console.WriteLine("Tack för ditt köp!");
        currentCustomer = null;
        Console.ReadKey();
    }

    static void LoadCustomers()
    {
        if (File.Exists(customersFile))
        {
            customers = File.ReadAllLines(customersFile)
                .Select(line => Customer.FromFileString(line))
                .ToList();
        }
    }

    static void SaveCustomers()
    {
        File.WriteAllLines(customersFile, customers.Select(c => c.ToFileString()));
    }

    static void ChangeCurrency()
    {
        Console.WriteLine("Välj valuta:");
        foreach (var currency in exchangeRates.Keys)
        {
            Console.WriteLine(currency);
        }

        string newCurrency = Console.ReadLine().ToUpper();
        if (exchangeRates.ContainsKey(newCurrency))
        {
            currentCurrency = newCurrency;
            Console.WriteLine($"Valuta ändrad till {currentCurrency}");
        }
        else
        {
            Console.WriteLine("Ogiltig valuta.");
        }

        Console.ReadKey();
    }

    static decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[currentCurrency];
    }
}


*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Product
{
    public string Name { get; }
    public decimal Price { get; }
    public string Category { get; }

    public Product(string name, decimal price, string category)
    {
        Name = name;
        Price = price;
        Category = category;
    }
}

public abstract class Customer
{
    public string Name { get; }
    protected string Password { get; }
    protected List<(Product Product, int Quantity)> Cart { get; } = new List<(Product, int)>();
    public abstract double DiscountPercentage { get; }

    protected Customer(string name, string password)
    {
        Name = name;
        Password = password;
    }

    public bool VerifyPassword(string password) => Password == password;

    public void AddToCart(Product product, int quantity)
    {
        var existingItem = Cart.Find(item => item.Product.Name == product.Name);
        if (existingItem.Product != null)
        {
            Cart.Remove(existingItem);
            Cart.Add((existingItem.Product, existingItem.Quantity + quantity));
        }
        else
        {
            Cart.Add((product, quantity));
        }
    }

    public decimal GetTotalPrice()
    {
        decimal totalPrice = Cart.Sum(item => item.Product.Price * item.Quantity);
        return totalPrice * (1 - (decimal)DiscountPercentage);
    }

    public override string ToString()
    {
        var cartString = string.Join("\n", Cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name} ({GetType().Name})\nRabatt: {DiscountPercentage:P0}\nKundvagn:\n{cartString}\nTotalt (med rabatt): {GetTotalPrice():C}";
    }

    public string ToFileString() => $"{GetType().Name},{Name},{Password}";

    public static Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        return parts[0] switch
        {
            "GoldCustomer" => new GoldCustomer(parts[1], parts[2]),
            "SilverCustomer" => new SilverCustomer(parts[1], parts[2]),
            "BronzeCustomer" => new BronzeCustomer(parts[1], parts[2]),
            _ => throw new ArgumentException("Invalid customer type")
        };
    }
}

public class GoldCustomer : Customer
{
    public override double DiscountPercentage => 0.15;
    public GoldCustomer(string name, string password) : base(name, password) { }
}

public class SilverCustomer : Customer
{
    public override double DiscountPercentage => 0.10;
    public SilverCustomer(string name, string password) : base(name, password) { }
}

public class BronzeCustomer : Customer
{
    public override double DiscountPercentage => 0.05;
    public BronzeCustomer(string name, string password) : base(name, password) { }
}

class Program
{
    private static List<Customer> customers = new List<Customer>();
    private static List<Product> products = new List<Product>
    {
        new Product("Bröd", 25m, "Mat"),
        new Product("Pasta", 15m, "Mat"),
        new Product("Ris", 20m, "Mat"),
        new Product("Läsk", 15m, "Dricka"),
        new Product("Juice", 20m, "Dricka"),
        new Product("Vatten", 10m, "Dricka"),
        new Product("Choklad", 30m, "Godis"),
        new Product("Lösgodis", 25m, "Godis"),
        new Product("Chips", 35m, "Godis")
    };
    private static Customer currentCustomer;
    private const string CustomersFile = "customers.txt";
    private static Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>
    {
        { "SEK", 1m },
        { "USD", 0.096m },
        { "EUR", 0.088m }
    };
    private static string currentCurrency = "SEK";

    static void Main()
    {
        LoadCustomers();
        while (true)
        {
            if (currentCustomer == null)
                ShowMainMenu();
            else
                ShowShoppingMenu();
        }
    }

    static void ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("Välkommen till affären!");
        Console.WriteLine("1. Logga in");
        Console.WriteLine("2. Registrera ny kund");
        Console.WriteLine("3. Byt valuta");
        Console.WriteLine("4. Avsluta");

        switch (Console.ReadLine())
        {
            case "1": Login(); break;
            case "2": RegisterNewCustomer(); break;
            case "3": ChangeCurrency(); break;
            case "4": SaveCustomers(); Environment.Exit(0); break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();

        currentCustomer = customers.FirstOrDefault(c => c.Name == name);

        if (currentCustomer == null)
        {
            Console.WriteLine("Kunden finns inte. Vill du registrera en ny kund? (ja/nej)");
            if (Console.ReadLine().ToLower() == "ja")
                RegisterNewCustomer();
        }
        else if (!currentCustomer.VerifyPassword(password))
        {
            Console.WriteLine("Fel lösenord. Försök igen.");
            currentCustomer = null;
        }
        else
            Console.WriteLine($"Välkommen, {currentCustomer.Name}!");

        Console.ReadKey();
    }

    static void RegisterNewCustomer()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();

        if (customers.Any(c => c.Name == name))
        {
            Console.WriteLine("En kund med det namnet finns redan.");
            return;
        }

        Console.WriteLine("Välj kundnivå: 1. Bronze, 2. Silver, 3. Gold");
        string choice = Console.ReadLine();

        Customer newCustomer = choice switch
        {
            "1" => new BronzeCustomer(name, password),
            "2" => new SilverCustomer(name, password),
            "3" => new GoldCustomer(name, password),
            _ => new BronzeCustomer(name, password)
        };

        customers.Add(newCustomer);
        currentCustomer = newCustomer;
        Console.WriteLine("Ny kund registrerad och inloggad!");
        SaveCustomers();

        Console.ReadKey();
    }

    static void ShowShoppingMenu()
    {
        Console.Clear();
        Console.WriteLine($"Inloggad som: {currentCustomer.Name}");
        Console.WriteLine("1. Handla");
        Console.WriteLine("2. Se kundvagn");
        Console.WriteLine("3. Gå till kassan");
        Console.WriteLine("4. Logga ut");

        switch (Console.ReadLine())
        {
            case "1": Shop(); break;
            case "2": ViewCart(); break;
            case "3": Checkout(); break;
            case "4": currentCustomer = null; break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    static void Shop()
    {
        while (true)
        {

            Console.WriteLine("Välj kategori");
            Console.WriteLine("1. Mat");
            Console.WriteLine("2. Dricka");
            Console.WriteLine("3. Godis");
            Console.WriteLine("4. Återgå till huvudmenyn");


            string categoryChoice = Console.ReadLine();
            if (categoryChoice == "4") return;

            string selectedCategory = categoryChoice switch
            {
                "1" => "Mat",
                "2" => "Dricka",
                "3" => "Godis",
                _ => ""
            };

            if (string.IsNullOrEmpty(selectedCategory))
            {
                Console.WriteLine("Ogiltigt val. Försök igen.");
                Console.ReadKey();
                continue;
            }

            ShopInCategory(selectedCategory);
        }
    }

    static void ShopInCategory(string category)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Tillgängliga produkter i kategorin {category}:");
            var categoryProducts = products.Where(p => p.Category == category).ToList();

            for (int i = 0; i < categoryProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categoryProducts[i].Name} - {ConvertCurrency(categoryProducts[i].Price):F2} {currentCurrency}");
            }

            Console.WriteLine($"{categoryProducts.Count + 1}. Återgå till kategorimenyn");
            Console.Write("Välj produkt (nummer): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categoryProducts.Count + 1)
            {
                if (choice == categoryProducts.Count + 1) return;

                Console.Write("Ange antal: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    currentCustomer.AddToCart(categoryProducts[choice - 1], quantity);
                    Console.WriteLine("Produkt tillagd i kundvagnen.");
                }
                else
                    Console.WriteLine("Ogiltigt antal.");
            }
            else
                Console.WriteLine("Ogiltigt val.");

            Console.ReadKey();
        }
    }

    static void ViewCart()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.ReadKey();
    }

    static void Checkout()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.WriteLine($"Totalt att betala: {ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currentCurrency}");
        Console.WriteLine("Tack för ditt köp!");
        currentCustomer = null;
        Console.ReadKey();
    }

    static void LoadCustomers()
    {
        if (File.Exists(CustomersFile))
        {
            customers = File.ReadAllLines(CustomersFile)
                .Select(Customer.FromFileString)
                .ToList();
        }
    }

    static void SaveCustomers()
    {
        File.WriteAllLines(CustomersFile, customers.Select(c => c.ToFileString()));
    }

    static void ChangeCurrency()
    {
        Console.WriteLine("Välj valuta:");
        foreach (var currency in exchangeRates.Keys)
        {
            Console.WriteLine(currency);
        }

        string newCurrency = Console.ReadLine().ToUpper();
        if (exchangeRates.ContainsKey(newCurrency))
        {
            currentCurrency = newCurrency;
            Console.WriteLine($"Valuta ändrad till {currentCurrency}");
        }
        else
            Console.WriteLine("Ogiltig valuta.");

        Console.ReadKey();
    }

    static decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[currentCurrency];
    }
}