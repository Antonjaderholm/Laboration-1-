// See https://aka.ms/new-console-template for more information
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// Produktklass som representerar en vara i butiken
public class Product
{
    public string Name { get; }
    public decimal Price { get; }
    public string Category { get; }

    // En abstrakt basklass för alla kundtyper
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
    protected string PasswordHash { get; }
    protected List<(Product Product, int Quantity)> Cart { get; } = new List<(Product, int)>();
    public abstract double DiscountPercentage { get; }

    protected Customer(string name, string passwordHash)
    {
        Name = name;
        PasswordHash = passwordHash;
    }

    // Verifierar kundens lösenord
    public bool VerifyPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

    // Gör så man lägger till en produkt i kundvagnen eller uppdaterar kvantiteten om produkten redan finns med. 
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

    //Beräknar totalpriset för kundvagnen med de beröda rabatterna.
    public decimal GetTotalPrice()
    {
        decimal totalPrice = Cart.Sum(item => item.Product.Price * item.Quantity);
        return totalPrice * (1 - (decimal)DiscountPercentage);
    }

    // Nedan skapar jag en strängrepresentation av kundens kundvagn.
    public override string ToString()
    {
        var cartString = string.Join("\n", Cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name} ({GetType().Name})\nRabatt: {DiscountPercentage:P0}\nKundvagn:\n{cartString}\nTotalt (med rabatt): {GetTotalPrice():C}";
    }

    // Här skapar jag en strängrepresentation av kunden för fillagring.
    public string ToFileString() => $"{GetType().Name},{Name},{PasswordHash}";

    // Och här skapar jag en kundinstans från en filsträng 
    public static Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        return parts[0] switch
        {
            nameof(GoldCustomer) => new GoldCustomer(parts[1], parts[2]),
            nameof(SilverCustomer) => new SilverCustomer(parts[1], parts[2]),
            nameof(BronzeCustomer) => new BronzeCustomer(parts[1], parts[2]),
            _ => throw new ArgumentException("Invalid customer type")
        };
    }

    private static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

public class GoldCustomer : Customer
{
    public override double DiscountPercentage => 0.15;
    public GoldCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

public class SilverCustomer : Customer
{
    public override double DiscountPercentage => 0.10;
    public SilverCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

public class BronzeCustomer : Customer
{
    public override double DiscountPercentage => 0.05;
    public BronzeCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

public class CustomerService
{
    private List<Customer> customers;
    private const string CustomersFile = "customers.txt";

    public CustomerService()
    {
        LoadCustomers();
    }

    public void LoadCustomers()
    {
        try
        {
            if (File.Exists(CustomersFile))
            {
                customers = File.ReadAllLines(CustomersFile)
                    .Select(Customer.FromFileString)
                    .ToList();
            }
            else
            {
                customers = new List<Customer>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading customers: {ex.Message}");
            customers = new List<Customer>();
        }
    }

    public void SaveCustomers()
    {
        try
        {
            File.WriteAllLines(CustomersFile, customers.Select(c => c.ToFileString()));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving customers: {ex.Message}");
        }
    }

    public Customer GetCustomerByName(string name)
    {
        return customers.FirstOrDefault(c => c.Name == name);
    }

    public void AddCustomer(Customer customer)
    {
        if (customers.Any(c => c.Name == customer.Name))
        {
            throw new ArgumentException("A customer with this name already exists.");
        }
        customers.Add(customer);
        SaveCustomers();
    }

    public List<Customer> GetAllCustomers()
    {
        return customers;
    }
}

public class ProductService
{
    private List<Product> products;

    public ProductService()
    {
        InitializeProducts();
    }

    private void InitializeProducts()
    {
        products = new List<Product>
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
    }

    public List<Product> GetProductsByCategory(string category)
    {
        return products.Where(p => p.Category == category).ToList();
    }

    public List<string> GetCategories()
    {
        return products.Select(p => p.Category).Distinct().ToList();
    }
}

public class CurrencyService
{
    private Dictionary<string, decimal> exchangeRates;
    public string CurrentCurrency { get; private set; }

    public CurrencyService()
    {
        exchangeRates = new Dictionary<string, decimal>
        {
            { "SEK", 1m },
            { "USD", 0.096m },
            { "EUR", 0.088m }
        };
        CurrentCurrency = "SEK";
    }

    public bool ChangeCurrency(string newCurrency)
    {
        if (exchangeRates.ContainsKey(newCurrency))
        {
            CurrentCurrency = newCurrency;
            return true;
        }
        return false;
    }

    public decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[CurrentCurrency];
    }

    public List<string> GetAvailableCurrencies()
    {
        return exchangeRates.Keys.ToList();
    }
}

class Program
{
    private static CustomerService customerService;
    private static ProductService productService;
    private static CurrencyService currencyService;
    private static Customer currentCustomer;

    static void Main()
    {
        customerService = new CustomerService();
        productService = new ProductService();
        currencyService = new CurrencyService();

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
            case "4": Environment.Exit(0); break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();

        currentCustomer = customerService.GetCustomerByName(name);

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

        try
        {
            Console.WriteLine("Välj kundnivå: 1. Bronze, 2. Silver, 3. Gold");
            string choice = Console.ReadLine();

            Customer newCustomer = choice switch
            {
                "1" => new BronzeCustomer(name, HashPassword(password)),
                "2" => new SilverCustomer(name, HashPassword(password)),
                "3" => new GoldCustomer(name, HashPassword(password)),
                _ => new BronzeCustomer(name, HashPassword(password))
            };

            customerService.AddCustomer(newCustomer);
            currentCustomer = newCustomer;
            Console.WriteLine("Ny kund registrerad och inloggad!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
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
            Console.Clear();
            Console.WriteLine("Välj kategori");
            var categories = productService.GetCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            Console.WriteLine($"{categories.Count + 1}. Återgå till huvudmenyn");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categories.Count + 1)
            {
                if (choice == categories.Count + 1) return;
                ShopInCategory(categories[choice - 1]);
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Försök igen.");
                Console.ReadKey();
            }
        }
    }

    static void ShopInCategory(string category)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Tillgängliga produkter i kategorin {category}:");
            var categoryProducts = productService.GetProductsByCategory(category);

            for (int i = 0; i < categoryProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categoryProducts[i].Name} - {currencyService.ConvertCurrency(categoryProducts[i].Price):F2} {currencyService.CurrentCurrency}");
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
        Console.WriteLine($"Totalt att betala: {currencyService.ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currencyService.CurrentCurrency}");
        Console.WriteLine("Tack för ditt köp!");
        currentCustomer = null;
        Console.ReadKey();
    }

    static void ChangeCurrency()
    {
        Console.WriteLine("Välj valuta:");
        var currencies = currencyService.GetAvailableCurrencies();
        foreach (var currency in currencies)
        {
            Console.WriteLine(currency);
        }

        string newCurrency = Console.ReadLine().ToUpper();
        if (currencyService.ChangeCurrency(newCurrency))
        {
            Console.WriteLine($"Valuta ändrad till {currencyService.CurrentCurrency}");
        }
        else
            Console.WriteLine("Ogiltig valuta.");

        Console.ReadKey();
    }

    private static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}*/


/*using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// Produktklass som representerar en vara i butiken
// Vi använder en separat klass för produkter för att enkapsulera all produktrelaterad information
// Detta gör det lättare att hantera och utöka produktfunktionalitet i framtiden
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

// En abstrakt basklass för alla kundtyper
// Vi använder en abstrakt klass här för att definiera gemensam funktionalitet för alla kundtyper
// Detta främjar kodåteranvändning och gör det enkelt att lägga till nya kundtyper i framtiden
public abstract class Customer
{
    public string Name { get; }
    protected string PasswordHash { get; }
    protected List<(Product Product, int Quantity)> Cart { get; } = new List<(Product, int)>();

    // Abstrakt egenskap som tvingar subklasser att implementera sin egen rabattsats
    public abstract double DiscountPercentage { get; }

    protected Customer(string name, string passwordHash)
    {
        Name = name;
        PasswordHash = passwordHash;
    }


    // Verifierar kundens lösenord
    // Vi använder en hashad version av lösenordet för ökad säkerhet
    public bool VerifyPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

    // Lägger till en produkt i kundvagnen eller uppdaterar kvantiteten om produkten redan finns
    // Denna metod hanterar både tillägg av nya produkter och uppdatering av befintliga
    // för att undvika duplicering av kod och göra kundvagnshanteringen mer robust
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

    // Beräknar totalpriset för kundvagnen med tillämpade rabatter
    // Denna metod kapslar in logiken för prisberäkning, vilket gör det enkelt att ändra
    // eller utöka prisberäkningslogiken i framtiden
    public decimal GetTotalPrice()
    {
        decimal totalPrice = Cart.Sum(item => item.Product.Price * item.Quantity);
        return totalPrice * (1 - (decimal)DiscountPercentage);
    }

    // Skapar en strängrepresentation av kundens kundvagn
    // Detta är användbart för att visa kundvagnsinformation för användaren
    public override string ToString()
    {
        var cartString = string.Join("\n", Cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name} ({GetType().Name})\nRabatt: {DiscountPercentage:P0}\nKundvagn:\n{cartString}\nTotalt (med rabatt): {GetTotalPrice():C}";
    }

    // Skapar en strängrepresentation av kunden för fillagring
    // Detta används för att spara kunddata i en enkel textbaserad format
    public string ToFileString() => $"{GetType().Name},{Name},{PasswordHash}";

    // Skapar en kundinstans från en filsträng
    // Denna statiska metod gör det möjligt att återskapa kundobjekt från sparad data
    public static Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        return parts[0] switch
        {
            nameof(GoldCustomer) => new GoldCustomer(parts[1], parts[2]),
            nameof(SilverCustomer) => new SilverCustomer(parts[1], parts[2]),
            nameof(BronzeCustomer) => new BronzeCustomer(parts[1], parts[2]),
            _ => throw new ArgumentException("Invalid customer type")
        };
    }

    // Hashar lösenordet för säker lagring
    // Vi använder SHA256 för att skapa en säker hash av lösenordet
    // Detta ökar säkerheten genom att undvika lagring av lösenord i klartext
    private static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

// Kundklasser med specifika rabattsatser
// Genom att använda arv kan vi enkelt definiera olika kundtyper med olika rabattsatser
public class GoldCustomer : Customer
{
    public override double DiscountPercentage => 0.15;
    public GoldCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

public class SilverCustomer : Customer
{
    public override double DiscountPercentage => 0.10;
    public SilverCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

public class BronzeCustomer : Customer
{
    public override double DiscountPercentage => 0.05;
    public BronzeCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

// Service för att hantera kunder
// Denna klass kapslar in all logik relaterad till kundhantering, inklusive lagring och hämtning
public class CustomerService
{
    private List<Customer> customers;
    private const string CustomersFile = "customers.txt";

    public CustomerService()
    {
        LoadCustomers();
    }

    // Laddar kunder från fil
    // Detta gör det möjligt att behålla kunddata mellan programkörningar
    public void LoadCustomers()
    {
        try
        {
            if (File.Exists(CustomersFile))
            {
                customers = File.ReadAllLines(CustomersFile)
                    .Select(Customer.FromFileString)
                    .ToList();
            }
            else
            {
                customers = new List<Customer>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading customers: {ex.Message}");
            customers = new List<Customer>();
        }
    }

    // Sparar kunder till fil
    // Detta säkerställer att kunddata bevaras mellan programkörningar
    public void SaveCustomers()
    {
        try
        {
            File.WriteAllLines(CustomersFile, customers.Select(c => c.ToFileString()));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving customers: {ex.Message}");
        }
    }

    // Hämtar en kund baserat på namn
    // Detta möjliggör snabb sökning efter en specifik kund
    public Customer GetCustomerByName(string name)
    {
        return customers.FirstOrDefault(c => c.Name == name);
    }

    // Lägger till en ny kund
    // Vi kontrollerar om kunden redan existerar för att undvika dubbletter
    public void AddCustomer(Customer customer)
    {
        if (customers.Any(c => c.Name == customer.Name))
        {
            throw new ArgumentException("A customer with this name already exists.");
        }
        customers.Add(customer);
        SaveCustomers();
    }

    // Hämtar alla kunder
    // Detta kan vara användbart för administrativa ändamål
    public List<Customer> GetAllCustomers()
    {
        return customers;
    }
}

// Service för att hantera produkter
// Denna klass kapslar in all logik relaterad till produkthantering
public class ProductService
{
    private List<Product> products;

    public ProductService()
    {
        InitializeProducts();
    }

    // Initierar en lista med produkter
    // I en verklig applikation skulle detta troligtvis hämtas från en databas
    private void InitializeProducts()
    {
        products = new List<Product>
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
    }

    // Hämtar produkter efter kategori
    // Detta möjliggör enkel filtrering av produkter för användaren
    public List<Product> GetProductsByCategory(string category)
    {
        return products.Where(p => p.Category == category).ToList();
    }

    // Hämtar alla unika kategorier
    // Detta används för att visa tillgängliga kategorier för användaren
    public List<string> GetCategories()
    {
        return products.Select(p => p.Category).Distinct().ToList();
    }
}

// Service för att hantera valutor
// Denna klass hanterar valutakonvertering och tillgängliga valutor
public class CurrencyService
{
    private Dictionary<string, decimal> exchangeRates;
    public string CurrentCurrency { get; private set; }

    public CurrencyService()
    {
        exchangeRates = new Dictionary<string, decimal>
        {
            { "SEK", 1m },
            { "USD", 0.096m },
            { "EUR", 0.088m }
        };
        CurrentCurrency = "SEK";
    }

    // Byter valuta
    // Returnerar true om bytet lyckades, false annars
    public bool ChangeCurrency(string newCurrency)
    {
        if (exchangeRates.ContainsKey(newCurrency))
        {
            CurrentCurrency = newCurrency;
            return true;
        }
        return false;
    }

    // Konverterar belopp till aktuell valuta
    // Detta gör det möjligt att visa priser i olika valutor
    public decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[CurrentCurrency];
    }

    // Hämtar tillgängliga valutor
    // Detta används för att visa tillgängliga valutaalternativ för användaren
    public List<string> GetAvailableCurrencies()
    {
        return exchangeRates.Keys.ToList();
    }
}

class Program
{
    private static CustomerService customerService;
    private static ProductService productService;
    private static CurrencyService currencyService;
    private static Customer currentCustomer;

    static void Main()
    {
        customerService = new CustomerService();
        productService = new ProductService();
        currencyService = new CurrencyService();

        // Huvudprogramloop
        // Denna loop håller programmet igång tills användaren väljer att avsluta
        while (true)
        {
            if (currentCustomer == null)
                ShowMainMenu();
            else
                ShowShoppingMenu();
        }
    }

    // Visar huvudmenyn
    // Denna metod hanterar användarinteraktionen i huvudmenyn
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
            case "4": Environment.Exit(0); break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    // Hanterar inloggning
    // Denna metod verifierar användarens inloggningsuppgifter
    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();
        currentCustomer = customerService.GetCustomerByName(name);
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

    // Registrerar en ny kund
    // Denna metod hanterar processen för att skapa en ny kundprofil
    static void RegisterNewCustomer()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();
        try
        {
            Console.WriteLine("Välj kundnivå: 1. Bronze, 2. Silver, 3. Gold");
            string choice = Console.ReadLine();
            Customer newCustomer = choice switch
            {
                "1" => new BronzeCustomer(name, HashPassword(password)),
                "2" => new SilverCustomer(name, HashPassword(password)),
                "3" => new GoldCustomer(name, HashPassword(password)),
                _ => new BronzeCustomer(name, HashPassword(password))
            };
            customerService.AddCustomer(newCustomer);
            currentCustomer = newCustomer;
            Console.WriteLine("Ny kund registrerad och inloggad!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.ReadKey();
    }

    // Visar shoppingmenyn
    // Denna metod hanterar användarinteraktionen i shoppingmenyn
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

        // Hanterar shoppingprocessen
        // Denna metod låter användaren välja produktkategori och produkter att lägga till i kundvagnen
        static void Shop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Välj kategori");
                var categories = productService.GetCategories();
                for (int i = 0; i < categories.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {categories[i]}");
                }
                Console.WriteLine($"{categories.Count + 1}. Återgå till huvudmenyn");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categories.Count + 1)
                {
                    if (choice == categories.Count + 1) return;
                    ShopInCategory(categories[choice - 1]);
                }
                else
                {
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    Console.ReadKey();
                }
            }
        }

        // Hanterar shopping inom en specifik kategori
        // Denna metod visar produkter inom en vald kategori och låter användaren lägga till dem i kundvagnen
        static void ShopInCategory(string category)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Tillgängliga produkter i kategorin {category}:");
                var categoryProducts = productService.GetProductsByCategory(category);
                for (int i = 0; i < categoryProducts.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {categoryProducts[i].Name} - {currencyService.ConvertCurrency(categoryProducts[i].Price):F2} {currencyService.CurrentCurrency}");
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

        // Visar kundvagnen
        // Denna metod visar innehållet i den aktuella kundens kundvagn
        static void ViewCart()
        {
            Console.Clear();
            Console.WriteLine(currentCustomer.ToString());
            Console.ReadKey();
        }

        // Hanterar utcheckning
        // Denna metod visar den slutliga summan och avslutar köpet
        static void Checkout()
        {
            Console.Clear();
            Console.WriteLine(currentCustomer.ToString());
            Console.WriteLine($"Totalt att betala: {currencyService.ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currencyService.CurrentCurrency}");
            Console.WriteLine("Tack för ditt köp!");
            currentCustomer = null;
            Console.ReadKey();
        }

        // Hanterar valutabyte
        // Denna metod låter användaren byta den aktuella valutan
        static void ChangeCurrency()
        {
            Console.WriteLine("Välj valuta:");
            var currencies = currencyService.GetAvailableCurrencies();
            foreach (var currency in currencies)
            {
                Console.WriteLine(currency);
            }
            string newCurrency = Console.ReadLine().ToUpper();
            if (currencyService.ChangeCurrency(newCurrency))
            {
                Console.WriteLine($"Valuta ändrad till {currencyService.CurrentCurrency}");
            }
            else
                Console.WriteLine("Ogiltig valuta.");
            Console.ReadKey();
        }

        // Hashar lösenordet för säker lagring
        // Denna metod används för att skapa en säker hash av användarens lösenord
        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }*/
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net.Sockets;

// Produktklass som representerar en vara i butiken
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

// En abstrakt basklass för alla kundtyper
public abstract class Customer
{
    public string Name { get; }
    public string PasswordHash { get; protected set; }
    protected string OriginalPassword { get; set; }
    protected List<(Product Product, int Quantity)> Cart { get; } = new List<(Product, int)>();

    public abstract double DiscountPercentage { get; }

    protected Customer(string name, string passwordHash)
    {
        Name = name;
        PasswordHash = passwordHash;
    }

    // Verifierar kundens lösenord
    public bool VerifyPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

    // Lägger till en produkt i kundvagnen eller uppdaterar kvantiteten om produkten redan finns
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

    // Beräknar totalpriset för kundvagnen med tillämpade rabatter
    public decimal GetTotalPrice()
    {
        decimal totalPrice = Cart.Sum(item => item.Product.Price * item.Quantity);
        return totalPrice * (1 - (decimal)DiscountPercentage);
    }

    // Skapar en strängrepresentation av kundens kundvagn
    public override string ToString()
    {
        var cartString = string.Join("\n", Cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name} ({GetType().Name})\nRabatt: {DiscountPercentage:P0}\nKundvagn:\n{cartString}\nTotalt (med rabatt): {GetTotalPrice():C}";
    }

    // Skapar en strängrepresentation av kunden för fillagring
    public string ToFileString() => $"{GetType().Name},{Name},{PasswordHash}";

    // Skapar en kundinstans från en filsträng
    public static Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        return parts[0] switch
        {
            nameof(GoldCustomer) => new GoldCustomer(parts[1], parts[2]),
            nameof(SilverCustomer) => new SilverCustomer(parts[1], parts[2]),
            nameof(BronzeCustomer) => new BronzeCustomer(parts[1], parts[2]),
            _ => throw new ArgumentException("Invalid customer type")
        };
    }

    // Hashar lösenordet för säker lagring
    private static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Ny metod för att hämta kundinformation
    public CustomerInformation GetCustomerInformation()
    {
        return new CustomerInformation(this);
    }
}

// Kundklasser med specifika rabattsatser
public class GoldCustomer : Customer
{
    public override double DiscountPercentage => 0.15;
    public GoldCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

public class SilverCustomer : Customer
{
    public override double DiscountPercentage => 0.10;
    public SilverCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

public class BronzeCustomer : Customer
{
    public override double DiscountPercentage => 0.05;
    public BronzeCustomer(string name, string passwordHash) : base(name, passwordHash) { }
}

// Ny klass för kundinformation
public class CustomerInformation
{
    private Customer customer;

    public CustomerInformation(Customer customer)
    {
        this.customer = customer;
    }

    public override string ToString()
    {
        string customerType = customer switch
        {
            BronzeCustomer => "Bronze",
            SilverCustomer => "Silver",
            GoldCustomer => "Gold",
            _ => "Unknown"
        };
        return $"Namn: {customer.Name}\nLösenord: {customer.PasswordHash}\nKundtyp: {customerType}";
    }
}

// Service för att hantera kunder
public class CustomerService
{
    private List<Customer> customers;
    private const string CustomersFile = "customers.txt";

    public CustomerService()
    {
        LoadCustomers();
    }

    // Laddar kunder från fil
    public void LoadCustomers()
    {
        try
        {
            if (File.Exists(CustomersFile))
            {
                customers = File.ReadAllLines(CustomersFile)
                    .Select(Customer.FromFileString)
                    .ToList();
            }
            else
            {
                customers = new List<Customer>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading customers: {ex.Message}");
            customers = new List<Customer>();
        }
    }

    // Sparar kunder till fil
    public void SaveCustomers()
    {
        try
        {
            File.WriteAllLines(CustomersFile, customers.Select(c => c.ToFileString()));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving customers: {ex.Message}");
        }
    }

    // Hämtar en kund baserat på namn
    public Customer GetCustomerByName(string name)
    {
        return customers.FirstOrDefault(c => c.Name == name);
    }

    // Lägger till en ny kund
    public void AddCustomer(Customer customer)
    {
        if (customers.Any(c => c.Name == customer.Name))
        {
            throw new ArgumentException("A customer with this name already exists.");
        }
        customers.Add(customer);
        SaveCustomers();
    }

    // Hämtar alla kunder
    public List<Customer> GetAllCustomers()
    {
        return customers;
    }
}

// Service för att hantera produkter
public class ProductService
{
    private List<Product> products;

    public ProductService()
    {
        InitializeProducts();
    }

    // Initierar en lista med produkter
    private void InitializeProducts()
    {
        products = new List<Product>
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
    }

    // Hämtar produkter efter kategori
    public List<Product> GetProductsByCategory(string category)
    {
        return products.Where(p => p.Category == category).ToList();
    }

    // Hämtar alla unika kategorier
    public List<string> GetCategories()
    {
        return products.Select(p => p.Category).Distinct().ToList();
    }
}

// Service för att hantera valutor
public class CurrencyService
{
    private Dictionary<string, decimal> exchangeRates;
    public string CurrentCurrency { get; private set; }

    public CurrencyService()
    {
        exchangeRates = new Dictionary<string, decimal>
        {
            { "SEK", 1m },
            { "USD", 0.096m },
            { "EUR", 0.088m }
        };
        CurrentCurrency = "SEK";
    }

    // Byter valuta
    public bool ChangeCurrency(string newCurrency)
    {
        if (exchangeRates.ContainsKey(newCurrency))
        {
            CurrentCurrency = newCurrency;
            return true;
        }
        return false;
    }

    // Konverterar belopp till aktuell valuta
    public decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[CurrentCurrency];
    }

    // Hämtar tillgängliga valutor
    public List<string> GetAvailableCurrencies()
    {
        return exchangeRates.Keys.ToList();
    }
}

class Program
{
    private static CustomerService customerService;
    private static ProductService productService;
    private static CurrencyService currencyService;
    private static Customer currentCustomer;

    static void Main()
    {
        customerService = new CustomerService();
        productService = new ProductService();
        currencyService = new CurrencyService();

        // Huvudprogramloop
        while (true)
        {
            if (currentCustomer == null)
                ShowMainMenu();
            else
                ShowShoppingMenu();
        }
    }

    // Visar huvudmenyn
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
            case "4": Environment.Exit(0); break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    // Hanterar inloggning
    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();
        currentCustomer = customerService.GetCustomerByName(name);
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

    // Registrerar en ny kund
    static void RegisterNewCustomer()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();
        try
        {
            Console.WriteLine("Välj kundnivå: 1. Bronze, 2. Silver, 3. Gold");
            string choice = Console.ReadLine();
            Customer newCustomer = choice switch
            {
                "1" => new BronzeCustomer(name, HashPassword(password)),
                "2" => new SilverCustomer(name, HashPassword(password)),
                "3" => new GoldCustomer(name, HashPassword(password)),
                _ => new BronzeCustomer(name, HashPassword(password))
            };
            customerService.AddCustomer(newCustomer);
            currentCustomer = newCustomer;
            Console.WriteLine("Ny kund registrerad och inloggad!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.ReadKey();
    }

    // Visar shoppingmenyn
    static void ShowShoppingMenu()
    {
        Console.Clear();
        Console.WriteLine($"Inloggad som: {currentCustomer.Name}");
        Console.WriteLine("1. Handla");
        Console.WriteLine("2. Se kundvagn");
        Console.WriteLine("3. Gå till kassan");
        Console.WriteLine("4. Visa kundinformation");
        Console.WriteLine("5. Logga ut");
        switch (Console.ReadLine())
        {
            case "1": Shop(); break;
            case "2": ViewCart(); break;
            case "3": Checkout(); break;
            case "4": ShowCustomerInformation(); break;
            case "5": currentCustomer = null; break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    // Hanterar shoppingprocessen
    static void Shop()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Välj kategori");
            var categories = productService.GetCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            Console.WriteLine($"{categories.Count + 1}. Återgå till huvudmenyn");


            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categories.Count + 1)
                {
                    if (choice == categories.Count + 1) return;
                    ShopInCategory(categories[choice - 1]);
                }
                else
                {
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    Console.ReadKey();
                }
        }
    }

    // Hanterar shopping inom en specifik kategori
    static void ShopInCategory(string category)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Tillgängliga produkter i kategorin {category}:");
            var categoryProducts = productService.GetProductsByCategory(category);
            for (int i = 0; i < categoryProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categoryProducts[i].Name} - {currencyService.ConvertCurrency(categoryProducts[i].Price):F2} {currencyService.CurrentCurrency}");
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

    // Visar kundvagnen
    static void ViewCart()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.ReadKey();
    }

    // Hanterar utcheckning
    static void Checkout()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.WriteLine($"Totalt att betala: {currencyService.ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currencyService.CurrentCurrency}");
        Console.WriteLine("Tack för ditt köp!");
        currentCustomer = null;
        Console.ReadKey();
    }

    // Hanterar valutabyte
    static void ChangeCurrency()
    {
        Console.WriteLine("Välj valuta:");
        var currencies = currencyService.GetAvailableCurrencies();
        foreach (var currency in currencies)
        {
            Console.WriteLine(currency);
        }
        string newCurrency = Console.ReadLine().ToUpper();
        if (currencyService.ChangeCurrency(newCurrency))
        {
            Console.WriteLine($"Valuta ändrad till {currencyService.CurrentCurrency}");
        }
        else
            Console.WriteLine("Ogiltig valuta.");
        Console.ReadKey();
    }

    // Visar kundinformation
    static void ShowCustomerInformation()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.GetCustomerInformation().ToString());
        Console.ReadKey();
    }

    // Hashar lösenordet för säker lagring
    private static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}*/

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
    public string PasswordHash { get; protected set; }
    protected string OriginalPassword { get; set; }
    protected List<(Product Product, int Quantity)> Cart { get; } = new List<(Product, int)>();

    public abstract double DiscountPercentage { get; }

    protected Customer(string name, string password)
    {
        Name = name;
        OriginalPassword = password;
        PasswordHash = HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

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

    public string GetOriginalPassword()
    {
        return OriginalPassword;
    }

    private static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public CustomerInformation GetCustomerInformation()
    {
        return new CustomerInformation(this);
    }

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
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

public class CustomerInformation
{
    private Customer customer;

    public CustomerInformation(Customer customer)
    {
        this.customer = customer;
    }

    public override string ToString()
    {
        string customerType = customer switch
        {
            BronzeCustomer => "Bronze",
            SilverCustomer => "Silver",
            GoldCustomer => "Gold",
            _ => "Unknown"
        };
        return $"Namn: {customer.Name}\nLösenord: {customer.GetOriginalPassword()}\nKundtyp: {customerType}";
    }
}

public class CustomerService
{
    private List<Customer> customers;
    private const string CustomersFile = "customers.txt";

    public CustomerService()
    {
        LoadCustomers();
    }

    public void LoadCustomers()
    {
        try
        {
            if (File.Exists(CustomersFile))
            {
                customers = File.ReadAllLines(CustomersFile)
                    .Select(FromFileString)
                    .ToList();
            }
            else
            {
                customers = new List<Customer>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading customers: {ex.Message}");
            customers = new List<Customer>();
        }
    }

    public void SaveCustomers()
    {
        try
        {
            File.WriteAllLines(CustomersFile, customers.Select(ToFileString));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving customers: {ex.Message}");
        }
    }

    public Customer GetCustomerByName(string name)
    {
        return customers.FirstOrDefault(c => c.Name == name);
    }

    public void AddCustomer(Customer customer)
    {
        if (customers.Any(c => c.Name == customer.Name))
        {
            throw new ArgumentException("A customer with this name already exists.");
        }
        customers.Add(customer);
        SaveCustomers();
    }

    public List<Customer> GetAllCustomers()
    {
        return customers;
    }

    public string ToFileString(Customer customer) => $"{customer.GetType().Name},{customer.Name},{customer.PasswordHash},{customer.GetOriginalPassword()}";

    public Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        Customer customer = parts[0] switch
        {
            nameof(GoldCustomer) => new GoldCustomer(parts[1], parts[3]),
            nameof(SilverCustomer) => new SilverCustomer(parts[1], parts[3]),
            nameof(BronzeCustomer) => new BronzeCustomer(parts[1], parts[3]),
            _ => throw new ArgumentException("Invalid customer type")
        };
        customer.SetPasswordHash(parts[2]);
        return customer;
    }
}

public class ProductService
{
    private List<Product> products;

    public ProductService()
    {
        InitializeProducts();
    }

    private void InitializeProducts()
    {
        products = new List<Product>
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
    }

    public List<Product> GetProductsByCategory(string category)
    {
        return products.Where(p => p.Category == category).ToList();
    }

    public List<string> GetCategories()
    {
        return products.Select(p => p.Category).Distinct().ToList();
    }
}

public class CurrencyService
{
    private Dictionary<string, decimal> exchangeRates;
    public string CurrentCurrency { get; private set; }

    public CurrencyService()
    {
        exchangeRates = new Dictionary<string, decimal>
        {
            { "SEK", 1m },
            { "USD", 0.096m },
            { "EUR", 0.088m }
        };
        CurrentCurrency = "SEK";
    }

    public bool ChangeCurrency(string newCurrency)
    {
        if (exchangeRates.ContainsKey(newCurrency))
        {
            CurrentCurrency = newCurrency;
            return true;
        }
        return false;
    }

    public decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[CurrentCurrency];
    }

    public List<string> GetAvailableCurrencies()
    {
        return exchangeRates.Keys.ToList();
    }
}

class Program
{
    private static CustomerService customerService;
    private static ProductService productService;
    private static CurrencyService currencyService;
    private static Customer currentCustomer;

    static void Main()
    {
        customerService = new CustomerService();
        productService = new ProductService();
        currencyService = new CurrencyService();

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
            case "4": Environment.Exit(0); break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();
        currentCustomer = customerService.GetCustomerByName(name);
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
        try
        {
            Console.WriteLine("Välj kundnivå: 1. Bronze, 2. Silver, 3. Gold");
            string choice = Console.ReadLine();
            Customer newCustomer = choice switch
            {
                "1" => new BronzeCustomer(name, password),
                "2" => new SilverCustomer(name, password),
                "3" => new GoldCustomer(name, password),
                _ => new BronzeCustomer(name, password)
            };
            customerService.AddCustomer(newCustomer);
            currentCustomer = newCustomer;
            Console.WriteLine("Ny kund registrerad och inloggad!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
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
        Console.WriteLine("4. Visa kundinformation");
        Console.WriteLine("5. Logga ut");
        switch (Console.ReadLine())
        {
            case "1": Shop(); break;
            case "2": ViewCart(); break;
            case "3": Checkout(); break;
            case "4": ShowCustomerInformation(); break;
            case "5": currentCustomer = null; break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    static void Shop()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Välj kategori");
            var categories = productService.GetCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            Console.WriteLine($"{categories.Count + 1}. Återgå till huvudmenyn");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categories.Count + 1)
            {
                if (choice == categories.Count + 1) return;
                ShopInCategory(categories[choice - 1]);
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Försök igen.");
                Console.ReadKey();
            }
        }
    }

    static void ShopInCategory(string category)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Tillgängliga produkter i kategorin {category}:");
            var categoryProducts = productService.GetProductsByCategory(category);
            for (int i = 0; i < categoryProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categoryProducts[i].Name} - {currencyService.ConvertCurrency(categoryProducts[i].Price):F2} {currencyService.CurrentCurrency}");
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
            Console.WriteLine($"Totalt att betala: {currencyService.ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currencyService.CurrentCurrency}");
            Console.WriteLine("Tack för ditt köp!");
            currentCustomer = null;
            Console.ReadKey();
        }

        static void ChangeCurrency()
        {
            Console.WriteLine("Välj valuta:");
            var currencies = currencyService.GetAvailableCurrencies();
            foreach (var currency in currencies)
            {
                Console.WriteLine(currency);
            }
            string newCurrency = Console.ReadLine().ToUpper();
            if (currencyService.ChangeCurrency(newCurrency))
            {
                Console.WriteLine($"Valuta ändrad till {currencyService.CurrentCurrency}");
            }
            else
                Console.WriteLine("Ogiltig valuta.");
            Console.ReadKey();
        }

        static void ShowCustomerInformation()
        {
            Console.Clear();
            Console.WriteLine(currentCustomer.GetCustomerInformation().ToString());
            Console.ReadKey();
        }
    } */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net.Sockets;

// Produktklassen representerar en vara i butiken
public class Product
{
    public string Name { get; }
    public decimal Price { get; }
    public string Category { get; }

    // Konstruktor för att skapa en ny produkt
    public Product(string name, decimal price, string category)
    {
        Name = name;
        Price = price;
        Category = category;
    }
}

// Abstrakt basklass för alla kundtyper
public abstract class Customer
{
    public string Name { get; }
    public string PasswordHash { get; protected set; }
    protected string OriginalPassword { get; set; }
    protected List<(Product Product, int Quantity)> Cart { get; } = new List<(Product, int)>();

    // Abstrakt egenskap för rabatt, implementeras av subklasser
    public abstract double DiscountPercentage { get; }

    // Konstruktor för att skapa en ny kund
    protected Customer(string name, string password)
    {
        Name = name;
        OriginalPassword = password;
        PasswordHash = HashPassword(password);
    }

    // Metod för att verifiera kundens lösenord
    public bool VerifyPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

    // Metod för att lägga till produkter i kundvagnen
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

    // Metod för att beräkna totalpriset med rabatt
    public decimal GetTotalPrice()
    {
        decimal totalPrice = Cart.Sum(item => item.Product.Price * item.Quantity);
        return totalPrice * (1 - (decimal)DiscountPercentage);
    }

    // Överskuggad ToString-metod för att visa kundinformation och kundvagn
    public override string ToString()
    {
        var cartString = string.Join("\n", Cart.Select(item => $"{item.Product.Name} - {item.Quantity} st - {item.Product.Price * item.Quantity:C}"));
        return $"Kund: {Name} ({GetType().Name})\nRabatt: {DiscountPercentage:P0}\nKundvagn:\n{cartString}\nTotalt (med rabatt): {GetTotalPrice():C}";
    }

    // Metod för att hämta kundens ursprungliga lösenord (används för demo-ändamål)
    public string GetOriginalPassword()
    {
        return OriginalPassword;
    }

    // Privat metod för att hasha lösenord med SHA256
    private static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Metod för att hämta kundinformation
    public CustomerInformation GetCustomerInformation()
    {
        return new CustomerInformation(this);
    }

    // Metod för att sätta lösenordshash (används vid inläsning från fil)
    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
    }
}

// Subklasser för olika kundtyper med specifika rabatter
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

// Klass för att hantera kundinformation
public class CustomerInformation
{
    private Customer customer;

    public CustomerInformation(Customer customer)
    {
        this.customer = customer;
    }

    // Överskuggad ToString-metod för att visa kundinformation
    public override string ToString()
    {
        string customerType = customer switch
        {
            BronzeCustomer => "Bronze",
            SilverCustomer => "Silver",
            GoldCustomer => "Gold",
            _ => "Unknown"
        };
        return $"Namn: {customer.Name}\nLösenord: {customer.GetOriginalPassword()}\nKundtyp: {customerType}";
    }
}

// Klass för att hantera kundtjänster som inläsning och sparande av kunder
public class CustomerService
{
    private List<Customer> customers;
    private const string CustomersFile = "customers.txt";

    public CustomerService()
    {
        LoadCustomers();
    }

    // Metod för att läsa in kunder från fil
    public void LoadCustomers()
    {
        try
        {
            if (File.Exists(CustomersFile))
            {
                customers = File.ReadAllLines(CustomersFile)
                    .Select(FromFileString)
                    .ToList();
            }
            else
            {
                customers = new List<Customer>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading customers: {ex.Message}");
            customers = new List<Customer>();
        }
    }

    // Metod för att spara kunder till fil
    public void SaveCustomers()
    {
        try
        {
            File.WriteAllLines(CustomersFile, customers.Select(ToFileString));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving customers: {ex.Message}");
        }
    }

    // Metod för att hämta en kund baserat på namn
    public Customer GetCustomerByName(string name)
    {
        return customers.FirstOrDefault(c => c.Name == name);
    }

    // Metod för att lägga till en ny kund
    public void AddCustomer(Customer customer)
    {
        if (customers.Any(c => c.Name == customer.Name))
        {
            throw new ArgumentException("A customer with this name already exists.");
        }
        customers.Add(customer);
        SaveCustomers();
    }

    // Metod för att hämta alla kunder
    public List<Customer> GetAllCustomers()
    {
        return customers;
    }

    // Metod för att konvertera en kund till en sträng för fillagring
    public string ToFileString(Customer customer) => $"{customer.GetType().Name},{customer.Name},{customer.PasswordHash},{customer.GetOriginalPassword()}";

    // Metod för att skapa en kund från en sträng från filen
    public Customer FromFileString(string fileString)
    {
        var parts = fileString.Split(',');
        Customer customer = parts[0] switch
        {
            nameof(GoldCustomer) => new GoldCustomer(parts[1], parts[3]),
            nameof(SilverCustomer) => new SilverCustomer(parts[1], parts[3]),
            nameof(BronzeCustomer) => new BronzeCustomer(parts[1], parts[3]),
            _ => throw new ArgumentException("Invalid customer type")
        };
        customer.SetPasswordHash(parts[2]);
        return customer;
    }
}

// Klass för att hantera produkttjänster
public class ProductService
{
    private List<Product> products;

    public ProductService()
    {
        InitializeProducts();
    }

    // Metod för att initialisera produktlistan
    private void InitializeProducts()
    {
        products = new List<Product>
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
    }

    // Metod för att hämta produkter baserat på kategori
    public List<Product> GetProductsByCategory(string category)
    {
        return products.Where(p => p.Category == category).ToList();
    }

    // Metod för att hämta alla unika kategorier
    public List<string> GetCategories()
    {
        return products.Select(p => p.Category).Distinct().ToList();
    }
}

// Klass för att hantera valutatjänster
public class CurrencyService
{
    private Dictionary<string, decimal> exchangeRates;
    public string CurrentCurrency { get; private set; }

    public CurrencyService()
    {
        exchangeRates = new Dictionary<string, decimal>
        {
            { "SEK", 1m },
            { "USD", 0.096m },
            { "EUR", 0.088m }
        };
        CurrentCurrency = "SEK";
    }

    // Metod för att byta valuta
    public bool ChangeCurrency(string newCurrency)
    {
        if (exchangeRates.ContainsKey(newCurrency))
        {
            CurrentCurrency = newCurrency;
            return true;
        }
        return false;
    }

    // Metod för att konvertera belopp till vald valuta
    public decimal ConvertCurrency(decimal amount)
    {
        return amount / exchangeRates["SEK"] * exchangeRates[CurrentCurrency];
    }

    // Metod för att hämta alla tillgängliga valutor
    public List<string> GetAvailableCurrencies()
    {
        return exchangeRates.Keys.ToList();
    }
}

// Huvudprogrammet som hanterar användarinteraktionen
class Program
{
    private static CustomerService customerService;
    private static ProductService productService;
    private static CurrencyService currencyService;
    private static Customer currentCustomer;

    static void Main()
    {
        customerService = new CustomerService();
        productService = new ProductService();
        currencyService = new CurrencyService();

        // Huvudloop för programmet
        while (true)
        {
            if (currentCustomer == null)
                ShowMainMenu();
            else
                ShowShoppingMenu();
        }
    }

    // Metod för att visa huvudmenyn
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
            case "4": Environment.Exit(0); break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    // Metod för inloggning
    static void Login()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();
        currentCustomer = customerService.GetCustomerByName(name);
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

    // Metod för att registrera en ny kund
    static void RegisterNewCustomer()
    {
        Console.Write("Ange namn: ");
        string name = Console.ReadLine();
        Console.Write("Ange lösenord: ");
        string password = Console.ReadLine();
        try
        {
            Console.WriteLine("Välj kundnivå: 1. Bronze, 2. Silver, 3. Gold");
            string choice = Console.ReadLine();

            // Skapar en ny kund baserat på vald nivå
            Customer newCustomer = choice switch
            {
                "1" => new BronzeCustomer(name, password),
                "2" => new SilverCustomer(name, password),
                "3" => new GoldCustomer(name, password),
                _ => new BronzeCustomer(name, password)
            };
            customerService.AddCustomer(newCustomer);
            currentCustomer = newCustomer;
            Console.WriteLine("Ny kund registrerad och inloggad!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.ReadKey();
    }

    // Metod för att visa shoppingmenyn
    static void ShowShoppingMenu()
    {
        Console.Clear();
        Console.WriteLine($"Inloggad som: {currentCustomer.Name}");
        Console.WriteLine("1. Handla");
        Console.WriteLine("2. Se kundvagn");
        Console.WriteLine("3. Gå till kassan");
        Console.WriteLine("4. Visa kundinformation");
        Console.WriteLine("5. Logga ut");
        switch (Console.ReadLine())
        {
            case "1": Shop(); break;
            case "2": ViewCart(); break;
            case "3": Checkout(); break;
            case "4": ShowCustomerInformation(); break;
            case "5": currentCustomer = null; break;
            default: Console.WriteLine("Ogiltigt val. Försök igen."); break;
        }
    }

    // Metod för att hantera shopping
    static void Shop()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Välj kategori");
            var categories = productService.GetCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            Console.WriteLine($"{categories.Count + 1}. Återgå till huvudmenyn");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categories.Count + 1)
            {
                if (choice == categories.Count + 1) return;
                ShopInCategory(categories[choice - 1]);
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Försök igen.");
                Console.ReadKey();
            }
        }
    }

    // Metod för att handla i en specifik kategori
    static void ShopInCategory(string category)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Tillgängliga produkter i kategorin {category}:");
            var categoryProducts = productService.GetProductsByCategory(category);
            for (int i = 0; i < categoryProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categoryProducts[i].Name} - {currencyService.ConvertCurrency(categoryProducts[i].Price):F2} {currencyService.CurrentCurrency}");
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

    // Metod för att visa kundvagnen
    static void ViewCart()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.ReadKey();
    }

    // Metod för att hantera utcheckning
    static void Checkout()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.ToString());
        Console.WriteLine($"Totalt att betala: {currencyService.ConvertCurrency(currentCustomer.GetTotalPrice()):F2} {currencyService.CurrentCurrency}");
        Console.WriteLine("Tack för ditt köp!");
        currentCustomer = null;
        Console.ReadKey();
    }

    // Metod för att byta valuta
    static void ChangeCurrency()
    {
        Console.WriteLine("Välj valuta:");
        var currencies = currencyService.GetAvailableCurrencies();
        foreach (var currency in currencies)
        {
            Console.WriteLine(currency);
        }
        string newCurrency = Console.ReadLine().ToUpper();
        if (currencyService.ChangeCurrency(newCurrency))
        {
            Console.WriteLine($"Valuta ändrad till {currencyService.CurrentCurrency}");
        }
        else
            Console.WriteLine("Ogiltig valuta.");
        Console.ReadKey();
    }

    // Metod för att visa kundinformation
    static void ShowCustomerInformation()
    {
        Console.Clear();
        Console.WriteLine(currentCustomer.GetCustomerInformation().ToString());
        Console.ReadKey();
    }
}
