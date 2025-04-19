using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using System; // 确保添加此命名空间

namespace LibraryManagementSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; } // 添加借阅记录表

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置Book表
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.Author)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(b => b.ISBN)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(b => b.Publisher)
                    .HasMaxLength(100);

                entity.Property(b => b.Status)
                    .IsRequired()
                    .HasDefaultValue("可借");

                // 静态种子数据（所有值都是硬编码）
                entity.HasData(
                    new Book
                    {
                        Id = 1,
                        Title = "C#入门经典",
                        Author = "John Smith",
                        ISBN = "978-1234567890",
                        Publisher = "清华大学出版社",
                        PublicationDate = new DateTime(2023, 1, 1), // 固定日期
                        Status = "可借"
                    }
                );
            });

            // 配置User表
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasDefaultValue("Reader")
                    .HasMaxLength(20);
            });

            // 配置BorrowRecord表（按需添加）
            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.ToTable("BorrowRecords");
                entity.HasKey(br => br.Id);

                entity.Property(br => br.BorrowDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(br => br.DueDate)
                    .IsRequired();

                // 关系配置
                entity.HasOne(br => br.Book)
                    .WithMany(b => b.BorrowRecords)
                    .HasForeignKey(br => br.BookId);

                entity.HasOne(br => br.User)
                    .WithMany(u => u.BorrowedBooks)
                    .HasForeignKey(br => br.UserId);
            });
        }
    }
}