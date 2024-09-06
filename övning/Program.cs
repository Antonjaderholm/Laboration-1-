// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;

class Car
    {
    public string _make = "";
        public string _color = ""; 
    public void PrintDetails()
    {
        Console.WriteLine("Make: " + _make);
        Console.WriteLine("Color " + _color);
    }

    }
    class Program
    {
        static void Main(string[] args)
        {
            // Skapa två instanser av Car
            Car car1 = new Car();
            car1.Make = "Toyota";
            car1.Color = "Röd";
            Car car2 = new Car();
            car2.Make = "Volvo";
            car2.Color = "Blå";
            // Skriv ut informationen om bilarna
            Console.WriteLine($"Bil 1: Märke: {car1.Make}, Färg: {car1.Color}");
            Console.WriteLine($"Bil 2: Märke: {car2.Make}, Färg: {car2.Color}");
            Console.ReadKey();
        }
    }

