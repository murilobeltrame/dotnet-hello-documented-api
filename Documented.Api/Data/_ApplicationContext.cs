using Microsoft.EntityFrameworkCore;

namespace Documented.Api.Data
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Todo> Todos { get; set; }
    }
}
