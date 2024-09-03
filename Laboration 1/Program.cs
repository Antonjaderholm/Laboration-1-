// See https://aka.ms/new-console-template for more information
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

            Labb1(input); // Process the input directly 
            break;
    }


    void Labb1(string text)
    {
        Console.WriteLine();    // Ny rad

        int numberIndex = -1;       // Start index för siffran
        int numberIndexEnd = -1;    // Slut index för siffran
        int numbersFound = 0;       // Antal nummer vi har hittat totalt
        string numberText = "";     // Nummer omvandlat till text

        BigInteger totalSumBIG = 0; // Datatyp för att hålla, i teorin, ett hur stort tal som helst
                                    //long totalSum = 0;

        // Loopa igenom alla tecken i texten
        for (int currentChar = 0; currentChar < text.Length; currentChar++)
        {
            // Om det nuvarande tecknet är en siffra
            if (Char.IsDigit(text[currentChar]))
            {
                // Spara vilket index siffran har
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

                        // Om talet bara är 2 siffror långt & vi inte vill räkna med det, avbryt andra loopen
                        if (numberIndexEnd - numberIndex == 2 && !countShortNumbers)
                        {
                            break;
                        }

                        // Annars hämtar resultatet från metoder nedan.
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
