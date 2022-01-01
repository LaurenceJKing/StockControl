var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.MapPost("orders/create", () =>
{
    var id = "dc069aeb-5eb9-4f1b-9dba-bcc7d7b0c47d";
    return Results.Created($"/orders/{id}", id);
});

app.UseHttpsRedirection();

app.Run();
