using Microsoft.EntityFrameworkCore;

namespace MyBoards_myVersion.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options)
        {

        }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<WorkItemState> WorkItemStates { get; set; }
        public DbSet<WorkItemTag> WorkItemTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Epic>()
                .Property(wi => wi.EndDate)
                .HasPrecision(3);

            modelBuilder.Entity<Task>(eb =>
            {
                eb.Property(wi => wi.Activity).HasMaxLength(200);
                eb.Property(wi => wi.RemainingWork).HasPrecision(14, 2);
            });

            modelBuilder.Entity<Issue>(eb =>
            {
                eb.Property(wi => wi.Efford).HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<WorkItem>(eb =>
            {
                eb.Property(x => x.Area).HasColumnType("varchar(200)");
                eb.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
                eb.Property(wi => wi.Priority).HasDefaultValue(1);

                eb.HasMany(w => w.Comments)
                    .WithOne(c => c.WorkItem)
                    .HasForeignKey(c => c.WorkItemId);

                eb.HasOne(w => w.Author)
                    .WithMany(u => u.WorkItems)
                    .HasForeignKey(w => w.AuthorId);

                eb.HasOne(w => w.State)
                    .WithMany()
                    .HasForeignKey(w => w.StateId);

                eb.HasMany(w => w.Tags)
                    .WithMany(t => t.WorkItems)
                    .UsingEntity<WorkItemTag>(
                        w => w.HasOne(wit => wit.Tag)
                            .WithMany()
                            .HasForeignKey(wit => wit.TagId),
                        w => w.HasOne(wit => wit.WorkItem)
                            .WithMany()
                            .HasForeignKey(wit => wit.WorkItemId),
                        wit =>
                        {
                        wit.HasKey(x => new { x.TagId, x.WorkItemId});
                        wit.Property(x => x.PublicationDate).HasDefaultValueSql("getutcdate()");
                        }
                    ); 
            });

            modelBuilder.Entity<Comment>(eb =>
            {
                eb.Property(c => c.CreatedDate).HasDefaultValueSql("getutcdate()");
                eb.Property(c => c.UpdatedDate).ValueGeneratedOnUpdate();
                eb.HasOne(c => c.Author)
                    .WithMany(a => a.Comments)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>(eb =>
            {
                eb.HasOne(u => u.Address)
                    .WithOne(a => a.User)
                    .HasForeignKey<Address>(a => a.UserId);

            });

            modelBuilder.Entity<WorkItemState>(eb =>
            {
                eb.Property(x => x.State).IsRequired().HasMaxLength(50);
                eb.HasData(new WorkItemState() { Id = 1,State = "To Do" },
                    new WorkItemState() { Id=2,State = "Doing"},
                    new WorkItemState() { Id=3,State = "Done"}
                    );
            });

            modelBuilder.Entity<Tag>(eb =>
            {
                eb.HasData(new Tag() { Id = 1, Value="Web"},
                    new Tag() { Id = 2, Value = "UI" },
                    new Tag() { Id = 3, Value = "Desktop" },
                    new Tag() { Id = 4, Value = "API" },
                    new Tag() { Id = 5, Value = "Service" });
            });
          
        }

    }
}
