using WebPoker.DataBaseContext;
using WebPoker.Models;
using WebPoker.PlayersRepository;
using WebPoker.Repository;
using WebPoker.RepositoryInterfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PokerDbContext>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<IRepositoryReader<PlayerResponse>>(services => services.GetRequiredService<PlayerRepository>());
builder.Services.AddScoped<IPlayerCreator>(services => services.GetRequiredService<PlayerRepository>());
builder.Services.AddScoped<IPlayerUpdater>(services => services.GetRequiredService<PlayerRepository>());
builder.Services.AddScoped<IPlayerDeleter>(services => services.GetRequiredService<PlayerRepository>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "WebPoker API v1");
    });
}

app.UseHttpsRedirection();

var defaultFiles = new DefaultFilesOptions();

defaultFiles.DefaultFileNames.Clear();

defaultFiles.DefaultFileNames.Add("index.html");

app.UseDefaultFiles(defaultFiles);

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

using (var context = new PokerDbContext())
{
    await context.Database.EnsureCreatedAsync();
}
app.Run();