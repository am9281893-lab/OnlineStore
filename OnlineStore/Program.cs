using Microsoft.EntityFrameworkCore;
using OnlineStore.DataAccess;
using OnlineStore.Models;

using System.Text.Json;

ApplicationDbContext _db = new();


var result = _db.Orders
    .Where(e => e.Status == OrderStatus.Delivered)
    .Select(e => new
    {
        OrderId = e.Id,
        CustomerName = e.Customer.FullName,
        Total = e.OrderItems.Sum(e => e.Quantity * e.UnitPrice * (1 - e.Discount))

    })
 .OrderByDescending(e => e.Total)
 .ToList();


var totalOrders = _db.Orders.Count();

var result2 = _db.Orders
    .GroupBy(e => e.Status)
    .Select(e => new
    {
        Status = e.Key,
        OrderCount = e.Count(),
        Percentage = totalOrders == 0
            ? 0
            : (double)e.Count() * 100 / totalOrders
    })
    .ToList();


var result3 = _db.Products
    .Where(e => e.Price >
        _db.Products
            .Where(e => e.CategoryId == e.CategoryId)
            .Average(e => (decimal?)e.Price))
    .Select(e => new
    {
        ProductName = e.Name,
        e.Price,
        CategoryName = e.Category.Name
    })
    .ToList();


var result4 = _db.Products
    .Where(e => !e.OrderItems.Any())
    .Select(e => new
    {
        ProductId = e.Id,
        ProductName = e.Name,
        e.Price
    })
    .ToList();



var thirtyDaysAgo = DateTime.Now.AddDays(-30);

var result5 = _db.Customers
    .Where(e => e.RegisteredAt >= thirtyDaysAgo
             && e.Orders.Any())
    .Select(e => new
    {
        CustomerId = e.Id,
        CustomerName = e.FullName,
        e.Email,
        e.RegisteredAt
    })
    .ToList();


var result6 = _db.Customers
    .Select(e => new
    {
        CustomerId = e.Id,
        CustomerName = e.FullName,
        CancelledOrdersCount = e.Orders.Count(o => o.Status == OrderStatus.Cancelled)
    })
    .Where(e => e.CancelledOrdersCount > 2)
    .ToList();



var result7 = _db.Customers
.Select(e => new
{
    CustomerName = e.FullName,

    OrderCount = e.Orders
     .Count(e => e.Status == OrderStatus.Delivered),

    TotalSpent = e.Orders
     .Where(e => e.Status == OrderStatus.Delivered)
     .Sum(e => e.OrderItems.Sum(e =>
         e.Quantity * e.UnitPrice * (1 - e.Discount))),

    AverageOrderValue = e.Orders
     .Where(o => o.Status == OrderStatus.Delivered)
     .Select(o => (decimal?)o.OrderItems.Sum(i =>
         i.Quantity * i.UnitPrice * (1 - i.Discount)))
     .Average() ?? 0m
})
.OrderByDescending(e => e.TotalSpent)
.Take(5)
.ToList();


var result8 = _db.Orders
    .Where(e => e.Status == OrderStatus.Delivered
             && e.OrderDate.Year == 2025)
    .GroupBy(e => new
    {
        e.OrderDate.Year,
        e.OrderDate.Month
    })
    .Select(e => new
    {
        e.Key.Year,
        e.Key.Month,
        OrderCount = e.Count(),
        Revenue = e.Sum(e => e.OrderItems.Sum(e =>
            e.Quantity * e.UnitPrice * (1 - e.Discount)))
    })
    .OrderBy(x => x.Month)
    .ToList();



var result9 = _db.OrderItems
    .Where(e => e.Order.Status == OrderStatus.Delivered)
    .GroupBy(e => e.Product.Category.Name)
    .Select(e => new
    {
        CategoryName = e.Key,

        Revenue = e.Sum(e =>
            e.Quantity * e.UnitPrice * (1 - e.Discount))
    })
    .OrderByDescending(e => e.Revenue)
    .ToList();



var result10 = _db.Customers
    .Where(e => !e.Orders.Any())
    .GroupBy(e => e.City)
    .Select(e => new
    {
        City = e.Key,
        CustomerCount = e.Count()
    })
    .OrderByDescending(e => e.CustomerCount)
    .ToList();



var result11 = _db.Products
    .Where(e => e.Reviews.Count() >= 10)
    .Select(e => new
    {
        ProductName = e.Name,
        CategoryName = e.Category.Name,
        ReviewCount = e.Reviews.Count(),
        AverageRating = e.Reviews.Average(e => e.Rating)
    })
    .Where(e => e.AverageRating >= 4.5)
    .ToList();



var result12 = _db.Categories
    .SelectMany(e => e.Products
        .OrderByDescending(e => e.Price)
        .Take(2),
        (e, p) => new
        {
            CategoryName = p.Name,
            ProductName = p.Name,
            p.Price
        })
    .ToList();



var result13 = _db.Products
    .Select(e => new
    {
        ProductName = e.Name,
        e.Stock,

        PendingQuantity = e.OrderItems
            .Where(e => e.Order.Status == OrderStatus.Pending)
            .Sum(e => (int?)e.Quantity) ?? 0
    })
    .Where(e => e.Stock < e.PendingQuantity)
    .ToList();



var customerSpending = _db.Customers
    .Select(e => new
    {
        CustomerName = e.FullName,

        TotalSpent = e.Orders
            .Where(e => e.Status == OrderStatus.Delivered)
            .SelectMany(e => e.OrderItems)
            .Sum(e =>
                e.Quantity * e.UnitPrice * (1 - e.Discount))
    });

var averageSpending = customerSpending
    .Average(e => e.TotalSpent);

var result14 = customerSpending
    .Where(e => e.TotalSpent > averageSpending)
    .ToList();



var result15 = _db.Categories
    .Select(e => new
    {
        CategoryName = e.Name,

        BestSellingProduct = e.Products
            .Select(e => new
            {
                ProductName = e.Name,

                TotalQuantitySold = e.OrderItems
                    .Where(e =>
                        e.Order.Status == OrderStatus.Delivered)
                    .Sum(e => (int?)e.Quantity) ?? 0
            })
            .OrderByDescending(e => e.TotalQuantitySold)
            .FirstOrDefault()
    })
    .Select(e => new
    {
        e.CategoryName,
        ProductName = e.BestSellingProduct == null
            ? null
            : e.BestSellingProduct.ProductName,
        TotalQuantitySold = e.BestSellingProduct == null
            ? 0
            : e.BestSellingProduct.TotalQuantitySold
    })
    .ToList();



var result16 = _db.Customers
    .Where(e => _db.Products
        .Where(p => p.Category.Name == "Laptops")
        .All(p => e.Orders
            .Where(e => e.Status == OrderStatus.Delivered)
            .SelectMany(e => e.OrderItems)
            .Any(e => e.ProductId == e.Id)))
    .Select(e => new
    {
        CustomerId = e.Id,
        CustomerName = e.FullName
    })
    .ToList();




var result17 = _db.Reviews
    .Where(e => !e.Customer.Orders
        .SelectMany(e => e.OrderItems)
        .Any(e => e.ProductId == e.ProductId))
    .Select(e => new
    {
        CustomerName = e.Customer.FullName,
        ProductName = e.Product.Name,
        e.Rating,
        e.Comment
    })
    .ToList();


var result18 = _db.Customers
    .GroupJoin(
        _db.Orders,
        customer => customer.Id,
        order => order.CustomerId,
        (customer, orders) => new
        {
            CustomerId = customer.Id,
            CustomerName = customer.FullName,
            OrderCount = orders.Count()
        })
    .ToList();


var result19 = _db.Orders
    .SelectMany(o => o.OrderItems
        .SelectMany(e => o.OrderItems
            .Where(e => e.ProductId < e.ProductId)
            .Select(e => new
            {
                Product1 = e.Product.Name,
                Product2 = e.Product.Name,
                Product1Id = e.ProductId,
                Product2Id = e.ProductId
            })))
    .GroupBy(e => new
    {
        e.Product1Id,
        e.Product2Id,
        e.Product1,
        e.Product2
    })
    .Select(e => new
    {
        e.Key.Product1,
        e.Key.Product2,
        TimesTogether = e.Count()
    })
    .OrderByDescending(e => e.TimesTogether)
    .Take(5)
    .ToList();



var result20 = _db.Customers
    .Where(e => e.Orders.Any())
    .Select(e => new
    {
        CustomerName = e.FullName,

        LatestOrder = e.Orders
            .OrderByDescending(e => e.OrderDate)
            .Select(e => new
            {
                OrderId = e.Id,
                e.OrderDate,

                Items = e.OrderItems
                    .Select(e => new
                    {
                        ProductName = e.Product.Name,
                        e.Quantity
                    })
                    .ToList()
            })
            .FirstOrDefault()
    })
    .ToList();




var result21 = _db.Customers
    .Select(e => new
    {
        CustomerName = e.FullName,

        FavoriteCategory = e.Orders
            .Where(e => e.Status == OrderStatus.Delivered)
            .SelectMany(e => e.OrderItems)
            .GroupBy(e => e.Product.Category.Name)
            .Select(e => new
            {
                CategoryName = e.Key,
                TotalQuantity = e.Sum(e => e.Quantity)
            })
            .OrderByDescending(e => e.TotalQuantity)
            .Select(e => new
            {
                e.CategoryName,
                e.TotalQuantity
            })
            .FirstOrDefault()
    })
    .Select(e => new
    {
        e.CustomerName,
        CategoryName = e.FavoriteCategory == null
            ? null
            : e.FavoriteCategory.CategoryName,
        TotalQuantity = e.FavoriteCategory == null
            ? 0
            : e.FavoriteCategory.TotalQuantity
    })
    .ToList();




var productRevenue = _db.Products
    .Select(e => new
    {
        ProductId = e.Id,
        ProductName = e.Name,

        Revenue = e.OrderItems
            .Where(e => e.Order.Status == OrderStatus.Delivered)
            .Sum(e =>
                e.Quantity * e.UnitPrice * (1 - e.Discount))
    })
    .OrderByDescending(e => e.Revenue)
    .ToList();

var result22 = productRevenue
    .Select((e, index) => new
    {
        Rank = index + 1,
        e.ProductName,
        e.Revenue
    })
    .ToList();


_db.Products
   .Where(e => e.Category.Name == "Accessories")
   .Where(e => e.Stock < 5)
   .ExecuteUpdate(e =>
e.SetProperty(e => e.Price, e => e.Price * 1.10m));

var oneYearAgo = DateTime.Now.AddYears(-1);
int deletedOrders = _db.Orders
.Where(o => o.Status == OnlineStore.Models.OrderStatus.Cancelled
         && o.OrderDate < oneYearAgo)
.ExecuteDelete();

Console.WriteLine($"Deleted Orders: {deletedOrders}");

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true
};

PrintResult("Result 1 - Delivered Orders", result, jsonOptions);
PrintResult("Result 2 - Orders by Status", result2, jsonOptions);
PrintResult("Result 3 - Products Above Category Average", result3, jsonOptions);
PrintResult("Result 4 - Products Never Ordered", result4, jsonOptions);
PrintResult("Result 5 - New Customers with Orders", result5, jsonOptions);
PrintResult("Result 6 - Customers with More Than 2 Cancelled Orders", result6, jsonOptions);
PrintResult("Result 7 - Top 5 Customers by Spending", result7, jsonOptions);
PrintResult("Result 8 - Monthly Revenue in 2025", result8, jsonOptions);
PrintResult("Result 9 - Revenue by Category", result9, jsonOptions);
PrintResult("Result 10 - Cities with Customers Without Orders", result10, jsonOptions);
PrintResult("Result 11 - Highly Rated Products", result11, jsonOptions);
PrintResult("Result 12 - Top 2 Products per Category", result12, jsonOptions);
PrintResult("Result 13 - Products with Insufficient Stock", result13, jsonOptions);
PrintResult("Result 14 - Customers Spending Above Average", result14, jsonOptions);
PrintResult("Result 15 - Best-Selling Product per Category", result15, jsonOptions);
PrintResult("Result 16 - Customers Who Bought All Laptops", result16, jsonOptions);
PrintResult("Result 17 - Reviews by Customers Who Never Bought the Product", result17, jsonOptions);
PrintResult("Result 18 - Order Count per Customer", result18, jsonOptions);
PrintResult("Result 19 - Products Frequently Bought Together", result19, jsonOptions);
PrintResult("Result 20 - Latest Order per Customer", result20, jsonOptions);
PrintResult("Result 21 - Favorite Category per Customer", result21, jsonOptions);
PrintResult("Result 22 - Product Revenue Ranking", result22, jsonOptions);

void PrintResult<T>(string title, T data, JsonSerializerOptions options)
{
    Console.WriteLine();
    Console.WriteLine($"========== {title} ==========");
    Console.WriteLine(JsonSerializer.Serialize(data, options));
}
