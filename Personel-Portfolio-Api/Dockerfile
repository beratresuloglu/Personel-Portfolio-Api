# 1. AŞAMA: Build (Derleme)
# Microsoft'un resmi .NET 8 SDK imajını kullanıyoruz
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et (paketleri indir)
COPY ["Personel-Portfolio-Api.csproj", "./"]
RUN dotnet restore "Personel-Portfolio-Api.csproj"

# Kalan tüm dosyaları kopyala
COPY . .

# Projeyi Release modunda derle ve yayınla
RUN dotnet publish "Personel-Portfolio-Api.csproj" -c Release -o /app/publish

# 2. AŞAMA: Runtime (Çalıştırma)
# Sadece çalışmak için gereken hafif ASP.NET Core Runtime imajını kullanıyoruz
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Render ve Linux ortamları için port ayarı (Genelde 8080)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "Personel-Portfolio-Api.dll"]