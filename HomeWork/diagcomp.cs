using System;
using System.Collections.Generic;

#region Interfaces
public interface IHotelService
{
    List<string> SearchHotels(string location, string roomClass, decimal maxPrice);
}

public interface IBookingService
{
    bool BookRoom(string user, string hotel, DateTime startDate, DateTime endDate);
}

public interface IPaymentService
{
    bool ProcessPayment(string user, decimal amount, string method);
}

public interface INotificationService
{
    void SendNotification(string user, string message);
}

public interface IUserManagementService
{
    bool RegisterUser(string username, string password);
    bool AuthenticateUser(string username, string password);
}
#endregion

#region Service Implementations
public class HotelService : IHotelService
{
    private List<string> hotels = new List<string>
    {
        "Hotel A - New York",
        "Hotel B - Paris",
        "Hotel C - Tokyo"
    };

    public List<string> SearchHotels(string location, string roomClass, decimal maxPrice)
    {
        Console.WriteLine($"Searching hotels in {location} with room class {roomClass} under price {maxPrice}...");
        return hotels; 
    }
}

public class BookingService : IBookingService
{
    public bool BookRoom(string user, string hotel, DateTime startDate, DateTime endDate)
    {
        Console.WriteLine($"Booking room in {hotel} for {user} from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}...");
        return true; 
    }
}

public class PaymentService : IPaymentService
{
    public bool ProcessPayment(string user, decimal amount, string method)
    {
        Console.WriteLine($"Processing payment of {amount} for {user} via {method}...");
        return true; 
    }
}

public class NotificationService : INotificationService
{
    public void SendNotification(string user, string message)
    {
        Console.WriteLine($"Sending notification to {user}: {message}");
    }
}

public class UserManagementService : IUserManagementService
{
    private Dictionary<string, string> users = new Dictionary<string, string>();

    public bool RegisterUser(string username, string password)
    {
        if (users.ContainsKey(username)) return false;
        users[username] = password;
        Console.WriteLine($"User {username} registered successfully.");
        return true;
    }

    public bool AuthenticateUser(string username, string password)
    {
        return users.ContainsKey(username) && users[username] == password;
    }
}
#endregion

#region Main Program
class Program
{
    static void Main()
    {
        IUserManagementService userService = new UserManagementService();
        IHotelService hotelService = new HotelService();
        IBookingService bookingService = new BookingService();
        IPaymentService paymentService = new PaymentService();
        INotificationService notificationService = new NotificationService();

        Console.WriteLine("Welcome to the Hotel Booking System!");

        
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter password: ");
        string password = Console.ReadLine();
        userService.RegisterUser(username, password);

        if (userService.AuthenticateUser(username, password))
        {
            Console.WriteLine("Login successful!");

            
            Console.Write("Enter location: ");
            string location = Console.ReadLine();
            var hotels = hotelService.SearchHotels(location, "Standard", 200);
            Console.WriteLine("Available hotels:");
            foreach (var hotel in hotels)
            {
                Console.WriteLine($"- {hotel}");
            }

            
            Console.Write("Enter hotel name to book: ");
            string hotelName = Console.ReadLine();
            Console.Write("Enter check-in date (yyyy-mm-dd): ");
            DateTime checkIn = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter check-out date (yyyy-mm-dd): ");
            DateTime checkOut = DateTime.Parse(Console.ReadLine());

            if (bookingService.BookRoom(username, hotelName, checkIn, checkOut))
            {
                Console.WriteLine("Booking successful!");
                paymentService.ProcessPayment(username, 150, "Credit Card");
                notificationService.SendNotification(username, "Booking confirmed!");
            }
        }
        else
        {
            Console.WriteLine("Login failed.");
        }
    }
}
#endregion
