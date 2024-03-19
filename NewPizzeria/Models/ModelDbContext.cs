using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NewPizzeria.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        public virtual DbSet<DettaglioOrdini> DettaglioOrdini { get; set; }
        public virtual DbSet<Ordini> Ordini { get; set; }
        public virtual DbSet<Prodotti> Prodotti { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ordini>()
                .HasMany(e => e.DettaglioOrdini)
                .WithRequired(e => e.Ordini)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prodotti>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<Prodotti>()
                .Property(e => e.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Prodotti>()
                .HasMany(e => e.DettaglioOrdini)
                .WithRequired(e => e.Prodotti)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.Utenti)
                .WithMany(e => e.Roles)
                .Map(m => m.ToTable("UtentiRuoli").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<Utenti>()
                .Property(e => e.Username)
                .IsFixedLength();

            modelBuilder.Entity<Utenti>()
                .Property(e => e.Password)
                .IsFixedLength();

            modelBuilder.Entity<Utenti>()
                .Property(e => e.Email)
                .IsFixedLength();

            modelBuilder.Entity<Utenti>()
                .Property(e => e.Role)
                .IsFixedLength();

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Ordini)
                .WithRequired(e => e.Utenti)
                .WillCascadeOnDelete(false);
        }
    }
}
