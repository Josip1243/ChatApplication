using ChatApplicationServer.Repository;
using Microsoft.EntityFrameworkCore;
using ChatApplicationServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ChatApplicationServer.HubConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers();
builder.Services.AddDbContextPool<ChatContext>( options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionString"))
);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Auth:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ChatService, ChatService>();
builder.Services.AddScoped<ConnectionService, ConnectionService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<UserRepositoryMock, UserRepositoryMock>();
builder.Services.AddSingleton<ChatRepositoryMock, ChatRepositoryMock>();
builder.Services.AddSingleton<ConnectionsRepositoryMock, ConnectionsRepositoryMock>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseCors("AllowAllHeaders");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();