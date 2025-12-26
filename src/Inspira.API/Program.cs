using Inspira.Infrastructure;
using Inspira.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Inspira.API.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MongoDB repositories
builder.Services.AddMongoRepositories(builder.Configuration);

// Register application services
builder.Services.AddScoped<ISsnCheckService, SsnCheckService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(SsnCheckMappingProfile));

// Add controllers
builder.Services.AddControllers();

// Configure Auth0 JWT bearer authentication for incoming requests
var auth0Domain = builder.Configuration["Auth0:Domain"] ?? string.Empty;
var auth0Audience = builder.Configuration["Auth0:Audience"] ?? string.Empty;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = auth0Domain;
    options.Audience = auth0Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SsnScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "inspira.api.ssn");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
