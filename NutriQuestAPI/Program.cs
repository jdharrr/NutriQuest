using NutriQuestAPI;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injections
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureGoogleApi(builder.Configuration);

// Other Services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();