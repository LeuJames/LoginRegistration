using Microsoft.EntityFrameworkCore; 
    namespace LoginRegistration.Models
    {
        public class MyContext : DbContext
        {
            public MyContext(DbContextOptions options) : base(options) { }
            
						// This is where the models go
            public DbSet<User> Users {get;set;}
        }
    }