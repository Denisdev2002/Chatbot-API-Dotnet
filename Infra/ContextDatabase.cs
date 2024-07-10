using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra
{
    public class ContextDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UsersDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Question>().ToTable("Question");
            modelBuilder.Entity<Session>().ToTable("Session");
            modelBuilder.Entity<Conversation>().ToTable("Conversation");
            modelBuilder.Entity<Response>().ToTable("Response");
            modelBuilder.Entity<Source>().ToTable("Source");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Source> Sources { get; set; }
    }
}