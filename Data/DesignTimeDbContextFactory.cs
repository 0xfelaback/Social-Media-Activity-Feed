using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class SocialMediaDataContextFactory : IDesignTimeDbContextFactory<SocialMediaDataContext>
{
    public SocialMediaDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SocialMediaDataContext>();
        optionsBuilder.UseSqlite("Data Source=social.db");

        return new SocialMediaDataContext(optionsBuilder.Options);
    }
}