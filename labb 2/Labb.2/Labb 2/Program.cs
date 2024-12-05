// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using static BokhandelSystem.Program;

namespace BokhandelSystem
{
    // Modellklass för butiker - representerar fysiska bokhandlar
    public class Butiker
    {
        public int Id { get; set; }
        public string Butiksnamn { get; set; }
        public string Gatuadress { get; set; }
        public string Postnummer { get; set; }
        public string Stad { get; set; }

        // Relation till lagersaldo - en butik kan ha många böcker
        public ICollection<Lagersaldo> Lagersaldo { get; set; }
    }

    // Modellklass för böcker - representerar alla böcker i sortimentet
    public class Böcker
    {
        [Key]
        public string ISBN13 { get; set; }

        public string Titel { get; set; }

        [Column("Språk")]
        public string Språk { get; set; }

        public decimal Pris { get; set; }

        public DateTime Utgivningsdatum { get; set; }

        [Column("FörfattareId")]
        public int FörfattareId { get; set; }

        [Column("FörlagId")]
        public int FörlagId { get; set; }

        [Column("KategoriId")]
        public int KategoriId { get; set; }

        public virtual Författare Författare { get; set; }
        public virtual Förlag Förlag { get; set; }
        public virtual Kategorier Kategori { get; set; }
        public virtual ICollection<Lagersaldo> Lagersaldo { get; set; }
    }

    // Modellklass för författare
    public class Författare
    {
        public int Id { get; set; }
        public string Förnamn { get; set; }
        public string Efternamn { get; set; }
        public DateTime Födelsedatum { get; set; }

        // En författare kan ha skrivit flera böcker
        public ICollection<Böcker> Böcker { get; set; }
    }

    // Modellklass för förlag
    public class Förlag
    {
        public int Id { get; set; }
        public string Namn { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }

        // Ett förlag kan ha gett ut många böcker
        public ICollection<Böcker> Böcker { get; set; }
    }

    // Modellklass för kategorier
    public class Kategorier
    {
        public int Id { get; set; }
        public string Namn { get; set; }
        public string Beskrivning { get; set; }

        // En kategori kan innehålla många böcker
        public ICollection<Böcker> Böcker { get; set; }
    }

    // Modellklass för lagersaldo - kopplar ihop butiker med böcker
    public class Lagersaldo
    {
        public int ButikId { get; set; }
        public string ISBN { get; set; }
        public int Antal { get; set; }

        [ForeignKey("ISBN")]
        public virtual Böcker Bok { get; set; }
        public virtual Butiker Butik { get; set; }
    }
    // DbContext-klass som hanterar databasanslutningen
    public class BokhandelContext : DbContext
    {
        // DbSet-properties för varje tabell i databasen
        public DbSet<Butiker> Butiker { get; set; }
        public DbSet<Böcker> Böcker { get; set; }
        public DbSet<Författare> Författare { get; set; }
        public DbSet<Förlag> Förlag { get; set; }
        public DbSet<Kategorier> Kategorier { get; set; }
        public DbSet<Lagersaldo> Lagersaldo { get; set; }

        // Konfigurerar anslutningen till SQL Server
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=Bokhandel;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        // Konfigurerar modellrelationer och mappningar
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mappar entiteter till existerande databastabeller
            modelBuilder.Entity<Butiker>().ToTable("Butiker", schema: "dbo");
            modelBuilder.Entity<Böcker>().ToTable("Böcker", schema: "dbo");
            modelBuilder.Entity<Författare>().ToTable("Författare", schema: "dbo");
            modelBuilder.Entity<Lagersaldo>().ToTable("Lagersaldo", schema: "dbo");

            // Konfigurerar primärnycklar
            modelBuilder.Entity<Böcker>().HasKey(b => b.ISBN13);
            modelBuilder.Entity<Lagersaldo>().HasKey(l => new { l.ButikId, l.ISBN });
        }
    }

    // Huvudprogramklass
    class Program
    {
        static void Main(string[] args)
        {
            // Skapar en ny instans av databaskontext
            using var context = new BokhandelContext();

            // Huvudprogramloop
            while (true)
            {
                // Visar huvudmenyn
                Console.WriteLine("=== Bokhandelssystem ===");
                Console.WriteLine("1. Visa lagersaldo");
                Console.WriteLine("2. Lägg till bok i butik");
                Console.WriteLine("3. Ta bort bok från butik");
                Console.WriteLine("4. Lägg till ny bok");
                Console.WriteLine("5. Lägg till författare");
                Console.WriteLine("6. Redigera bok");
                Console.WriteLine("7. Redigera författare");
                Console.WriteLine("8. Ta bort bok");
                Console.WriteLine("9. Ta bort författare");
                Console.WriteLine("10. Avsluta");

                // Hanterar användarens val
                var val = Console.ReadLine();

                switch (val)
                {
                    case "1":
                        VisaLagersaldo(context);
                        break;
                    case "2":
                        LäggTillBokIButik(context);
                        break;
                    case "3":
                        TaBortBokFrånButik(context);
                        break;
                    case "4":
                        LäggTillNyBok(context);
                        break;
                    case "5":
                        LäggTillFörfattare(context);
                        break;
                    case "6":
                        RedigeraBok(context);
                        break;
                    case "7":
                        RedigeraFörfattare(context);
                        break;
                    case "8":
                        TaBortBok(context);
                        break;
                    case "9":
                        TaBortFörfattare(context);
                        break;
                    case "10":
                        return;
                }
            }
        }

        private static void LäggTillFörfattare(BokhandelContext context)
        {
            Console.WriteLine("\nLägg till ny författare");

            Console.Write("Förnamn: ");
            var förnamn = Console.ReadLine();

            Console.Write("Efternamn: ");
            var efternamn = Console.ReadLine();

            Console.Write("Födelsedatum (ÅÅÅÅ-MM-DD): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime födelsedatum))
            {
                try
                {
                    var nyFörfattare = new Författare
                    {
                        Förnamn = förnamn,  // Matcha det exakta egenskapsnamnet från din modell
                        Efternamn = efternamn,
                        Födelsedatum = födelsedatum,  // Matcha det exakta egenskapsnamnet från din modell
                        Böcker = new List<Böcker>()
                    };

                    context.Författare.Add(nyFörfattare);
                    context.SaveChanges();
                    Console.WriteLine($"Författare {förnamn} {efternamn} har lagts till!");
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Databasfel: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel uppstod: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt datumformat.");
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        // DTO för grundläggande bokinformation
        public class BookDetails
        {
            public string ISBN13 { get; set; }
            public string Titel { get; set; }
            public string Språk { get; set; }
            public decimal Pris { get; set; }
            public DateTime Utgivningsdatum { get; set; }
            public string Författare { get; set; }
            public string Förlag { get; set; }
        }

        // DTO för bokinformation inklusive lagersaldo
        public class BookInventoryDetails
        {
            public string ISBN13 { get; set; }
            public string Titel { get; set; }
            public string Språk { get; set; }
            public decimal Pris { get; set; }
            public DateTime Utgivningsdatum { get; set; }
            public string Författare { get; set; }
            public string Förlag { get; set; }
            public int Antal { get; set; }
        }
        static void VisaLagersaldo(BokhandelContext context)
        {
            // Visa tillgänliga butiker
            Console.WriteLine("\nTillgängliga butiker:");
            var butiker = context.Butiker.ToList();
            foreach (var butik in butiker)
            {
                Console.WriteLine($"{butik.Id}. {butik.Butiksnamn}");
            }

            // Välj bland butiker
            Console.Write("\nVälj butik (ID): ");
            if (!int.TryParse(Console.ReadLine(), out int selectedStoreId))
            {
                Console.WriteLine("Ogiltigt val av butik.");
                return;
            }

            // Få inventory för valda butiker med alla bok detaljer.
            var stockQuery = FormattableStringFactory.Create($@"
    SELECT 
        b.ISBN13,
        b.Titel,
        b.Språk,
        b.Pris,
        b.Utgivningsdatum,
        CONCAT(f.Förnamn, ' ', f.Efternamn) as Författare,
        fl.Namn as Förlag,
        COALESCE(l.Antal, 0) as Antal
    FROM dbo.Böcker b
    LEFT JOIN dbo.Lagersaldo l ON b.ISBN13 = l.ISBN AND l.ButikId = {selectedStoreId}
    LEFT JOIN dbo.Författare f ON b.FörfattareId = f.Id
    LEFT JOIN dbo.Förlag fl ON b.FörlagId = fl.Id
    ORDER BY b.Titel");

            var inventory = context.Database
                .SqlQuery<BookInventoryDetails>(stockQuery)
                .ToList();

            // Display results
            var selectedStore = butiker.FirstOrDefault(b => b.Id == selectedStoreId);
            if (selectedStore != null)
            {
                Console.WriteLine($"\nLagersaldo för {selectedStore.Butiksnamn}:");
                Console.WriteLine("------------------------");

                foreach (var item in inventory)
                {
                    Console.WriteLine($"ISBN13: {item.ISBN13}");
                    Console.WriteLine($"Titel: {item.Titel}");
                    Console.WriteLine($"Författare: {item.Författare}");
                    Console.WriteLine($"Förlag: {item.Förlag}");
                    Console.WriteLine($"Språk: {item.Språk}");
                    Console.WriteLine($"Pris: {item.Pris:C}");
                    Console.WriteLine($"Antal i lager: {item.Antal} st");
                    Console.WriteLine("------------------------");
                }
            }
            else
            {
                Console.WriteLine("Butiken hittades inte.");
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }



        // Lägger till en bok i en butiks lager
        static void LäggTillBokIButik(BokhandelContext context)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\nBöcker som finns i systemet:");
                var books = context.Database.SqlQuery<BookDetails>($@"
    SELECT 
        b.ISBN13, 
        b.Titel, 
        b.[Språk], 
        b.Pris, 
        b.Utgivningsdatum,
        CONCAT(f.[Förnamn], ' ', f.[Efternamn]) as Författare,
        fl.[Namn] as Förlag
    FROM dbo.Böcker b
    LEFT JOIN dbo.[Författare] f ON b.[FörfattareId] = f.Id
    LEFT JOIN dbo.[Förlag] fl ON b.[FörlagId] = fl.Id
    ORDER BY b.Titel").ToList();

                Console.WriteLine("\nBöcker som finns i systemet:");
                foreach (var book in books)
                {
                    Console.WriteLine($"ISBN13: {book.ISBN13}");
                    Console.WriteLine($"Titel: {book.Titel}");
                    Console.WriteLine($"Författare: {book.Författare}");
                    Console.WriteLine($"Förlag: {book.Förlag}");
                    Console.WriteLine($"Pris: {book.Pris:C}");
                    Console.WriteLine($"Språk: {book.Språk}");
                    Console.WriteLine("------------------------");
                }

                Console.WriteLine("\nTillgängliga butiker:");
                var butiker = context.Butiker.ToList();
                foreach (var butik in butiker)
                {
                    Console.WriteLine($"{butik.Id}. {butik.Butiksnamn}");
                }

                Console.Write("\nVälj butik (ID): ");
                var butikInput = Console.ReadLine();

                if (string.IsNullOrEmpty(butikInput))
                {
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    continue;
                }

                if (int.TryParse(butikInput, out int butikId))
                {
                    Console.Write("Ange ISBN13 för boken: ");
                    var isbn = Console.ReadLine();
                    Console.Write("Ange antal: ");
                    var antalInput = Console.ReadLine();

                    if (int.TryParse(antalInput, out int antal))
                    {
                        try
                        {
                            var existing = context.Lagersaldo
                                .FirstOrDefault(l => l.ButikId == butikId && l.ISBN == isbn);

                            if (existing != null)
                            {
                                existing.Antal += antal;
                            }
                            else
                            {
                                var lagersaldo = new Lagersaldo
                                {
                                    ButikId = butikId,
                                    ISBN = isbn,
                                    Antal = antal
                                };
                                context.Lagersaldo.Add(lagersaldo);
                            }

                            using (var transaction = context.Database.BeginTransaction())
                            {
                                context.SaveChanges();
                                transaction.Commit();
                                Console.WriteLine("\nBoken har lagts till i butikens lager!");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ett fel uppstod vid sparande: {ex.Message}");
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine($"Detaljerat fel: {ex.InnerException.Message}");
                            }
                        }
                    }

                    Console.Write("\nVill du lägga till fler böcker? (J/N): ");
                    var svar = Console.ReadLine()?.ToUpper();
                    isRunning = svar == "J";
                }
            }
        }
        static void LäggTillNyBok(BokhandelContext context)
        {
            Console.WriteLine("\nLägg till ny bok");

            // Visa tillgänliga författare
            var authors = context.Database.SqlQuery<AuthorListItem>($@"
        SELECT 
            Id,
            CONCAT([Förnamn], ' ', [Efternamn]) as FullName
        FROM dbo.[Författare]
        ORDER BY [Efternamn], [Förnamn]").ToList();

            Console.WriteLine("\nTillgängliga författare:");
            foreach (var author in authors)
            {
                Console.WriteLine($"{author.Id}. {author.FullName}");
            }

            // Visa tillgänliga publishers
            var publishers = context.Database.SqlQuery<PublisherListItem>($@"
    SELECT 
        Id,
        [Namn] as Name
    FROM [dbo].[Förlag]
    ORDER BY [Namn]").ToList();

            Console.WriteLine("\nTillgängliga förlag:");
            foreach (var publisher in publishers)
            {
                Console.WriteLine($"{publisher.Id}. {publisher.Name}");
            }

            Console.Write("\nISBN13: ");
            var isbn = Console.ReadLine();

            Console.Write("Titel: ");
            var titel = Console.ReadLine();

            Console.Write("Språk (Svenska/English): ");
            var språk = Console.ReadLine();

            Console.Write("Pris: ");
            decimal.TryParse(Console.ReadLine(), out decimal pris);

            Console.Write("Välj författare (ID): ");
            int.TryParse(Console.ReadLine(), out int författareId);

            Console.Write("Välj förlag (ID): ");
            int.TryParse(Console.ReadLine(), out int förlagId);

            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var insertQuery = FormattableStringFactory.Create(@"
            INSERT INTO [dbo].[Böcker] 
                (ISBN13, Titel, [Språk], Pris, Utgivningsdatum, [FörfattareId], [FörlagId])
            VALUES 
                ({0}, {1}, {2}, {3}, GETDATE(), {4}, {5})",
                        isbn, titel, språk, pris, författareId, förlagId);

                    var rowsAffected = context.Database.ExecuteSql(insertQuery);
                    transaction.Commit();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("\nBoken har lagts till!");
                        Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod: {ex.Message}");
                Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
        }

        public class PublisherListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class AuthorListItem
        {
            public int Id { get; set; }
            public string FullName { get; set; }
        }

        public class BookEditDTO
        {
            public string ISBN13 { get; set; }
            public string Titel { get; set; }
            public decimal Pris { get; set; }
            public string Språk { get; set; }
        }

        // Redigerar en befintlig bok
        static void RedigeraBok(BokhandelContext context)
        {
            Console.WriteLine("\nVälj bok att redigera:");

            var books = context.Database
                .SqlQuery<BookDetails>($@"
        SELECT 
            b.ISBN13, 
            b.Titel, 
            b.[Språk], 
            b.Pris, 
            b.Utgivningsdatum,
            CONCAT(f.[Förnamn], ' ', f.[Efternamn]) as Författare,
            fl.[Namn] as Förlag
        FROM dbo.Böcker b
        LEFT JOIN dbo.[Författare] f ON b.[FörfattareId] = f.Id
        LEFT JOIN dbo.[Förlag] fl ON b.[FörlagId] = fl.Id")
                .ToList();

            foreach (var book in books)
            {
                Console.WriteLine($"ISBN13: {book.ISBN13}");
                Console.WriteLine($"Titel: {book.Titel}");
                Console.WriteLine($"Författare: {book.Författare ?? "Okänd"}");
                Console.WriteLine($"Förlag: {book.Förlag ?? "Okänt"}");
                Console.WriteLine("------------------------");
            }

            // Här kommer redigeringsdelen kring isbn input
            Console.Write("\nAnge ISBN13 för boken som ska redigeras: ");
            var isbn = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(isbn))
            {
                Console.WriteLine("ISBN kan inte vara tomt.");
                Console.ReadKey();
                return;
            }


            // Få book med säker SQL query
            var valdBok = context.Database
    .SqlQuery<BookEditDTO>(FormattableStringFactory.Create(@"
        SELECT 
            ISBN13,
            Titel,
            Pris,
            [Språk]
        FROM dbo.Böcker 
        WHERE ISBN13 = {0}", isbn))
    .FirstOrDefault();

            if (valdBok != null)
            {
                // Visa nuvarande värden
                Console.WriteLine($"\nNuvarande värden:");
                Console.WriteLine($"Titel: {valdBok.Titel}");
                Console.WriteLine($"Pris: {valdBok.Pris:C}");

                // Uppdatera titel
                Console.Write("\nNy titel (eller Enter för att behålla nuvarande): ");
                var nyTitel = Console.ReadLine();

                // Uppdatera pris
                Console.Write("Nytt pris (eller Enter för att behålla nuvarande): ");
                var nyPrisStr = Console.ReadLine();

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Uppdatera titel om ny angetts
                        if (!string.IsNullOrEmpty(nyTitel))
                        {
                            var updateTitleQuery = FormattableStringFactory.Create(
                                "UPDATE dbo.Böcker SET Titel = {0} WHERE ISBN13 = {1}",
                                nyTitel,
                                valdBok.ISBN13
                            );
                            context.Database.ExecuteSql(updateTitleQuery);
                        }

                        // Uppdatera pris om nytt angetts och är giltigt
                        if (!string.IsNullOrEmpty(nyPrisStr) && decimal.TryParse(nyPrisStr, out decimal nyPris))
                        {
                            var updatePriceQuery = FormattableStringFactory.Create(
                                "UPDATE dbo.Böcker SET Pris = {0} WHERE ISBN13 = {1}",
                                nyPris,
                                valdBok.ISBN13
                            );
                            context.Database.ExecuteSql(updatePriceQuery);
                        }

                        transaction.Commit();
                        Console.WriteLine("\nBoken har uppdaterats framgångsrikt!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"\nEtt fel uppstod vid uppdateringen: {ex.Message}");
                    }
                }

                Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine($"\nIngen bok hittades med ISBN: {isbn}");
                Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
        }

        public class BookStockDetails
        {
            public string ISBN { get; set; }
            public string Titel { get; set; }
            public string Språk { get; set; }
            public decimal Pris { get; set; }
            public DateTime Utgivningsdatum { get; set; }
            public string Författare { get; set; }
            public string Förlag { get; set; }
            public string Butiksnamn { get; set; }
            public int Antal { get; set; }
            public int? ButikId { get; set; }
            public string SortButik { get; set; }
            public string SortTitel { get; set; }
        }

        // Tar bort en bok från en butiks lager
        static void TaBortBokFrånButik(BokhandelContext context)
        {
            Console.WriteLine("\nTillgängliga butiker:");
            var butiker = context.Butiker.ToList();
            foreach (var butik in butiker)
            {
                Console.WriteLine($"{butik.Id}. {butik.Butiksnamn}");
            }

            Console.Write("\nVälj butik (ID): ");
            int selectedStoreId = int.Parse(Console.ReadLine());

            var stockQuery = FormattableStringFactory.Create($@"
    SELECT DISTINCT
        b.ISBN13 as ISBN,
        b.Titel,
        b.Språk as Språk,
        b.Pris,
        b.Utgivningsdatum,
        COALESCE(CONCAT(f.Förnamn, ' ', f.Efternamn), 'Okänd') as Författare,
        fl.[Namn] as Förlag,
        COALESCE(bt.Butiksnamn, 'Ej i lager') as Butiksnamn,
        COALESCE(l.Antal, 0) as Antal,
        bt.Id as ButikId,
        bt.Butiksnamn as SortButik,
        b.Titel as SortTitel
    FROM dbo.Böcker b
    LEFT JOIN dbo.Lagersaldo l ON b.ISBN13 = l.ISBN AND l.ButikId = {selectedStoreId}
    LEFT JOIN dbo.Butiker bt ON l.ButikId = bt.Id
    LEFT JOIN dbo.[Författare] f ON b.[FörfattareId] = f.Id
    LEFT JOIN dbo.[Förlag] fl ON b.[FörlagId] = fl.Id
    WHERE l.ButikId IS NOT NULL
    ORDER BY b.Titel");

            var booksInStock = context.Database
                .SqlQuery<BookStockDetails>(stockQuery)
                .ToList();

            if (!booksInStock.Any())
            {
                Console.WriteLine("Inga böcker hittades i denna butik.");
                return;
            }

            var currentButik = "";
            foreach (var item in booksInStock)
            {
                if (currentButik != item.Butiksnamn)
                {
                    currentButik = item.Butiksnamn;
                    Console.WriteLine($"\nButik: {item.Butiksnamn} (ID: {item.ButikId})");
                }
                Console.WriteLine($"- ISBN: {item.ISBN} | {item.Titel} | {item.Författare} | Antal: {item.Antal} st");
            }

            Console.Write("\nAnge ISBN på boken: ");
            var isbn = Console.ReadLine()?.Trim() ?? "";
            while (string.IsNullOrEmpty(isbn))
            {
                Console.WriteLine("ISBN kan inte vara tomt. Försök igen:");
                isbn = Console.ReadLine()?.Trim() ?? "";
            }

            int antal;
            do
            {
                Console.Write("Hur många exemplar vill du ta bort? ");
                if (int.TryParse(Console.ReadLine(), out antal) && antal > 0)
                {
                    break;
                }
                Console.WriteLine("Ange ett giltigt antal större än 0.");
            } while (true);

            // Få nuvarande saldo innan uppdatering
            var currentStock = context.Lagersaldo
                .FirstOrDefault(l => l.ButikId == selectedStoreId && l.ISBN == isbn);

            if (currentStock != null && currentStock.Antal >= antal)
            {
                currentStock.Antal -= antal;
                context.SaveChanges();
                Console.WriteLine($"\n{antal} exemplar har tagits bort från butikens lager!");
                Console.WriteLine($"Nytt lagersaldo: {currentStock.Antal} st");
            }
            else
            {
                Console.WriteLine($"\nKunde inte ta bort böckerna. Aktuellt lager: {currentStock?.Antal ?? 0} st");
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }
        // DTO-klass för författarinformation
        public class AuthorEditDTO
        {
            public int Id { get; set; }
            public string Förnamn { get; set; }
            public string Efternamn { get; set; }
            public DateTime Födelsedatum { get; set; }
        }



        static void RedigeraFörfattare(BokhandelContext context)
        {
            Console.WriteLine("\nVälj författare att redigera:");

            var authors = context.Database.SqlQuery<AuthorListItem>($@"
        SELECT 
            Id,
            CONCAT([Förnamn], ' ', [Efternamn]) as FullName
        FROM dbo.[Författare]
        ORDER BY [Efternamn], [Förnamn]").ToList();

            foreach (var author in authors)
            {
                Console.WriteLine($"{author.Id}. {author.FullName}");
            }

            Console.Write("\nAnge författarens ID: ");
            if (!int.TryParse(Console.ReadLine(), out int authorId))
            {
                Console.WriteLine("Ogiltigt ID.");
                return;
            }

            var författare = context.Författare.FirstOrDefault(f => f.Id == authorId);
            if (författare == null)
            {
                Console.WriteLine("Författaren hittades inte.");
                return;
            }

            Console.WriteLine($"\nNuvarande information:");
            Console.WriteLine($"Förnamn: {författare.Förnamn}");
            Console.WriteLine($"Efternamn: {författare.Efternamn}");
            Console.WriteLine($"Födelsedatum: {författare.Födelsedatum:yyyy-MM-dd}");

            Console.Write("\nNytt förnamn (eller Enter för att behålla nuvarande): ");
            var nyttFörnamn = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nyttFörnamn))
                författare.Förnamn = nyttFörnamn;

            Console.Write("Nytt efternamn (eller Enter för att behålla nuvarande): ");
            var nyttEfternamn = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nyttEfternamn))
                författare.Efternamn = nyttEfternamn;

            try
            {
                context.SaveChanges();
                Console.WriteLine("Författaren har uppdaterats!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod: {ex.Message}");
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        // DTO-klass för bokradering
        public class BookDeleteDTO
        {
            public string ISBN13 { get; set; }
            public string Titel { get; set; }
            public bool HasInventory { get; set; }
        }
        static void TaBortBok(BokhandelContext context)
        {
            Console.WriteLine("\nVälj bok att ta bort:");

            var books = context.Database.SqlQuery<BookDetails>($@"
        SELECT 
            b.ISBN13, 
            b.Titel,
            b.[Språk],
            b.Pris,
            b.Utgivningsdatum,
            CONCAT(f.[Förnamn], ' ', f.[Efternamn]) as Författare,
            fl.[Namn] as Förlag
        FROM dbo.Böcker b
        LEFT JOIN dbo.[Författare] f ON b.[FörfattareId] = f.Id
        LEFT JOIN dbo.[Förlag] fl ON b.[FörlagId] = fl.Id").ToList();

            foreach (var book in books)
            {
                Console.WriteLine($"ISBN13: {book.ISBN13}");
                Console.WriteLine($"Titel: {book.Titel}");
                Console.WriteLine($"Författare: {book.Författare}");
                Console.WriteLine("------------------------");
            }

            Console.Write("\nAnge ISBN13 för boken som ska tas bort: ");
            var isbn = Console.ReadLine()?.Trim();

            try
            {
                var deleteQuery = FormattableStringFactory.Create(@"
            DELETE FROM dbo.Lagersaldo WHERE ISBN = {0};
            DELETE FROM dbo.Böcker WHERE ISBN13 = {0};", isbn);

                var rowsAffected = context.Database.ExecuteSql(deleteQuery);
                Console.WriteLine(rowsAffected > 0 ? "Boken har tagits bort!" : "Ingen bok hittades med angivet ISBN.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod: {ex.Message}");
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }
        //DTO class for author deletion
        public class AuthorDeleteDTO
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public int BookCount { get; set; }
        }
        static void TaBortFörfattare(BokhandelContext context)
        {
            Console.WriteLine("\nVälj författare att ta bort:");

            var authors = context.Database.SqlQuery<AuthorDeleteDTO>($@"
        SELECT 
            f.Id,
            CONCAT(f.[Förnamn], ' ', f.[Efternamn]) as FullName,
            COUNT(b.ISBN13) as BookCount
        FROM dbo.[Författare] f
        LEFT JOIN dbo.Böcker b ON f.Id = b.[FörfattareId]
        GROUP BY f.Id, f.[Förnamn], f.[Efternamn]
        ORDER BY FullName").ToList();

            foreach (var author in authors)
            {
                Console.WriteLine($"{author.Id}. {author.FullName} (Antal böcker: {author.BookCount})");
            }

            Console.Write("\nAnge författarens ID: ");
            if (int.TryParse(Console.ReadLine(), out int authorId))
            {
                var selectedAuthor = authors.FirstOrDefault(a => a.Id == authorId);
                if (selectedAuthor != null)
                {
                    Console.WriteLine($"\nÄr du säker på att du vill ta bort {selectedAuthor.FullName}?");
                    Console.WriteLine($"Detta kommer även ta bort {selectedAuthor.BookCount} böcker.");
                    Console.Write("Skriv 'JA' för att bekräfta: ");

                    if (Console.ReadLine()?.ToUpper() == "JA")
                    {
                        var deleteQuery = FormattableStringFactory.Create(@"
                    DELETE FROM dbo.[Författare] WHERE Id = {0}", authorId);

                        var rowsAffected = context.Database.ExecuteSql(deleteQuery);
                        Console.WriteLine(rowsAffected > 0
                            ? "Författaren och alla tillhörande böcker har tagits bort!"
                            : "Kunde inte hitta författaren.");
                    }
                }
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

    }
}
   









