namespace BookRental.Data

{
    using Microsoft.EntityFrameworkCore;
    using BookRental.Models;

    public class Library : DbContext
    {
        public Library(DbContextOptions<Library> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Rent> Rents { get; set; }
    }
}