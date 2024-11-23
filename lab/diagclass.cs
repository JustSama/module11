using System;
using System.Collections.Generic;

#region Classes

public class Book
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string ISBN { get; private set; }
    public bool IsAvailable { get; private set; }

    public Book(string title, string author, string isbn)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        IsAvailable = true;
    }

    public void MarkAsLoaned()
    {
        IsAvailable = false;
    }

    public void MarkAsAvailable()
    {
        IsAvailable = true;
    }

    public override string ToString()
    {
        return $"{Title} by {Author} (ISBN: {ISBN}) - {(IsAvailable ? "Available" : "Loaned")}";
    }
}

public class Reader
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public List<Book> BorrowedBooks { get; private set; }

    public Reader(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
        BorrowedBooks = new List<Book>();
    }

    public void BorrowBook(Book book)
    {
        if (book.IsAvailable)
        {
            book.MarkAsLoaned();
            BorrowedBooks.Add(book);
            Console.WriteLine($"{Name} borrowed the book: {book.Title}");
        }
        else
        {
            Console.WriteLine($"The book {book.Title} is not available.");
        }
    }

    public void ReturnBook(Book book)
    {
        if (BorrowedBooks.Contains(book))
        {
            book.MarkAsAvailable();
            BorrowedBooks.Remove(book);
            Console.WriteLine($"{Name} returned the book: {book.Title}");
        }
        else
        {
            Console.WriteLine($"{Name} does not have the book: {book.Title}");
        }
    }

    public override string ToString()
    {
        return $"{Name} (ID: {Id}, Email: {Email})";
    }
}

public class Librarian
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Position { get; private set; }
    private List<Book> Books;

    public Librarian(int id, string name, string position)
    {
        Id = id;
        Name = name;
        Position = position;
        Books = new List<Book>();
    }

    public void AddBook(Book book)
    {
        Books.Add(book);
        Console.WriteLine($"Librarian {Name} added the book: {book.Title}");
    }

    public void RemoveBook(Book book)
    {
        if (Books.Remove(book))
        {
            Console.WriteLine($"Librarian {Name} removed the book: {book.Title}");
        }
        else
        {
            Console.WriteLine($"The book {book.Title} was not found.");
        }
    }

    public List<Book> GetBooks()
    {
        return Books;
    }

    public override string ToString()
    {
        return $"{Name} (ID: {Id}, Position: {Position})";
    }
}


public class Loan
{
    public Book Book { get; private set; }
    public Reader Reader { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    public Loan(Book book, Reader reader)
    {
        Book = book;
        Reader = reader;
        LoanDate = DateTime.Now;
    }

    public void CompleteLoan()
    {
        ReturnDate = DateTime.Now;
        Book.MarkAsAvailable();
    }

    public override string ToString()
    {
        return $"{Reader.Name} borrowed {Book.Title} on {LoanDate.ToShortDateString()}" +
               (ReturnDate.HasValue ? $", returned on {ReturnDate.Value.ToShortDateString()}" : ", not returned yet");
    }
}

#endregion

#region Main Program

class Program
{
    static void Main(string[] args)
    {
        Librarian librarian = new Librarian(1, "Alice", "Head Librarian");
        Reader reader = new Reader(1, "Bob", "bob@example.com");

        Book book1 = new Book("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565");
        Book book2 = new Book("1984", "George Orwell", "9780451524935");

        librarian.AddBook(book1);
        librarian.AddBook(book2);

        Console.WriteLine("\nBooks in Library:");
        foreach (var book in librarian.GetBooks())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nBorrowing a Book:");
        reader.BorrowBook(book1);

        Console.WriteLine("\nBooks in Library After Borrowing:");
        foreach (var book in librarian.GetBooks())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nReturning a Book:");
        reader.ReturnBook(book1);

        Console.WriteLine("\nBooks in Library After Returning:");
        foreach (var book in librarian.GetBooks())
        {
            Console.WriteLine(book);
        }
    }
}

#endregion
