namespace GlamourLights
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=Model1")
        {
        }

        public virtual DbSet<customer> customer { get; set; }
        public virtual DbSet<department> department { get; set; }
        public virtual DbSet<item> item { get; set; }
        public virtual DbSet<purchase> purchase { get; set; }
        public virtual DbSet<shelf> shelf { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<customer>()
                .Property(e => e.firstName)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.lastName)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.cardNumber)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .HasMany(e => e.purchase)
                .WithRequired(e => e.customer1)
                .HasForeignKey(e => e.customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<department>()
                .Property(e => e.departmentName)
                .IsUnicode(false);

            modelBuilder.Entity<department>()
                .Property(e => e.genderType)
                .IsUnicode(false);

            modelBuilder.Entity<department>()
                .HasMany(e => e.shelf)
                .WithRequired(e => e.department)
                .HasForeignKey(e => e.departmentCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<item>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<item>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<item>()
                .Property(e => e.color)
                .IsUnicode(false);

            modelBuilder.Entity<item>()
                .Property(e => e.fabric)
                .IsUnicode(false);

            modelBuilder.Entity<item>()
                .Property(e => e.gender)
                .IsUnicode(false);

            modelBuilder.Entity<item>()
                .Property(e => e.type)
                .IsUnicode(false);

            modelBuilder.Entity<item>()
                .HasMany(e => e.purchase)
                .WithRequired(e => e.item1)
                .HasForeignKey(e => e.item)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<shelf>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<shelf>()
                .HasMany(e => e.item)
                .WithOptional(e => e.shelf1)
                .HasForeignKey(e => e.shelf);
        }
    }
}
