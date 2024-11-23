using System;
using System.Collections.Generic;
using System.Linq;

#region Interfaces

public interface IUserService
{
    User Register(string username, string password);
    User Login(string username, string password);
}

public interface IProductService
{
    List<Product> GetProducts();
    Product AddProduct(Product product);
}

public interface IOrderService
{
    Order CreateOrder(int userId, List<int> productIds);
    Order GetOrderStatus(int orderId);
}

public interface IPaymentService
{
    bool ProcessPayment(int orderId, decimal amount);
}

public interface INotificationService
{
    void SendNotification(int userId, string message);
}

#endregion

#region Models

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<Product> Products { get; set; }
    public string Status { get; set; }
}

#endregion

#region Services

public class UserService : IUserService
{
    private List<User> _users = new List<User>();
    private int _nextId = 1;

    public User Register(string username, string password)
    {
        var user = new User { Id = _nextId++, Username = username, Password = password };
        _users.Add(user);
        return user;
    }

    public User Login(string username, string password)
    {
        return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }
}

public class ProductService : IProductService
{
    private List<Product> _products = new List<Product>();
    private int _nextId = 1;

    public List<Product> GetProducts()
    {
        return _products;
    }

    public Product AddProduct(Product product)
    {
        product.Id = _nextId++;
        _products.Add(product);
        return product;
    }
}

public class PaymentService : IPaymentService
{
    public bool ProcessPayment(int orderId, decimal amount)
    {
        Console.WriteLine($"Processing payment for Order {orderId}. Amount: {amount:C}");
        return true; 
    }
}

public class NotificationService : INotificationService
{
    public void SendNotification(int userId, string message)
    {
        Console.WriteLine($"Notification sent to User {userId}: {message}");
    }
}

public class OrderService : IOrderService
{
    private readonly IProductService _productService;
    private readonly IPaymentService _paymentService;
    private readonly INotificationService _notificationService;
    private List<Order> _orders = new List<Order>();
    private int _nextId = 1;

    public OrderService(IProductService productService, IPaymentService paymentService, INotificationService notificationService)
    {
        _productService = productService;
        _paymentService = paymentService;
        _notificationService = notificationService;
    }

    public Order CreateOrder(int userId, List<int> productIds)
    {
        var products = _productService.GetProducts().Where(p => productIds.Contains(p.Id)).ToList();
        if (!products.Any())
        {
            throw new Exception("No products found for the given IDs.");
        }

        var order = new Order { Id = _nextId++, UserId = userId, Products = products, Status = "Created" };
        decimal totalAmount = products.Sum(p => p.Price);

        if (_paymentService.ProcessPayment(order.Id, totalAmount))
        {
            order.Status = "Paid";
            _notificationService.SendNotification(userId, "Your order has been successfully paid.");
        }
        else
        {
            order.Status = "Payment Failed";
            _notificationService.SendNotification(userId, "Payment failed. Please try again.");
        }

        _orders.Add(order);
        return order;
    }

    public Order GetOrderStatus(int orderId)
    {
        return _orders.FirstOrDefault(o => o.Id == orderId);
    }
}

#endregion

#region Main Program

class Program
{
    static void Main(string[] args)
    {
        IUserService userService = new UserService();
        IProductService productService = new ProductService();
        IPaymentService paymentService = new PaymentService();
        INotificationService notificationService = new NotificationService();
        IOrderService orderService = new OrderService(productService, paymentService, notificationService);

        
        var user = userService.Register("Alice", "password123");
        Console.WriteLine($"User registered: {user.Username}");

        
        productService.AddProduct(new Product { Name = "Laptop", Price = 1200 });
        productService.AddProduct(new Product { Name = "Smartphone", Price = 800 });

        
        Console.WriteLine("\nAvailable Products:");
        foreach (var product in productService.GetProducts())
        {
            Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: {product.Price:C}");
        }

        
        var order = orderService.CreateOrder(user.Id, new List<int> { 1, 2 });
        Console.WriteLine($"\nOrder Created: ID {order.Id}, Status: {order.Status}");

       
        var orderStatus = orderService.GetOrderStatus(order.Id);
        Console.WriteLine($"\nOrder Status: ID {orderStatus.Id}, Status: {orderStatus.Status}");
    }
}

#endregion
