using Microsoft.EntityFrameworkCore;
using MO.Apis;
using MO.LiveChat;
using MO.LiveChat.Configs;
using MO.LiveChat.Data;
using MO.LiveChat.Mappings;
using MO.LiveChat.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<LiveChatDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("LiveChatDb")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(x=>new AuthService(builder.Configuration.GetSection("Apis:MOAuth").Value, new HttpClient()));
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton<UserUpdater>();
builder.Services.AddOptions<UserUpdaterConfiguration>()
    .Bind(builder.Configuration.GetSection("UserUpdater"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
//builder.Services.Configure<UserUpdaterConfiguration>(builder.Configuration.GetSection("UserUpdater"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Services.GetService<UserUpdater>().StartAsync(app.Services.GetService<IHostApplicationLifetime>().ApplicationStopping);

app.Run();