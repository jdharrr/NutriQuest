using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NutriQuestAPI;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injections
builder.Services.ConfigureKeyVault(builder.Configuration);
builder.Services.ConfigureEmail(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.ConfigureGoogleApi(builder.Configuration);
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureNutriQuestRepositories();
builder.Services.ConfigureNutriQuestServices();

// Other Services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id= "Bearer",
                    Type=ReferenceType.SecurityScheme,
                }
            },
            new string[]{ }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNutriQuestApp",
        policy =>
        {
            policy.WithOrigins("https://proud-grass-047c1c410.6.azurestaticapps.net", "http://127.0.0.1:5500",
                    "http://127.0.0.1:5115", "http://127.0.0.1:5501")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowNutriQuestApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();