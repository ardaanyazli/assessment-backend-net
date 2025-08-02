# ContactBook Çözümü

.NET 9 ile geliştirilmiş, mikroservis mimarisi, olay odaklı iletişim ve kapsamlı raporlama özellikleri içeren kapsamlı bir iletişim yönetim sistemi.

## 🏗️ Mimari Genel Bakış

Bu çözüm, Clean Architecture (Temiz Mimari) prensiplerini takip eder ve aşağıdaki bileşenlerle bir mikroservis yapısını uygular:

### Temel Servisler
- **ContactBook.Contacts.API** - Kişi yönetimi için RESTful API
- **ContactBook.Reports.API** - Rapor oluşturma ve alma için RESTful API
- **ContactBook.Reports.Consumer** - Rapor isteklerini işleyen arka plan servisi

### Mimari Katmanlar
- **Domain Katmanı** - İş varlıkları ve iş kuralları
- **Application Katmanı** - Arayüzler, DTO’lar ve uygulama servisleri
- **Infrastructure Katmanı** - Veri erişimi, dış servisler ve kalıcılık
- **API Katmanı** - Controller’lar, middleware ve API yapılandırması

## 📁 Proje Yapısı

(proje klasör yapısı görseli korunmuştur)

## 🚀 Özellikler

### Kişi Yönetimi
- ✅ Kişi oluşturma, okuma, güncelleme ve silme
- ✅ İletişim bilgileri (e-posta, telefon, konum) yönetimi
- ✅ Varsayılan iletişim bilgisi belirleme
- ✅ Kapsamlı doğrulama ve hata yönetimi
- ✅ Tüm işlemler için iptal destekli (CancellationToken)

### Raporlama Sistemi
- ✅ Asenkron rapor oluşturma
- ✅ Konuma göre iletişim istatistikleri
- ✅ Konuma göre telefon numarası dağılımı
- ✅ Apache Kafka ile olay tabanlı mimari
- ✅ Rapor durumu takibi (İstendi → İşleniyor → Tamamlandı)

### Teknik Özellikler
- ✅ Clean Architecture ve DDD prensipleri
- ✅ Entity Framework Core & PostgreSQL
- ✅ Apache Kafka mesajlaşma sistemi
- ✅ Kapsamlı hata yönetimi ve loglama
- ✅ İstek iptal desteği
- ✅ Sağlık kontrolleri
- ✅ OpenAPI/Swagger belgeleri
- ✅ Docker ile konteynerleştirme

## 🛠️ Teknoloji Yığını

- **Framework**: .NET 9
- **Veritabanı**: PostgreSQL
- **Mesaj Aracısı**: Apache Kafka
- **ORM**: Entity Framework Core
- **API Belgeleme**: OpenAPI/Swagger
- **Test**: xUnit, Moq, FluentAssertions
- **Konteyner**: Docker & Docker Compose

## 📋 Gereksinimler

- .NET 9 SDK
- Docker ve Docker Compose
- PostgreSQL (ya da Docker ile)
- Apache Kafka (ya da Docker ile)

## 🏃‍♂️ Başlangıç Adımları

1. Repoyu klonlayın  
2. Altyapı servislerini Docker Compose ile başlatın  
3. `appsettings.Development.json` dosyalarında bağlantı bilgilerini güncelleyin  
4. Veritabanı göçlerini (migrations) uygulayın  
5. Servisleri başlatın (Docker veya manuel)  
6. API’lara şu adreslerden erişin:
   - **Contacts API**: http://localhost:5115
   - **Swagger**: http://localhost:5115/swagger
   - **Reports API**: http://localhost:5263
   - **Swagger**: http://localhost:5263/swagger

## 📖 API Kullanım Örnekleri

- Kişi oluştur, oku, güncelle, sil
- Rapor isteği gönder
- Tüm raporları veya belirli bir raporu getir

## 🧪 Testleri Çalıştırma

- Tüm testleri çalıştır: `dotnet test`
- Kapsama raporu üret: `dotnet test --collect:"XPlat Code Coverage"`
- Belirli bir test projesini çalıştır
- Kapsama raporu görselleştirme: `reportgenerator`

## 🏗️ Geliştirme Süreci

Yeni iletişim türü veya rapor tipi eklemek için:
- Domain ve Application katmanlarını güncelle
- EF Migration oluştur ve uygula
- Gerekirse Controller’ı düzenle

## 🔧 Yapılandırma

Servisler çevresel değişkenlerle yapılandırılabilir:
- PostgreSQL bağlantı bilgileri
- Kafka sunucu adresi
- Ortam seçimi (Development/Production)

## 🔍 İzleme & Sağlık Kontrolleri

- `/health` endpoint’leri
- Yapılandırılmış loglama
- Performans metrikleri
- Kafka mesaj işleme günlükleri

## 🚨 Hata Yönetimi

- Global hata middleware’i (`ErrorHandlingMiddleware`)
- İstek iptali middleware’i
- Doğrulama & iş kuralları kontrolü
- Uygun HTTP durum kodları