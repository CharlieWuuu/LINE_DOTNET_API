using Microsoft.EntityFrameworkCore;

namespace LINE_DotNet_API.Dtos
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<USER> USERS { get; set; }
		public DbSet<USER_LOGIN> USER_LOGINS { get; set; }
		public DbSet<SUBSCRIBE> SUBSCRIBES { get; set; }
		public DbSet<EMAIL_VERIFICATION> EMAIL_VERIFICATIONS { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<USER>()
				.HasKey(u => u.USER_ID);

			modelBuilder.Entity<USER_LOGIN>()
				.HasKey(ul => new { ul.USER_ID, ul.LOGIN_TIME });

			modelBuilder.Entity<SUBSCRIBE>()
				.HasKey(s => s.LINE_ID);

            modelBuilder.Entity<EMAIL_VERIFICATION>()
				.HasKey(s => s.EMAIL);
        }
	}
}
