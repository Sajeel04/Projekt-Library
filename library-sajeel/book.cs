using System;
using System.Collections.Generic;
using System.Text;

namespace project_book
{
    class Book
    {
        public string title;
        public string author;
        public string year;
        public int quantity;
        public string isbn;
        public string genre;

        public Book(string title, string author, string year, int quantity, string isbn, string genre)
        {
            this.title = title;
            this.author = author;
            this.year = year;
            this.quantity = quantity;
            this.isbn = isbn;
            this.genre = genre;
        }
    }
}