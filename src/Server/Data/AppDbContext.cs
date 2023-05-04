using BlazorWallets.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorWallets.Server.Data;

public class AppDbContext : DbContext
{
    public DbSet<Wallet> Wallets { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
    {
    }
}
