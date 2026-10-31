
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;


namespace OnlineStore.DataAccess;

public class ApplicationDbContext : DbContext
{

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=.;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
    }


        

}


