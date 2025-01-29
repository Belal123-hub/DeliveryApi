using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public override DbSet<User> Users { get; set; }
        public override DbSet<Role> Roles { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<LogoutUser> LogoutUsers { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishRating> DishRatings { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Basket and BasketItem relationship
            modelBuilder.Entity<Basket>()
                .HasMany(b => b.Items)
                .WithOne(i => i.Basket)
                .HasForeignKey(i => i.BasketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order and OrderItem relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // DishRating composite key and relationships
            modelBuilder.Entity<DishRating>()
                .HasKey(dr => new { dr.UserId, dr.DishId });

            modelBuilder.Entity<DishRating>()
                .HasOne(dr => dr.User)
                .WithMany()
                .HasForeignKey(dr => dr.UserId);

            modelBuilder.Entity<DishRating>()
                .HasOne(dr => dr.Dish)
                .WithMany()
                .HasForeignKey(dr => dr.DishId);

            // User and Order relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            // Basket and User relationship
            modelBuilder.Entity<Basket>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);

            // BasketItem and Dish relationship
            modelBuilder.Entity<BasketItem>()
                .HasOne(bi => bi.Dish)
                .WithMany()
                .HasForeignKey(bi => bi.DishId);

            // OrderItem and Dish relationship
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Dish)
                .WithMany()
                .HasForeignKey(oi => oi.DishId);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public int SaveChanges(DateTime dateTime)
        {
            AddTimestamps(dateTime);
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess, DateTime dateTime)
        {
            AddTimestamps(dateTime);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync(DateTime dateTime, CancellationToken cancellationToken = default)
        {
            AddTimestamps(dateTime);
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public Task<int> SaveChangesAsync(DateTime dateTime, bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            AddTimestamps(dateTime);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AddTimestamps(DateTime? dateTime = null)
        {
            var entities = ChangeTracker.Entries().Where(x => x is
            { Entity: IBaseEntity, State: EntityState.Added or EntityState.Deleted or EntityState.Modified });
            foreach (var entity in entities)
            {
                var now = dateTime ?? DateTime.UtcNow;
                switch (entity.State)
                {
                    case EntityState.Deleted:
                        ((IBaseEntity)entity.Entity).DeleteDateTime = now;
                        entity.State = EntityState.Modified;
                        break;
                    case EntityState.Modified:
                        ((IBaseEntity)entity.Entity).ModifyDateTime = now;
                        break;
                    case EntityState.Added:
                        ((IBaseEntity)entity.Entity).CreateDateTime = now;
                        break;
                }
            }
        }

    }
}
