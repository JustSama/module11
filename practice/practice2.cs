using System;
using System.Collections.Generic;
using System.Linq;

#region Models

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string ISBN { get; set; }
    public bool IsAvailable { get; private set; } = true;

    public void ChangeAvailability(bool status)
    {
        IsAvailable = status;
    }

    public override string ToString()
    {
        return $"Title: {Title}, Author: {Author}, Genre: {Genre}, ISBN: {ISBN}, Available: {IsAvailable}";
    }
}

public class Reader
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string TicketNumber { get; set; }

    public override string ToString()
    {
        return $"Reader: {FirstName} {LastName}, Ticket: {TicketNumber}";
    }
}

#endregion

#region Components

public interface ICatalog
{
    List<Book> SearchByTitle(string title);
    List<Book> SearchByAuthor(string author);
    List<Book> SearchByGenre(string genre);
    void AddBook(Book book);
}

public class Catalog : ICatalog
{
    private readonly List<Book> _books = new();

    public List<Book> SearchByTitle(string title)
    {
        return _books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Book> SearchByAuthor(string author)
    {
        return _books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Book> SearchByGenre(string genre)
    {
        return _books.Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public void AddBook(Book book)
    {
        _books.Add(book);
    }

    public List<Book> GetAllBooks() => _books;
}

public interface IAccountingSystem
{
    void RecordLoan(Reader reader, Book book);
    void RecordReturn(Reader reader, Book book);
    List<string> GetLoanHistory();
}

public class AccountingSystem : IAccountingSystem
{
    private readonly List<string> _history = new();

    public void RecordLoan(Reader reader, Book book)
    {
        _history.Add($"{DateTime.Now}: {reader} borrowed \"{book.Title}\"");
    }

    public void RecordReturn(Reader reader, Book book)
    {
        _history.Add($"{DateTime.Now}: {reader} returned \"{book.Title}\"");
    }

    public List<string> GetLoanHistory()
    {
        return _history;
    }
}

public class Librarian
{
    private readonly ICatalog _catalog;
    private readonly IAccountingSystem _accountingSystem;

    public Librarian(ICatalog catalog, IAccountingSystem accountingSystem)
    {
        _catalog = catalog;
        _accountingSystem = accountingSystem;
    }

    public void IssueBook(Reader reader, string isbn)
    {
        var book = _catalog.GetAllBooks().FirstOrDefault(b => b.ISBN == isbn && b.IsAvailable);
        if (book == null)
        {
            Console.WriteLine("Book is not available.");
            return;
        }

        book.ChangeAvailability(false);
        _accountingSystem.RecordLoan(reader, book);
        Console.WriteLine($"Book \"{book.Title}\" issued to {reader.FirstName}.");
    }

    public void ReturnBook(Reader reader, string isbn)
    {
        var book = _catalog.GetAllBooks().FirstOrDefault(b => b.ISBN == isbn && !b.IsAvailable);
        if (book == null)
        {
            Console.WriteLine("Book not found or already available.");
            return;
        }

        book.ChangeAvailability(true);
        _accountingSystem.RecordReturn(reader, book);
        Console.WriteLine($"Book \"{book.Title}\" returned by {reader.FirstName}.");
    }

    public void AddBook(Book book)
    {
        _catalog.AddBook(book);
        Console.WriteLine($"Book \"{book.Title}\" added to the catalog.");
    }
}

#endregion

#region Main Program

class Program
{
    static void Main()
    {
        var catalog = new Catalog();
        var accountingSystem = new AccountingSystem();
        var librarian = new Librarian(catalog, accountingSystem);


        librarian.AddBook(new Book { Title = "1984", Author = "George Orwell", Genre = "Dystopian", ISBN = "12345" });
        librarian.AddBook(new Book { Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", ISBN = "67890" });


        var reader = new Reader { FirstName = "Alice", LastName = "Smith", TicketNumber = "A001" };


        librarian.IssueBook(reader, "12345");

        var books = catalog.SearchByGenre("Fiction");
        Console.WriteLine("Books in Fiction genre:");
        foreach (var book in books)
        {
            Console.WriteLine(book);
        }


        librarian.ReturnBook(reader, "12345");

        Console.WriteLine("Loan history:");
        foreach (var entry in accountingSystem.GetLoanHistory())
        {
            Console.WriteLine(entry);
        }
    }
}

#endregion
