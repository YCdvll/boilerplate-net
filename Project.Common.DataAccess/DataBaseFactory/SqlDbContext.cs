using Microsoft.EntityFrameworkCore;
using Project.Common.DataAccess.Models;
using Project.Common.DataAccess.Models.User;

namespace Project.Common.DataAccess.DataBaseFactory;

public class SqlDbContext(DbContextOptions<SqlDbContext> options) : DbContext(options)
{
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<PostalAddress> PostalAddress { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}