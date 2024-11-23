using System;
using System.Collections.Generic;
using System.Linq;

class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Book(string title, string author, string isbn)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
    }

    public override string ToString()
    {
        return $"{Title} by {Author} (ISBN: {ISBN}) - {(IsAvailable ? "Available" : "Rented")}";
    }
}

class Reader
{
    public string Name { get; set; }
    public List<Book> BorrowedBooks { get; private set; } = new List<Book>();
    public int MaxBorrowLimit { get; set; } = 3;

    public Reader(string name)
    {
        Name = name;
    }

    public bool BorrowBook(Book book)
    {
        if (BorrowedBooks.Count >= MaxBorrowLimit)
        {
            Console.WriteLine($"{Name} cannot borrow more than {MaxBorrowLimit} books.");
            return false;
        }

        if (!book.IsAvailable)
        {
            Console.WriteLine($"Book '{book.Title}' is not available.");
            return false;
        }

        BorrowedBooks.Add(book);
        book.IsAvailable = false;
        Console.WriteLine($"{Name} borrowed '{book.Title}'.");
        return true;
    }

    public void ReturnBook(Book book)
    {
        if (BorrowedBooks.Remove(book))
        {
            book.IsAvailable = true;
            Console.WriteLine($"{Name} returned '{book.Title}'.");
        }
        else
        {
            Console.WriteLine($"{Name} does not have this book.");
        }
    }

    public void ShowBorrowedBooks()
    {
        Console.WriteLine($"{Name} has borrowed:");
        foreach (var book in BorrowedBooks)
        {
            Console.WriteLine($"  - {book.Title}");
        }
    }
}

class Librarian
{
    public string Name { get; set; }

    public Librarian(string name)
    {
        Name = name;
    }

    public void AddBook(Library library, Book book)
    {
        library.Books.Add(book);
        Console.WriteLine($"Librarian {Name} added '{book.Title}' to the library.");
    }
}

class Library
{
    public List<Book> Books { get; private set; } = new List<Book>();

    public void ShowAllBooks()
    {
        Console.WriteLine("Books in the library:");
        foreach (var book in Books)
        {
            Console.WriteLine($"  - {book}");
        }
    }

    public List<Book> SearchBooks(string query)
    {
        return Books.Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                             || b.Author.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}

class Program
{
    static void Main()
    {
        Library library = new Library();
        Librarian librarian = new Librarian("Alice");

        librarian.AddBook(library, new Book("The Catcher in the Rye", "J.D. Salinger", "9780316769488"));
        librarian.AddBook(library, new Book("To Kill a Mockingbird", "Harper Lee", "9780061120084"));
        librarian.AddBook(library, new Book("1984", "George Orwell", "9780451524935"));

        Reader reader = new Reader("John");

        library.ShowAllBooks();

        var bookToBorrow = library.Books[0];
        reader.BorrowBook(bookToBorrow);
        reader.ShowBorrowedBooks();

        library.ShowAllBooks();

        reader.ReturnBook(bookToBorrow);
        reader.ShowBorrowedBooks();

        library.ShowAllBooks();

        var searchResults = library.SearchBooks("1984");
        Console.WriteLine("Search results:");
        foreach (var book in searchResults)
        {
            Console.WriteLine($"  - {book}");
        }
    }
}
