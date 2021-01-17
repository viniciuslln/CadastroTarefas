using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class CadastroTarefasContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=appdb.db3");
        }
    }
}
