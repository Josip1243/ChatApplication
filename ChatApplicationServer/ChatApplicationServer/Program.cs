using ChatApplicationServer.Repository;
using ChatApplicationServer.Hubs;
using Microsoft.EntityFrameworkCore;
using ChatApplicationServer.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers();
builder.Services.AddDbContextPool<ChatContext>( options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionString"))
);

var services = builder.Services;

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddSingleton<UserRepositoryMock, UserRepositoryMock>();
builder.Services.AddSingleton<ConnectionsRepositoryMock, ConnectionsRepositoryMock>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors("AllowAllHeaders");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();