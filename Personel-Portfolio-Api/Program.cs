var builder = WebApplication.CreateBuilder(args);

// 1. Servisleri ekle
builder.Services.AddCors(options => {
    options.AddPolicy("AllowSpecificOrigins", b =>
        b.WithOrigins(
            "http://localhost:5500",      // VS Code Live Server (localhost olarak açarsan)
            "http://127.0.0.1:5500",      // BURASI EKLENDİ (Ekran görüntündeki adres)
            "https://localhost:7164",
            "https://portfolio-frontend-rho-red.vercel.app" // İlerideki canlı adresin
        )
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials()); // Bazen gerekebilir, eklemekte fayda var
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Politika ismini güncelledik: "AllowSpecificOrigins"
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();