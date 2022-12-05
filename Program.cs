using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyBoards_myVersion.Entities;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<MyBoardsContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();
if(pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();
if(!users.Any())
{
    var user1 = new User()
    {
        Email = "user1@test.com",
        FullName = "User One",
        Address = new Address()
        {
            City = "Warszawa",
            Street = "Szeroka"
        }
    };

    var user2 = new User()
    {
        Email = "user2@test.com",
        FullName = "User Two",
        Address = new Address()
        {
            City = "Poznan",
            Street = "Katowicka"
        }
    };

    dbContext.Users.AddRange(user1, user2);

    dbContext.SaveChanges();
}


/*app.MapGet("pagination", async (MyBoardsContext db) =>
{
    //user input
    var filter = "a";
    string sortBy = "";
    bool sortByDescending = false;
    int pageNumber = 1;
    int pagesize = 10;
    //

    var query = db.Users
        .Where(u => filter == null || 
        (u.Email.Contains(filter, StringComparison.OrdinalIgnoreCase) || u.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase)));

    if(sortBy != null)
    {
        //Expression<Func<User, object>> sortByExpression = user => user.Email;
        var columnsSelector = new Dictionary<string, Expression<Func<User, object>>>();
        query.OrderBy(sortByExpression);
    }
}
);
*/

app.MapGet("data", async (MyBoardsContext db) =>
{
    var user = await db.Users.Include(u => u.Comments)
    .ThenInclude(c => c.WorkItem)
    .Include(u => u.Address)
    .FirstAsync(u => u.Id == Guid.Parse("5CB27C3F-32D9-4474-CBC2-08DA10AB0E61"));
    
    return user;
}
);

app.MapPost("update", async (MyBoardsContext db) =>
{
    var epic = await db.Epics.FirstAsync(epic => epic.Id ==1);

    var onHoldState = db.WorkItemStates.FirstAsync(a => a.State == "On Hold");
    epic.StateId = 1;

    await db.SaveChangesAsync();
    return epic;
});

app.MapPost("create", async (MyBoardsContext db) =>
{
    var address = new Address()
    {
        City = "Krakow",
        Country = "Poland",
        Street = "Bracka"
    };

    var user = new User()
    {
        Email = "asdfghjkl@op.pl",
        FullName = "Test User",
        Address = address,
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();
});

app.MapDelete("delete", async (MyBoardsContext db) =>
{
    var workItemTags = await db.WorkItemTags
      .Where(c => c.WorkItemId == 12)
      .ToListAsync();

    db.WorkItemTags.RemoveRange(workItemTags);

    await db.SaveChangesAsync();
});

app.Run();
