using WebPoker.DataBaseContext;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PokerDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
