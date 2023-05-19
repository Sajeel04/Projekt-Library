using System;
using System.Collections.Generic;
using project_book;
using project_data;
using System.IO;

namespace project_library
{
    class Library
    {
        data d = new data();
        public bool reserve = false;

        public void addBooks(string title, string author, string year, int quantity, string isbn, string genre, bool checkReservation = true)
        {
            bool duplicate = false;
            int idx = 0;
            string[] books = d.getBookIterator();
            Book book = null;
            // Kolla om bok med samma ISBN redan existerar.
            for (int i = 0; i < books.Length; i++)
            {
                string[] bookValues = books[i].Split(' ');
                if (bookValues.Length == 6)
                {
                    book = new Book(bookValues[0], bookValues[1], bookValues[2], int.Parse(bookValues[3]), bookValues[4], bookValues[5]);
                    if (book.isbn == isbn)
                    {
                        duplicate = true;
                        idx = i;
                        break;
                    }
                }
            }

            // Om boken inte redan finns läggs den till i listan med alla böcker samt i lista med reserverade böcker. 
            if (!duplicate)
            {
                string[] lines =
                {
                    $"{title} {author} {year} {quantity} {isbn} {genre}"
                };
                string[] reserveLines =
                {
                    $"{isbn}"
                };

                d.writeToBookFile(lines, true);
                d.writeToReserveFile(reserveLines, true);
            }
            // Om boken redan finns i listan med böcker så ökas endast antalet. 
            if (duplicate)
            {
                books[idx] = $"{book.title} {book.author} {book.year} {book.quantity + quantity} {book.isbn} {book.genre}";
                d.writeToBookFile(books, false);
                // Kollar om boken reserverats av användare och lånar då boken automatiskt.
                if (checkReservation)
                {
                    checkReservedBooks(book);
                }
            }
        }

        public Book getBook(string isbn)
        {
            foreach (var bookLine in d.getBookIterator())
            {
                string[] bookValues = bookLine.Split(' ');
                Book book = new Book(bookValues[0], bookValues[1], bookValues[2], int.Parse(bookValues[3]), bookValues[4], bookValues[5]);
                if (book.isbn == isbn)
                {
                    if (book.quantity >= 0)
                    {
                        return new Book(book.title, book.author, book.year, book.quantity, book.isbn, book.genre);
                    }
                }
            }

            return new Book("error", "error", "error", -1, "error", "error");
        }

        public bool loanBook(string isbn, string personnummer)
        {
            reserve = false;
            Book book = getBook(isbn);
            // Om bok är slut ska användare ha möjlighet att reservera.
            if (book.quantity == 0)
            {
                reserve = true;
                return false;
            }
            if (book.quantity < 0)
            {
                Console.WriteLine("Boken finns inte");
                return false;
            }
            else
            {
                string[] bookLines = d.getLoanIterator();
                for (int i = 0; i < bookLines.Length; i++)
                {
                    if (bookLines[i].Split()[0] == personnummer)
                    {
                        if (bookLines[i].Contains(book.isbn))
                        {
                            Console.WriteLine("Du lånar redan denna bok.");
                        }
                        else
                        {
                            bookLines[i] += $" {book.isbn}";
                            d.writeToLoanFile(bookLines, false);
                            // Minska kvantitet av bok med 1. 
                            addBooks(book.title.Replace(' ', '-'), book.author.Replace(' ', '-'), book.year, -1, book.isbn, book.genre.Replace(' ', '-'), false);
                            return true;
                        }

                        return false;
                    }
                }
            }

            return false;
        }


        public bool reserveBook(string isbn, string personnummer)
        {
            Book book = getBook(isbn);
            if (book.quantity < 0)
            {
                return false;
            }
            else
            {
                string[] bookLines = d.getReserveIterator();
                for (int i = 0; i < bookLines.Length; i++)
                {
                    string[] line = bookLines[i].Split(' ');
                    if (line[0] == book.isbn)
                    {
                        // Kolla om användare redan reserverar bok.
                        foreach (var user in line)
                        {
                            if (user == personnummer)
                            {
                                Console.WriteLine($"Du reserverar redan {book.title}.");
                                return false;
                            }
                        }
                        // Om reservering är möjlig läggs personnummmer till i kön.
                        bookLines[i] += $" {personnummer}";
                        d.writeToReserveFile(bookLines, false);
                        return true;
                    }
                }
            }

            return false;
        }

        // Funktion för att kolla om användare reserverat böcker. När nya böcker läggs till kommer denna användas för att automatiskt ge böcker till de som reserverat.
        public void checkReservedBooks(Book book)
        {
            string[] bookLines = d.getReserveIterator();
            for (int i = 0; i < bookLines.Length; i++)
            {
                string[] splitLine = bookLines[i].Split(' ');
                // Kolla om rad är korrekt bok som lagts till.
                if (splitLine[0] == book.isbn)
                {
                    List<string> personnummer = new List<string>();
                    for (int j = 1; j < splitLine.Length; j++)
                    {
                        bool loanReservedBook = loanBook(book.isbn, splitLine[j]);
                        // Om lånet inte gick igenom d.v.s false är boken slut igen.
                        if (!loanReservedBook)
                        {
                            break;
                        }
                        personnummer.Add(splitLine[j]);
                    }
                    bookLines[i] = book.isbn;
                    for (int j = 1; j < splitLine.Length; j++)
                    {
                        if (!personnummer.Contains(splitLine[j]))
                        {
                            bookLines[i] += $" {splitLine[j]}";
                        }
                    }
                    d.writeToReserveFile(bookLines, false);

                    break;
                }
            }
        }

        public void returnBook(string isbn, string personnummer)
        {
            string[] bookLines = d.getLoanIterator();
            for (int i = 0; i < bookLines.Length; i++)
            {
                if (bookLines[i].Split()[0] == personnummer)
                {
                    string[] loanedBooks = bookLines[i].Split(' ');
                    bookLines[i] = loanedBooks[0];
                    for (int j = 1; j < loanedBooks.Length; j++)
                    {
                        if (loanedBooks[j] != isbn)
                        {
                            bookLines[i] += $" {loanedBooks[j]}";
                        }
                    }
                    d.writeToLoanFile(bookLines, false);
                    Book book = getBook(isbn);
                    if (book.quantity != -1)
                    {
                        addBooks(book.title, book.author, book.year, 1, book.isbn, book.genre);
                    }
                }
            }
        }

        public List<Book> listBooks(string keyword)
        {
            var books = new List<Book>();
            foreach (var bookLine in d.getBookIterator())
            {
                foreach (var word in keyword.Split(' '))
                {
                    string[] bookValues = bookLine.Split(' ');
                    Book book = new Book(bookValues[0], bookValues[1], bookValues[2], int.Parse(bookValues[3]), bookValues[4], bookValues[5]);
                    if (bookLine.ToLower().Contains(word.ToLower()))
                    {
                        books.Add(book);
                        break;
                    }
                }
            }

            return books;
        }

        public List<Book> listLoanedBooks(string personnummer)
        {
            List<Book> loanedBooks = new List<Book>();
            foreach (var line in d.getLoanIterator())
            {
                string[] l = line.Split(" ");
                if (l[0] == personnummer)
                {
                    for (int i = 1; i < l.Length; i++)
                    {
                        Book book = getBook(l[i]);
                        loanedBooks.Add(book);
                    }
                    break;
                }
            }

            return loanedBooks;
        }

        public void removeBook(string isbn) 
        {
            string[] books = d.getBookIterator();
            for (int i = 0; i < books.Length; i++)
            {
                string[] credentials = books[i].Split(' ');
                if (credentials[4] == isbn)
                {
                    books[i] = "";
                    var foos = new List<string>(books);
                    foos.RemoveAt(i);
                    d.writeToBookFile(foos.ToArray(), false);
                }
            }


            string[] loanFile = d.getLoanIterator();
            for (int i = 0; i < loanFile.Length; i++)
            {
                string[] line = loanFile[i].Split(' ');
                loanFile[i] = line[0];
                for (int j = 1; j < line.Length; j++)
                {
                    if (line[j] != isbn)
                    {
                        loanFile[i] += $" {line[j]}";
                    }
                }
            }

            d.writeToLoanFile(loanFile, false);

            // Ta bort i fil för reserveringar.
            string[] reserveFile = d.getReserveIterator();
            for (int i = 0; i < reserveFile.Length; i++)
            {
                string[] line = reserveFile[i].Split(' ');
                if (line[0] == isbn)
                {
                    reserveFile[i] = "";
                    var foos = new List<string>(reserveFile);
                    foos.RemoveAt(i);
                    d.writeToReserveFile(foos.ToArray(), false);
                }
            }
        }

        public void editBook(string isbn, string[] newCredentials)
        {
            string[] books = d.getBookIterator();
            bool edited = false;
            bool isbnChange = true;
            for (int i = 0; i < books.Length; i++)
            {
                string[] credentials = books[i].Split(' ');
                if (credentials[4] == isbn)
                {
                    for (int j = 0; j < newCredentials.Length; j++)
                    {
                        if (newCredentials[j] == " ")
                        {
                            // Kolla om isbn inte ändras.
                            if (j == 3)
                            {
                                isbnChange = false;
                            }

                            if (j > 2) { newCredentials[j] = credentials[j + 1]; }

                            else { newCredentials[j] = credentials[j]; }
                        }
                    }

                    books[i] = $"{newCredentials[0].Replace(' ', '-')} {newCredentials[1].Replace(' ', '-')} {newCredentials[2]} {credentials[3]} {newCredentials[3].Replace(' ', '-')} {newCredentials[4]}";
                    d.writeToBookFile(books, false);

                    edited = true;
                    break;
                }
            }
            
            

            if (!edited)
            {
                Console.WriteLine($"Kunde inte hitta bok med isbn {isbn}");
            }

            else
            {
                // Om isbn ändras ska det även ändras i fil för lånade böcker samt fil för reserveringar.
                if (isbnChange)
                {
                    // Ändra i fil för lånade böcker.
                    string[] loanFile = d.getLoanIterator();
                    for (int i = 0; i < loanFile.Length; i++)
                    {
                        string[] line = loanFile[i].Split(' ');
                        loanFile[i] = line[0];
                        for (int j = 1; j < line.Length; j++)
                        {
                            if (line[j] != isbn)
                            {
                                loanFile[i] += $" {line[j]}";
                            }
                            else
                            {
                                loanFile[i] += $" {newCredentials[3]}";
                            }
                        }
                    }

                    d.writeToLoanFile(loanFile, false);

                    // Ändra i fil för reserveringar.
                    string[] reserveFile = d.getReserveIterator();
                    for (int i = 0; i < reserveFile.Length; i++)
                    {
                        string[] line = reserveFile[i].Split(' ');
                        if (line[0] == isbn)
                        {
                            // Uppdatera isbn
                            reserveFile[i] = newCredentials[3];
                            for (int j = 1; j < line.Length; j++)
                            {
                                reserveFile[i] += $" {line[j]}";
                            }
                            break;
                        }
                    }
                    d.writeToReserveFile(reserveFile, false);
                }
            }
        }
    }
}
