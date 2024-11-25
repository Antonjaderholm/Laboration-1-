/*// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Numerics;

// Globals
bool quit = false;
bool countShortNumbers = true;

while (!quit)
{
    Console.WriteLine("Skriv in en string");
    string input = Console.ReadLine();

    switch (input)
    {
        case "0":
            quit = true;
            break;

        default:

            Labb1(input); // Bearbeta inmatningen direkt.
            break;
    }


    void Labb1(string text)
    {
        Console.WriteLine();    

        int numberIndex = -1;       // Start index för siffran
        int numberIndexEnd = -1;    // Slut index för siffran
        int numbersFound = 0;       // Antal nummer vi har hittat totalt
        string numberText = "";     // Nummer omvandlat till text

        BigInteger totalSumBIG = 0; // Datatyp för att hålla, i teorin, hur stort tal som helst
                                    //long totalSum = 0;

        // Loopa igenom alla tecken i texten
        for (int currentChar = 0; currentChar < text.Length; currentChar++)
        {
            // Om det nuvarande tecknet är en siffra
            if (Char.IsDigit(text[currentChar]))
            {
                // Spara vilket index, siffran har
                numberIndex = currentChar;

                // Loopa igenom resten av texten från och med nästa tecken
                for (int nextChar = currentChar + 1; nextChar < text.Length; nextChar++)
                {
                    // Om ett tecken inte är en siffra avbryter vi andra loopen
                    if (!Char.IsDigit(text[nextChar]))
                    {
                        break;
                    }
                    // Annars om samma siffra uppstår igen
                    else if (text[nextChar] == text[currentChar])
                    {
                        // Spara slutindexet av talet (+1 för att inkludera sista siffran i talet)
                        numberIndexEnd = nextChar + 1;

                        // Om talet enbart är 2 siffror långt & vi ej vill räkna med det, avbryt andra loopen
                        if (numberIndexEnd - numberIndex == 2 && !countShortNumbers)
                        {
                            break;
                        }

                        // Annars hämtar resultatet från metoder här nedan.
                        else
                        {
                            TextColor(text, numberIndex, numberIndexEnd, ConsoleColor.Blue);
                        }

                        // Spara talet i en egen sträng
                        numberText = text.Substring(numberIndex, numberIndexEnd - numberIndex);
                        // Konvertera strängen & plussa på värdet i den totala summan
                        totalSumBIG += BigInteger.Parse(numberText);
                        // Öka antalet hittade nummer med +1
                        numbersFound++;
                        // Bryt ur andra loopen och återgå till första loopen
                        break;
                    }
                }
            }
        }


        Console.WriteLine("Antal = " + numbersFound);   // Antal hittade nummer
        Console.WriteLine("Total = " + totalSumBIG);    // Totala summan

    }

    // Color

    void TextColor(string text, int start, int end, ConsoleColor color)
    {
        // Loopa igenom alla tecken i texten
        for (int i = 0; i < text.Length; i++)
        {
            // Om det nuvarande tecknet är inom ramen för 'start' indexet && inte gått förbi 'end' indexet, färga texten.
            if (i >= start && i < end)
            {

                // Sätt färgen & skriv ut texten
                Console.ForegroundColor = color;
                Console.Write(text[i]);
                Console.ResetColor();   // Både för- & bakgrundsfärger återställs
            }
            // Annars skriv ut tecknet som vanligt
            else
            {
                Console.Write(text[i]);
            }
        }

        Console.WriteLine("");    // Gör en ny rad
    }
    break;
}
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public enum CustomerTier { Gold, Silver, Bronze };

public class Customer
{
    public string Name { get; private set; }
    private string Password { get; private set; }
    public CustomerTier Tier { get; private set; }
    private List<Product> _cart;
    public List<Product> Cart { get { return _cart; } }

    public Customer(string name, string password, CustomerTier tier)
    {
        Name = name;
        Password = password;
        Tier = tier;
        _cart = new List<Product>();
    }

    public bool VerifyPassword(string password)
    {
        return Password == password;
    }

    public decimal CalculateTotalDiscount()
    {
        decimal discount = 0;
        switch (Tier)
        {
            case CustomerTier.Gold:
                discount = 0.15m;
                break;
            case CustomerTier.Silver:
                discount = 0.10m;
                break;
            case CustomerTier.Bronze:
                discount = 0.05m;
                break;
        }
        return discount;
    }

    public decimal CalculateTotalCartPrice()
    {
        decimal total = 0;
        foreach (Product product in Cart)
        {
            total += product.Price;
        }
        return total * (1 - CalculateTotalDiscount());
    }

    public override string ToString()
    {
        return $"Customer: {Name}\nPassword: {Password}\nTier: {Tier}\nCart: {string.Join(", ", Cart)}\nTotal price: {CalculateTotalCartPrice():C2}\n";
    }
}

public class Product
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; }

    public Product(string name, decimal price, string currency = "SEK")
    {
        Name = name;
        Price = price;
        Currency = currency;
    }

    public decimal GetPriceInCurrency(string currency)
    {
        // Implement logic to convert the price to the requested currency here
        // (You would need to add exchange rate data or integration with a currency API).
        if (currency == Currency)
        {
            return Price;
        }
        else
        {
            // Placeholder for currency conversion (replace with actual logic)
            return Price * 1.1m; // Example: Assuming 10% difference for currency conversion
        }
    }
}

public class Store
{
    private string _customerDataFile = "customers.txt";
    private List<Customer> _customers;
    private List<Product> _products;

    public Store()
    {
        _customers = LoadCustomers();
        _products = new List<Product>
        {
            new Product("Sausage", 15.00m),
            new Product("Drink", 10.00m),
            new Product("Apple", 5.00m)
        };
    }

    private List<Customer> LoadCustomers()
    {
        List<Customer> customers = new List<Customer>();
        if (File.Exists(_customerDataFile))
        {
            string[] lines = File.ReadAllLines(_customerDataFile);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 4)
                {
                    string name = parts[0].Trim();
                    string password = parts[1].Trim();
                    CustomerTier tier = (CustomerTier)Enum.Parse(typeof(CustomerTier), parts[2].Trim());
                    customers.Add(new Customer(name, password, tier));
                }
            }
        }
        return customers;
    }

    private void SaveCustomers()
    {
        List<string> customerLines = _customers.Select(c => $"{c.Name},{c.Password},{c.Tier}").ToList();
        File.WriteAllLines(_customerDataFile, customerLines);
    }

    public Customer Login(string username, string password)
    {
        Customer customer = _customers.FirstOrDefault(c => c.Name == username && c.VerifyPassword(password));
        if (customer != null)
        {
            return customer;
        }
        else
        {
            return null;
        }
    }

    public void Register(string name, string password, CustomerTier tier)
    {
        if (_customers.Any(c => c.Name == name))
        {
            Console.WriteLine("A customer with this name already exists.");
        }
        else
        {
            Customer newCustomer = new Customer(name, password, tier);
            _customers.Add(newCustomer);
            SaveCustomers();
            Console.WriteLine("Customer registered successfully.");
        }
    }

    public void Shop(Customer customer)
    {
        while (true)
        {
            Console.WriteLine("Available Products:");
            for (int i = 0; i < _products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_products[i].Name} - {_products[i].Price:C2} {_products[i].Currency}");
            }
            Console.WriteLine("0. Back to main menu");

            Console.Write("Choose a product (or enter 0 to go back): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= _products.Count)
            {
                if (choice == 0)
                {
                    break;
                }
                else
                {
                    Product product = _products[choice - 1];
                    Console.Write($"Enter quantity for {product.Name}: ");
                    if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                    {
                        for (int i = 0; i < quantity; i++)
                        {
                            customer.Cart.Add(product);
                        }
                        Console.WriteLine($"{quantity} {product.Name} added to cart.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid quantity.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
    }

    public void ViewCart(Customer customer)
    {
        if (customer.Cart.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
        }
        else
        {
            Console.WriteLine("Your Cart:");
            decimal totalPrice = 0;
            foreach (Product product in customer.Cart)
            {
                Console.WriteLine($"{product.Name} - {product.Price:C2} {product.Currency}");
                totalPrice += product.Price;
            }
            Console.WriteLine($"Total price: {totalPrice:C2} {product.Currency}");
            Console.WriteLine($"Total price after discount: {customer.CalculateTotalCartPrice():C2} {product.Currency}");
        }
    }

    public void Checkout(Customer customer)
    {
        Console.WriteLine("Checkout:");
        Console.WriteLine($"Total price: {customer.CalculateTotalCartPrice():C2} {product.Currency}");
        Console.WriteLine("Payment simulation successful.");
        customer.Cart.Clear();
        Console.WriteLine("Your cart is now empty.");
    }

    public void MainMenu(Customer customer)
    {
        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Shop");
            Console.WriteLine("2. View Cart");
            Console.WriteLine("3. Checkout");
            Console.WriteLine("4. Logout");

            Console.Write("Choose an option: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        Shop(customer);
                        break;
                    case 2:
                        ViewCart(customer);
                        break;
                    case 3:
                        Checkout(customer);
                        break;
                    case 4:
                        Console.WriteLine("Logged out.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }
}

public class Program
{
    static void Main(string[] args)
    {
        Store store = new Store();

        while (true)
        {
            Console.WriteLine("\nWelcome to the Store!");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            Console.Write("Choose an option: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter your name: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter your password: ");
                        string password = Console.ReadLine();
                        Console.WriteLine("Choose your customer tier:");
                        Console.WriteLine("1. Gold");
                        Console.WriteLine("2. Silver");
                        Console.WriteLine("3. Bronze");
                        Console.Write("Enter your choice: ");
                        if (int.TryParse(Console.ReadLine(), out int tierChoice))
                        {
                            CustomerTier tier = (CustomerTier)(tierChoice - 1);
                            store.Register(name, password, tier);
                        }
                        else
                        {
                            Console.WriteLine("Invalid tier choice.");
                        }
                        break;
                    case 2:
                        Console.Write("Enter your username: ");
                        string username = Console.ReadLine();
                        Console.Write("Enter your password: ");
                        string password = Console.ReadLine();
                        Customer customer = store.Login(username, password);
                        if (customer != null)
                        {
                            Console.WriteLine($"Welcome, {customer.Name}!");
                            store.MainMenu(customer);
                        }
                        else
                        {
                            Console.WriteLine("Invalid username or password.");
                            Console.WriteLine("Do you want to register a new customer? (y/n)");
                            if (Console.ReadLine().ToLower() == "y")
                            {
                                Console.Write("Enter your name: ");
                                name = Console.ReadLine();
                                Console.Write("Enter your password: ");
                                password = Console.ReadLine();
                                Console.WriteLine("Choose your customer tier:");
                                Console.WriteLine("1. Gold");
                                Console.WriteLine("2. Silver");
                                Console.WriteLine("3. Bronze");
                                Console.Write("Enter your choice: ");
                                if (int.TryParse(Console.ReadLine(), out tierChoice))
                                {
                                    tier = (CustomerTier)(tierChoice - 1);
                                    store.Register(name, password, tier);
                                    customer = store.Login(name, password);
                                    store.MainMenu(customer);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid tier choice.");
                                }
                            }
                        }
                        break;
                    case 3:
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }
}