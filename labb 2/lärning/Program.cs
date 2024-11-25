// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;
using System.Globalization;
using System.Linq.Expressions;
using System.Xml.Schema;

namespace upplärning
{
    internal class Program
    {
        static void Main(string[] args)

        {
            /*  double number1 = 8;
              double number2 = 3;
              double result = number1 / number2;

              int number3 = 5;

              Console.WriteLine(number3);
              Console.WriteLine(number3); */

            /*int number4 = 6;
            int number5 = 5;
            number4 += number5;
            number5 += 2;

            Console.WriteLine(number4);*/

            /* int minutes = 60;
             int dollars = 3000;
             int goalDollar = 2000;
             bool accountmoney = true;

             bool isExactlyOneHour = minutes == 60;
             bool Isthedollar = dollars >= goalDollar;
             bool accounthasmoney = dollars > 0;
             bool acccountmoney = dollars > 0;

             Console.WriteLine(isExactlyOneHour);
             Console.WriteLine(accounthasmoney);
             Console.WriteLine(Isthedollar);*/

            /*int number1 = 10;
            double number2 = 8.5;

            double number3 = number1 + number2;

            int number4 = (int) number2;
            int number5 = (int)(number2 + number3);

            int number6 = Convert.ToInt32(number2);

            string textNumber5 = "5";
            int integer5 = Convert.ToInt32(number5);

            Console.WriteLine(number4);
            Console.WriteLine(number5);
            Console.WriteLine(number6);*/

            /*Console.WriteLine("Skriv in ditt namn" );
            string input = Console.ReadLine();
            Console.WriteLine("Hej" + " " + input + " " + "Nu ska du få spela ett spel");

            Console.WriteLine("Skriv in din ålder");
            string ålder = Console.ReadLine();
            Console.WriteLine("Din ålder är" + " " + 5 + 5);

            string ålderOm36år = ålder + 30; 
            Console.WriteLine("Din ålder om 36 år är" + ålderOm36år);*/

            /* bool isActive = true;
             Console.WriteLine(" Är du säker? Slå in ja eller nej");
             string input = Console.ReadLine();

             if (input == "ja")
             {
                 Console.WriteLine(" Du är säker");
             }
              else if (input == "nej")
             {
                 Console.WriteLine( "dU ÄR INTE SÄKER");
             }
             else
             {
                 Console.WriteLine("Gör om fel skrivet");

             }
             Console.WriteLine();
             Console.ReadLine();*/
            /*bool condition = true;

            switch (condition) 
            { 
               case true:
               {
                        Console.WriteLine("Its true");
                        break;
                    }
                    case false: 
                    {
                        Console.WriteLine(" its false");
                        break;
                    }
                default:
                    break; */

            /*Console.WriteLine("cake you like?");
            string input = Console.ReadLine();
            switch (input)

            {
                case "yes":
                    Console.WriteLine(" Me to");
                    break;

                case "no":
                    Console.WriteLine("Either do i");
                    break;

                default:
                    Console.WriteLine("Skriv in ny");
                    break;
            }
                      Console.WriteLine();
            Console.WriteLine();
            */


            /* bool hasValidInput = false;
             bool canDelete = false;

             while (!hasValidInput)
             {
                 Console.WriteLine("are you sure? enter yes or no: ");
                 string input = Console.ReadLine();
                 if (input == "yes" || input == "no")
                 {
                     hasValidInput = true;
                     if (input == "yes")
                     {
                         canDelete = true;
                     }
                     else
                     {
                         canDelete = false;
                     }

                 }
                 else
                 {
                     Console.WriteLine("invalid input, try again");

                 }
                 if (canDelete)
                 {

                 }


                 {
                     Console.WriteLine(canDelete);

                     Console.WriteLine();
                 }*/

            /* for(int i = 10; i >= 0; i-=2)
             {
                 Console.WriteLine(i);
             }*/

            /*int[] myInts = {1,2,3,4};
             int[] myInts2 = new int[10];
             int[] myInts3;
             myInts3 = new int[] {1,2,3,4}; 


             int myFirtsInt = myInts[0]; 
             int myFourInt= myInts2[3];

             string[] myNames = { "An", "sv", "te", "we", "pandy" };

             for (int i = 0; i < myNames.Length; i++)
             {
                 Console.WriteLine(myNames[i]);
             }
             Console.ReadLine(); */

            // LIST

            /*List<string> myNames;
            myNames = new List<string> { "Anton", "Anna", "Elle"};

            string myFirstName = myNames[0];
            string myThirdName = myNames[2];

            myNames.Clear();

            /*Console.WriteLine(myFirstName);
            Console.WriteLine(myThirdName);

            for (int i = 0; i< myNames.Count; i++)
            {
                Console.WriteLine(myNames[i]);
            }

            Console.WriteLine();


            Console.ReadLine();
            
            myNames.Add("Sean");
             myNames.Add("Pam");

             for (int i = 0; i < myNames.Count; i++)
             {
                 Console.WriteLine(myNames[i]);
             }
             Console.WriteLine();

             myNames.Remove("Elle");
             myNames.Remove("Anton");

             for (int i = 0; i < myNames.Count; i++)
             {
                 Console.WriteLine(myNames[i]);
             }*/


            // for each loop


            /*List<string> myNames = new List<string> { "Anton", "Anna", "Elle", "Magda" };


            foreach (string name in myNames)
            {
                Console.WriteLine(name);
            }
            */
            /* List<int> myints = new List<int> {1, 2, 3, 4, 5 };

             foreach (int number in myints)
             {
                 Console.WriteLine(number);
             }


             /*for (int i = 0; i < myNames.Count; i++)
             {
                 Console.WriteLine(myNames[i]);
             }*/

            //kod game

            /*Console.WriteLine();
            Console.ReadLine();

            List<string> signUpsNames = new List<string>();
            
            while (true) 
            {
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("------------SIGN UP FOR GIVEAWAY------------");
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine();

                Console.Write("First name: ");
                string firstname = Console.ReadLine();
                Console.Write(" last name: ");
                string lastname = Console.ReadLine();


                if (firstname == "admin" && lastname == "print")
                { 
                    Console.Clear();
                Console.WriteLine("sign ups: ");
                Console.WriteLine("-------------------------------------------");

                foreach (string name in signUpsNames)
                    Console.WriteLine(name);
                Console.WriteLine();
                Console.WriteLine(" Press enter to return");
                 }
                else
                 {
                    signUpsNames.Add(firstname + " " + lastname);

                    Console.WriteLine();
                    Console.WriteLine("Thank you for signing up!");
                    Console.WriteLine("Press enter to finish");

                }
                Console.ReadLine();
                Console.Clear() ; */

            //break & continue
        }
        List<string> names = new List<string> { "Heh", "debh", "bheb", "uebbjec" };
   

    }
}
