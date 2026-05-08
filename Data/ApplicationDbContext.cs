using Microsoft.EntityFrameworkCore;

namespace TravelSphereMVC.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User>         Users         { get; set; }
        public DbSet<Package>      Packages      { get; set; }
        public DbSet<Booking>      Bookings      { get; set; }
        public DbSet<Traveller>    Travellers    { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Review>       Reviews       { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Decimal Precision ────────────────────────────────────────
            modelBuilder.Entity<Package>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Package>()
                .Property(p => p.DiscountPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            // ── Package → Admin ──────────────────────────────────────────
            modelBuilder.Entity<Package>()
                .HasOne(p => p.Admin)
                .WithMany(u => u.Packages)
                .HasForeignKey(p => p.AdminId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // ── Booking → User ───────────────────────────────────────────
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Booking → Package ────────────────────────────────────────
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Package)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Traveller → Booking ──────────────────────────────────────
            modelBuilder.Entity<Traveller>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Travellers)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── WishlistItem → User ──────────────────────────────────────
            modelBuilder.Entity<WishlistItem>()
                .HasOne(w => w.User)
                .WithMany(u => u.WishlistItems)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── WishlistItem → Package (NoAction to avoid multiple cascade path) ──
            modelBuilder.Entity<WishlistItem>()
                .HasOne(w => w.Package)
                .WithMany(p => p.WishlistItems)
                .HasForeignKey(w => w.PackageId)
                .OnDelete(DeleteBehavior.NoAction);

            // ── Notification → User ──────────────────────────────────────
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Review → User (NoAction — avoid multiple cascade from User) ──
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ── Review → Package (NoAction — Package→User→Review cascade avoided) ─
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Package)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
