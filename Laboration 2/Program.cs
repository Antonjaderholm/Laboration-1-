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
    protected string PasswordHash { get; }
    protected List<(Product Product, int Quantity)> Cart { get; } = new List<(Product, int)>();
    public abstract double DiscountPercentage { get; }

    protected Customer(string name, string passwordHash)
    {
        Name = name;
        PasswordHash = passwordHash;
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

    public string ToFileString() => $"{GetType().Name},{Name},{PasswordHash}";

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
}