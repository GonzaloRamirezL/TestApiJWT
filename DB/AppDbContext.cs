namespace ApiJWT.DB
{
    using ApiJWT.ViewModels;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UsuarioVM> Usuarios { get; set; }
    }

}
