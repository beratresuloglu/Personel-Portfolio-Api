using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Personel_Portfolio_Api
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        // Değişkenleri tanımlıyoruz ama değer atamıyoruz
        private readonly string _apiKey;
        private readonly string _grokUrl;

        // Constructor (Yapıcı Metot): Kod çalıştırıldığında konfigürasyonu içeri alır
        public ChatController(IConfiguration configuration)
        {
            // appsettings.json içindeki "GroqSettings" bölümünden verileri okur
            _apiKey = configuration["GroqSettings:ApiKey"];
            _grokUrl = configuration["GroqSettings:ApiUrl"] ?? "https://api.groq.com/openai/v1/chat/completions";
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] UserRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Mesaj boş olamaz." });
            }

            // Güvenlik Kontrolü: Eğer API Key okunamazsa hata verelim
            if (string.IsNullOrEmpty(_apiKey))
            {
                return StatusCode(500, new { error = "Sunucu yapılandırma hatası: API Key bulunamadı." });
            }

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var payload = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = new[]
                    {
                        new {
                            role = "system",
                            content = "Berat Resuloğlu'nun portfolyo asistanısın. " +
                            "Bilgiler: Berat Resuloğu Sakarya Üniversitesi Bilgisayar Mühendisliği 3. sınıf öğrencisi. O bir AI native yazılım geliştirici." +
                            "Hobileri ve ilgi alanları: Tarihe meraklı bu alanda okumalar yapmayı sever, en sevdiği oyun Mount and Blade 2: Bannerlord, Kalabalıklardan hoşlanmaz, doğayı ve yağmuru sever " +
                            "O şöyle bir insan olduğunu söylüyor: devamlı araştırır ve öğrenmeyi sever, zorluklarda kolay pes etmeyip hep bir çözüm yolu arar. Uyumlu kişiliği ekip çalışmasına uygun." +
                            "Uzmanlık: ASP.NET Core, Flutter, Makine Öğrenmesi, Görüntü işleme, IoT" +
                            "Projeler: ANKA (Deprem sonrası şebekesiz ortamda göçük altındaki afetzedelere ulaşılmasını amaçlayan proje)," +
                            " Anlatmaca ( Flutter ile dart dilinde geliştirilmiş,'Tabu' oyunuyla benzer konseptli bir mobil oyun, Bu oyunla ilgili detaylı bilgi için ilgili proje kartına tıklayarak ilerlenebilir." +
                            " Tiger Fitness Center ( ASP.NET Core MVC teknolojisiyle geliştirilmiş, rol tabanlı yetkilendirme ve AI koçluğu barındıran bir spor salonu yönetimi uygulaması), " +
                            " BAY Turizm (PostgreSQl veritabanı kullanılarak geliştirilmiş kapsamlı bir otobüs firması yönetim sistemi masaüstü uygulaması)," +
                            " Secure Exit IoT (Sensörler yardımıyla deprem ve yangın durumlarını algılayıp acil durum algoritmasını çalıştıran, mobil uygulamadan kontrol edilebilen ve firebase ile büyük veri entegreasyonu olan bir nesnelerin interneti projesi)." +
                            " Kişisel Portfolyo sitesi (Modern web teknolojileriyle geliştirilen, Groq AI entegrasyonuna sahip dinamik portfolyo sitesi. Ziyaretçilerin projelerim ve yetkinliklerim hakkında gerçek zamanlı bilgi alabileceği, yüksek performanslı bir dijital asistan içerir.)" +
                            "Kibar ve teknik bir dil kullan, lafı fazla uzatma,sadece ama sadece Berat Resuloğlu ile ilgili sorulara cevap ver, Berat Resuloğlu ile ilgili olmayan sorulara kibar bir üslupla cevap veremeyeceğini söyle, asla siyaset, ekonomi vb. konularda konuşma, halkı kin ve nefrete yöneltebilecek her türlü söylemden kaçın, müstehcen konular ile ilgili sorulara asla yanıt verme, asla küfür veya argo kullanma." +
                            "Eğer teknik altyapın sorulursa, Groq API üzerinden Llama 3 modelini kullandığını ve Berat tarafından entegre edildiğini söyle."
                        },
                        new {
                            role = "user",
                            content = request.Message
                        }
                    },
                    temperature = 0.7
                };

                var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonPayload = JsonSerializer.Serialize(payload, jsonOptions);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // _grokUrl değişkenini kullanıyoruz
                var response = await client.PostAsync(_grokUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { error = "Grok API hatası", details = errorDetails });
                }

                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Sunucu hatası oluştu", message = ex.Message });
            }
        }
    }

    public class UserRequest
    {
        public string Message { get; set; }
    }
}