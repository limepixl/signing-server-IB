using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProxyServer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProxyServer.Models.SignatureStatement>? SignatureStatement { get; set; }
        public DbSet<ProxyServer.Models.ServerKeyPair>? ServerKeyPair { get; set; }
    }
}