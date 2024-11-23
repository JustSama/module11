using System;
using System.Collections.Generic;
using System.Linq;

#region Models

public class Book
{
    public string Title { get; set; }
    public string ISBN { get; set; }
    public List<Author> Authors { get; set; }
    public int PublicationYear { get; set; }
    public bool IsAvailable { get; private set; } = true;

    public void ChangeAvailabilityStatus(bool status)
    {
        IsAvailable = status;
    }

    public string GetBookInfo()
    {
        return $"Title: {Title}, ISBN: {ISBN}, Available: {IsAvailable}";
    }
}

public class Author
{
    public string Name { get; set; }
    public string Biography { get; set; }
}

public abstract class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public abstract void DisplayInfo();
}

public class Reader : User
{
    public List<Loan> ActiveLoans { get; set; } = new List<Loan>();

    public void BorrowBook(Book book, Library library)
    {
        library.IssueLoan(this, book);
    }

    public void ReturnBook(Book book, Library library)
    {
        library.CompleteLoan(this, book);
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Reader: {Name}, Email: {Email}");
    }
}

public class Librarian : User
{
    public void AddBook(Book book, Library library)
    {
        library.AddBook(book);
    }

    public void RemoveBook(string isbn, Library library)
    {
        library.RemoveBook(isbn);
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Librarian: {Name}, Email: {Email}");
    }
}

public class Loan
{
    public Book Book { get; set; }
    public Reader Reader { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public void IssueLoan()
    {
        LoanDate = DateTime.Now;
        ReturnDate = null;
        Book.ChangeAvailabilityStatus(false);
    }

    public void CompleteLoan()
    {
        ReturnDate = DateTime.Now;
        Book.ChangeAvailabilityStatus(true);
    }
}

#endregion

#region Library

public class Library
{
    private List<Book> _books = new List<Book>();
    private List<Loan> _loans = new List<Loan>();

    public void AddBook(Book book)
    {
        _books.Add(book);
    }

    public void RemoveBook(string isbn)
    {
        var book = _books.FirstOrDefault(b => b.ISBN == isbn);
        if (book != null)
        {
            _books.Remove(book);
        }
    }

    public void IssueLoan(Reader reader, Book book)
    {
        if (!book.IsAvailable)
        {
            throw new Exception("Book is not available.");
        }

        var loan = new Loan { Book = book, Reader = reader };
        loan.IssueLoan();
        _loans.Add(loan);
        reader.ActiveLoans.Add(loan);
    }

    public void CompleteLoan(Reader reader, Book book)
    {
        var loan = _loans.FirstOrDefault(l => l.Book == book && l.Reader == reader && l.ReturnDate == null);
        if (loan != null)
        {
            loan.CompleteLoan();
            reader.ActiveLoans.Remove(loan);
        }
    }

    public List<Book> SearchBooks(string query)
    {
        return _books.Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public void GenerateReport()
    {
        Console.WriteLine("Library Report:");
        Console.WriteLine("Books:");
        foreach (var book in _books)
        {
            Console.WriteLine(book.GetBookInfo());
        }

        Console.WriteLine("\nActive Loans:");
        foreach (var loan in _loans.Where(l => l.ReturnDate == null))
        {
            Console.WriteLine($"Book: {loan.Book.Title}, Borrowed by: {loan.Reader.Name}");
        }
    }
}

#endregion

#region Main

class Program
{
    static void Main()
    {
        var library = new Library();

        var librarian = new Librarian { Id = 1, Name = "John", Email = "john@example.com" };
        var reader = new Reader { Id = 2, Name = "Alice", Email = "alice@example.com" };

        var book1 = new Book { Title = "C# Programming", ISBN = "12345", Authors = new List<Author> { new Author { Name = "Author A" } }, PublicationYear = 2020 };
        var book2 = new Book { Title = "Introduction to Algorithms", ISBN = "67890", Authors = new List<Author> { new Author { Name = "Author B" } }, PublicationYear = 2015 };

        librarian.AddBook(book1, library);
        librarian.AddBook(book2, library);

        reader.BorrowBook(book1, library);

        library.GenerateReport();

        reader.ReturnBook(book1, library);

        library.GenerateReport();
    }
}

#endregion
