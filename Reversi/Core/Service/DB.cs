using Microsoft.EntityFrameworkCore;

namespace Reversi.Core.Service
{

    class ReversiDBContext : DbContext {

        public DbSet<Score.Score> Scores { get; set; }
        public DbSet<Rating.Rating> Ratings { get; set; }
        public DbSet<Comments.Comment> Comments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Reversi;Trusted_Connection=True;");
        }

    }
}
