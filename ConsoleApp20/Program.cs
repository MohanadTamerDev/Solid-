using System;
using System.Collections.Generic;
using System.Linq;


// ==========================================
// Models
// ==========================================

class Order
{
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public CustomerType CustomerType { get; set; }
}

class OrderItem
{
    public decimal Price { get; set; }
}

enum CustomerType
{
    Normal,
    VIP1,
    VIP2
}


// ==========================================
// 1. Validation
// ==========================================

interface IOrderValidator
{
    void Validate(Order order);
}

class OrderValidator : IOrderValidator
{
    public void Validate(Order order)
    {
        if (order == null)
            throw new Exception("Invalid Order");

        if (order.Items.Count == 0)
            throw new Exception("No items have been found");
    }
}


// ==========================================
// 2. Discount
// ==========================================

interface IDiscount
{
    decimal CalculateDiscount(decimal totalPrice);
}


// VIP 1 = 30%
class VIP1Discount : IDiscount
{
    public decimal CalculateDiscount(decimal totalPrice)
    {
        return totalPrice * 30 / 100;
    }
}


// VIP 2 = 20%
class VIP2Discount : IDiscount
{
    public decimal CalculateDiscount(decimal totalPrice)
    {
        return totalPrice * 20 / 100;
    }
}


// Normal = 10%
class NormalDiscount : IDiscount
{
    public decimal CalculateDiscount(decimal totalPrice)
    {
        return totalPrice * 10 / 100;
    }
}


// ==========================================
// 3. Save Order
// ==========================================

interface IOrderRepository
{
    void Save(Order order);
}

class OrderRepository : IOrderRepository
{
    public void Save(Order order)
    {
        Console.WriteLine("Order has been saved");
    }
}


// ==========================================
// 4. Send Email
// ==========================================

interface IEmailService
{
    void SendEmail(Order order);
}

class EmailService : IEmailService
{
    public void SendEmail(Order order)
    {
        Console.WriteLine("An Email was sent to the customer");
    }
}


// ==========================================
// 5. Order Service
// ==========================================

class OrderService
{
    private readonly IOrderValidator validator;
    private readonly IOrderRepository repository;
    private readonly IEmailService emailService;

    public OrderService(
        IOrderValidator validator,
        IOrderRepository repository,
        IEmailService emailService)
    {
        this.validator = validator;
        this.repository = repository;
        this.emailService = emailService;
    }


    public void CheckOut(Order order, IDiscount discount)
    {
        // Validation
        validator.Validate(order);


        // Calculate Total
        decimal totalPrice = 0;

        foreach (var item in order.Items)
        {
            totalPrice += item.Price;
        }


        // Discount
        decimal discountValue =
            discount.CalculateDiscount(totalPrice);

        totalPrice -= discountValue;


        Console.WriteLine($"Total Price = {totalPrice}");


        // Save
        repository.Save(order);


        // Send Email
        emailService.SendEmail(order);
    }
}


// ==========================================
// Main
// ==========================================

class Program
{
    static void Main()
    {
        Order order = new Order();

        order.CustomerType = CustomerType.VIP1;

        order.Items.Add(new OrderItem { Price = 100 });
        order.Items.Add(new OrderItem { Price = 200 });
        order.Items.Add(new OrderItem { Price = 300 });


        // Dependencies
        IOrderValidator validator =
            new OrderValidator();

        IOrderRepository repository =
            new OrderRepository();

        IEmailService emailService =
            new EmailService();


        OrderService service =
            new OrderService(
                validator,
                repository,
                emailService);


        // VIP1 = 30% discount
        IDiscount discount =
            new VIP1Discount();


        service.CheckOut(order, discount);
    }
}
