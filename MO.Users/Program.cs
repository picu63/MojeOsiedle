using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MO.Auth.Data;
using MO.Auth.Mappings;
using MO.Integration.MessagingBus;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341/", LogEventLevel.Information)
    .CreateLogger());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IMessageBus, AzServiceBusMessageBus>(_ => new AzServiceBusMessageBus(builder.Configuration));
builder.Services.AddDbContext<UsersDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("UsersDb")));
builder.Services.AddIdentity<AuthUser, IdentityRole>().AddEntityFrameworkStores<UsersDbContext>();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

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

app.Run();
