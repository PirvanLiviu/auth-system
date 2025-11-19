using NeonTest.Data;
using NeonTest.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<NeonDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => 
      {
        options.TokenValidationParameters = new TokenValidationParameters
        {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:issuer"],
        ValidAudience = builder.Configuration["JWT:audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:signingkey"]))
        };
      });
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
