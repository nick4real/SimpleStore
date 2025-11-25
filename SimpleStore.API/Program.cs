using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using SimpleStore.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("simplestore-db");
    options.UseSqlServer(connectionString, o =>
    {
        //o.EnableRetryOnFailure();
    });
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost,

    // Only use 1 hop in Azure
    ForwardLimit = 1
});

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await using (var serviceScope = app.Services.CreateAsyncScope())
await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>())
{
    var executionStrategy = dbContext.Database.CreateExecutionStrategy();

    await executionStrategy.ExecuteAsync(async () =>
    {
        if (!app.Environment.IsProduction())
            await dbContext.Database.EnsureDeletedAsync();

         await dbContext.Database.EnsureCreatedAsync();
    });
}

app.Run();
