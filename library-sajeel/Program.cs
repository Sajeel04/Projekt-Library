using System;
using project_data;
using project_user;
using project_library;


namespace oop
{
    class Program
    {
        static void Main(string[] args)
        {
            int option = 9;
            string[] adminOptions = new string[] { "lägg till konto", "ta bort konto", "redigera användare", "lista användare", "lägg till bok", "ta bort bok", "redigera bok", "sök efter bok", "logga ut" };
            string[] memberOptioms = new string[] { "låna", "returnera", "lista lånade böcker", "ändra lösenord", "sök efter bok", "logga ut", " ", " ", " " };
            string[] options = new string[option];

            Library lib = new Library();

            while (true)
            {
                Console.WriteLine("Bibliotek\n");
                Member user = null;
                Librarian librarian = null;
                bool result = false;

                while (!result)
                {
                    Console.Write("Vill du logga in som medlem tryck (1) eller bibliotekarie(2): ");
                    int userSort = makeChoice();
                    bool createAccount = false;
                    Console.Write("Vill du skapa nytt konto tryck (1) eller logga in tryck (2)? ");
                    int newAccount = makeChoice();
                    string firstname = "";
                    string lastname = "";

                    if (newAccount == 1)
                    {
                        Console.Write("Skriv´förnamn: ");
                        firstname = Console.ReadLine();
                        Console.Write("Skriv efternamn: ");
                        lastname = Console.ReadLine();
                        createAccount = true;
                    }
                    Console.Write("Skriv personnummer DDMMYYY-XXX: ");
                    string username = Console.ReadLine();
                    Console.Write("Skriv in lösenordet: ");
                    string key = Console.ReadLine();
                    Console.Clear();
                    if (userSort == 1)
                    {
                        user = new Member(username, key, firstname, lastname, createAccount);
                        options = memberOptioms;
                        result = user.success;
                    }
                    else
                    {
                        
                        librarian = new Librarian(username, key, firstname, lastname, createAccount);
                        options = adminOptions;
                        result = librarian.success;
                    }
                }

                int Option1 = 0;
                while (options[Option1] != "logga ut")
                {
                    Console.Write("\nTryck enter för att gå till meny!");
                    string exit = Console.ReadLine();
                    Console.Clear();
                    for (int i = 0; i < option; i++)
                    {
                        if (options[i] != " ")
                        {
                            Console.WriteLine($"{i + 1} - {options[i]}");
                        }
                    }
                    Console.Write("välj: ");
                    Option1 = makeChoice() - 1;

                    if (options[Option1] == "låna")
                    {
                        Console.Write("Sökord: ");
                        string keyword = Console.ReadLine();
                        var books = lib.listBooks(keyword);
                        if (books.Count > 0)
                        {
                            Console.WriteLine("Alla böcker:");
                            for (int i = 0; i < books.Count; i++)
                            {
                                Console.WriteLine($"{i + 1} - {books[i].title.Replace('-', ' ')} - {books[i].author.Replace('-', ' ')} ({books[i].year})");
                            }
                            Console.WriteLine("Vilken bok vill du låna?");
                            int bookIdx = makeChoice() - 1;
                            bool loanSuccess = lib.loanBook(books[bookIdx].isbn, user.personnummer);
                            if (lib.reserve)
                            {
                                Console.Write("Boken finns inte. Vill du reservera boken?");
                                string reserveChoice = Console.ReadLine();
                                if (reserveChoice == "ja")
                                {
                                    bool reserveSuccess = lib.reserveBook(books[bookIdx].isbn, user.personnummer);
                                    if (reserveSuccess)
                                    {
                                        Console.WriteLine($"Den är reserverad nu {books[bookIdx].title}");
                                    }
                                }
                            }

                            if (loanSuccess)
                            {
                                Console.WriteLine($"Du har lånat boken {books[bookIdx].title.Replace('-', ' ')}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Hittade inga böcker med det angivna sökordet");
                        }
                    }

                    if (options[Option1] == "Returnera")
                    {

                        var books = lib.listLoanedBooks(user.personnummer);
                        if (books.Count == 0)
                        {
                            Console.WriteLine("Du har inga lånade böcker!");
                        }
                        else
                        {
                            Console.WriteLine("Dina lånade böcker:");
                            for (int i = 0; i < books.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {books[i].title.Replace('-', ' ')} ({books[i].year})");
                            }

                            Console.WriteLine("Vilken bok vill du returnera?");

                            int bookChoice = makeChoice() - 1;
                            lib.returnBook(books[bookChoice].isbn, user.personnummer);
                        }
                    }

                    if (options[Option1] == "ändra lösenord")
                    {
                        Console.Write("Skriv in nytt lösenord: ");
                        string newPassword = Console.ReadLine();
                        user.changePassword(newPassword);

                    }

                    if (options[Option1] == "lista lånade böcker")
                    {
                        var books = lib.listLoanedBooks(user.personnummer);
                        Console.WriteLine("Dina lånade böcker:");
                        if (books.Count == 0)
                        {
                            Console.WriteLine("Du har inga lånade böcker!");
                        }
                        for (int i = 0; i < books.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} {books[i].title.Replace('-', ' ')} ({books[i].year})");
                        }
                    }

                    if (options[Option1] == "sök efter bok")
                    {
                        Console.Write("Skriv in sökord: ");
                        string keyword = Console.ReadLine();
                        var books = lib.listBooks(keyword);
                        if (books.Count > 0)
                        {
                            Console.WriteLine("Alla böcker:");
                            for (int i = 0; i < books.Count; i++)
                            {
                                Console.WriteLine($"{i + 1} - {books[i].title.Replace('-', ' ')} - {books[i].author.Replace('-', ' ')} ({books[i].year})");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Hittade inga böcker med det angivna sökordet");
                        }
                    }

                    if (options[Option1] == "ta bort konto")
                    {
                        Console.WriteLine("Alla användare");
                        var users = librarian.listUsers();
                        Console.WriteLine("Vilken användare vill du ta bort?");
                        int userIdx = makeChoice() - 1;
                        librarian.removeUser(users[userIdx]);
                    }

                    if (options[Option1] == "lägg till konto")
                    {
                        Console.Write("Skriv in personnummer: ");
                        string personnummer = Console.ReadLine();
                        Console.Write("Skriv in lösenord: ");
                        string password = Console.ReadLine();
                        Console.Write("Skriv in namn: ");
                        string firstname = Console.ReadLine();
                        Console.Write("Skriv in efternamn: ");
                        string lastname = Console.ReadLine();
                        Member addUser = new Member(personnummer, password, firstname, lastname, true);
                    }
                    if (options[Option1] == "redigera användare")
                    {
                        Console.WriteLine("\nAlla personnummer:");
                        var users = librarian.listUsers();
                        Console.Write("\nVälj användare du vill ändra uppgifter på: ");
                        int idx = makeChoice() - 1;
                        string personnummerToEdit = users[idx];
                        int editIdx = 0;
                        string[] editChoice = { "personnummer", "lösenord", "namn", "efternamn", "avbryt" };
                        while (true)
                        {
                            string[] newCredits = { " ", " ", " ", " " };
                            for (int i = 0; i < editChoice.Length; i++)
                            {
                                Console.WriteLine($"{i + 1} - {editChoice[i]}");
                            }

                            Console.Write("Skriv in val: ");
                            editIdx = makeChoice() - 1;
                            if (editChoice[editIdx] == "avbryt")
                            {
                                break;
                            }

                            Console.Write($"Skriv in nytt {editChoice[editIdx]}: ");
                            newCredits[editIdx] = Console.ReadLine();
                            librarian.editMember(personnummerToEdit, newCredits);
                            if (editIdx == 0)
                            {
                                personnummerToEdit = newCredits[0];
                            }
                        }
                    }

                    if (options[Option1] == "lista användare")
                    {
                        Console.WriteLine("Alla användare");
                        var users = librarian.listUsers();
                    }

                    if (options[Option1] == "lägg till bok")
                    {
                        Console.Write("Lägger till bok\nSkriv in titel: ");
                        string title = Console.ReadLine().Replace(' ', '-');
                        Console.Write("Skriv in författare: ");
                        string author = Console.ReadLine().Replace(' ', '-');
                        Console.Write("Skriv in år: ");
                        string year = Console.ReadLine();
                        Console.Write("Skriv in mängd: ");
                        int quantity = int.Parse(Console.ReadLine());
                        Console.Write("Skriv in ISBN: ");
                        string isbn = Console.ReadLine();
                        Console.Write("Skriv in genre: ");
                        string genre = Console.ReadLine();
                        lib.addBooks(title, author, year, quantity, isbn, genre);
                    }

                    if (options[Option1] == "redigera bok")
                    {
                        Console.Write("Skriv in sökord: ");
                        string keyword = Console.ReadLine();
                        var books = lib.listBooks(keyword);
                        Console.WriteLine("Alla böcker:");
                        for (int i = 0; i < books.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} - {books[i].title.Replace('-', ' ')} - {books[i].author.Replace('-', ' ')} ({books[i].year})");
                        }
                        Console.Write("\nVälj bok du vill ändra uppgifter på: ");
                        int idx = makeChoice() - 1;
                        string isbnToEdit = books[idx].isbn;
                        int editIdx = 0;
                        string[] editChoice = { "titel", "författare", "år", "isbn", "genre", "avbryt" };
                        while (true)
                        {
                            string[] newCredits = { " ", " ", " ", " ", " " };
                            for (int i = 0; i < editChoice.Length; i++)
                            {
                                Console.WriteLine($"{i + 1} - {editChoice[i]}");
                            }

                            Console.Write("Skriv in val: ");
                            editIdx = makeChoice() - 1;
                            if (editChoice[editIdx] == "avbryt")
                            {
                                break;
                            }

                            Console.Write($"Skriv in ny {editChoice[editIdx]}: ");
                            newCredits[editIdx] = Console.ReadLine();
                            lib.editBook(isbnToEdit, newCredits);
                            if (editIdx == 3)
                            {
                                isbnToEdit = newCredits[3];
                            }
                        }
                    }

                    if(options[Option1] == "ta bort bok") 
                    {
                        Console.Write("Skriv in sökord: ");
                        string keyword = Console.ReadLine();
                        var books = lib.listBooks(keyword);
                        Console.WriteLine("Alla böcker:");
                        for (int i = 0; i < books.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} - {books[i].title.Replace('-', ' ')} - {books[i].author.Replace('-', ' ')} ({books[i].year})");
                        }
                        Console.Write("\nVälj bok du vill ändra uppgifter på: ");
                        int idx = makeChoice() - 1;
                        string isbnToRemove = books[idx].isbn;
                        lib.removeBook(isbnToRemove);
                        Console.WriteLine($"Tog bort bok {books[idx].title}");
                    }
                }

                Console.WriteLine("Utloggad!");
            }
        }

        static int makeChoice()
        {
            int option = 0;
            try
            {
                option = int.Parse(Console.ReadLine());
            }

            catch
            {
                Console.Write("Otillåtet val. Skriv in val igen: ");
                option = makeChoice();
            }

            return option;
        }
    }
}