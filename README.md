Microservices Solution - Backend Developer Task
https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
https://img.shields.io/badge/C%2523-239120?style=for-the-badge&logo=csharp&logoColor=white
https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white
https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white
https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white
https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white

İçindekiler

Proje Hakkında

Mimari Yapı

Teknolojiler

Özellikler

Başlangıç

Docker ile Çalıştırma

API Dökümantasyonu

Monitoring

Test

CI/CD

Katkıda Bulunma

Lisans

Proje Hakkında

Bu proje, modern yazılım mimarisi prensiplerini kullanarak geliştirilmiş, mikroservis tabanlı bir backend çözümüdür. Onion Architecture, CQRS Pattern, Event-Driven Architecture ve SOLID prensipleri gözetilerek tasarlanmıştır.

Proje, gerçek dünya senaryolarında karşılaşılan problemlere çözüm üretmek amacıyla geliştirilmiş olup, aşağıdaki temel yetenekleri sunar:

Merkezi Kimlik Doğrulama (Auth Service): JWT tabanlı authentication ve refresh token mekanizması

Ürün Yönetimi (Product Service): CQRS pattern ile komut ve sorgu ayrımı, Redis cache optimizasyonu

Merkezi Log Yönetimi (Log Service): Structured logging ile tüm servis loglarının toplanması

API Gateway: YARP ile merkezi routing, rate limiting ve güvenlik

Event-Driven Communication: RabbitMQ ile servisler arası asenkron iletişim

Mimari Yapı

Onion Architecture Katmanları
text
├── AuthService
│   ├── AuthService.Domain      # Entities, Interfaces, Enums
│   ├── AuthService.Application  # Commands, Queries, DTOs, Interfaces
│   ├── AuthService.Infrastructure # Repositories, Services, Data Context
│   └── AuthService.API         # Controllers, Middleware, Program.cs
├── ProductService
│   ├── ProductService.Domain    # Product entities, Events, Interfaces
│   ├── ProductService.Application # CQRS Handlers, DTOs, Validators
│   ├── ProductService.Infrastructure # Repositories, Cache, MessageBus
│   └── ProductService.API      # ProductsController, Program.cs
├── LogService
│   ├── LogService.Domain        # Log entities
│   ├── LogService.Application   # Log commands/queries
│   ├── LogService.Infrastructure # Seq integration, Repositories
│   └── LogService.API          # LogsController
└── ApiGateway
    └── YarpGateway             # Reverse proxy configuration
Veri Akışı

graph TD
    A[Client] --> B[API Gateway]
    B --> C[Auth Service]
    B --> D[Product Service]
    B --> E[Log Service]
    D --> F[(SQL Server)]
    D --> G[(Redis Cache)]
    D --> H[RabbitMQ]
    H --> E
    E --> I[(Seq)]

Teknolojiler

Teknoloji	Kullanım Amacı	Versiyon
.NET 8	Ana framework	8.0
C#	Programlama dili	12.0
Entity Framework Core	ORM ve veritabanı işlemleri	8.0
SQL Server LocalDB	Geliştirme veritabanı	-
MediatR	CQRS pattern implementasyonu	12.2.0
AutoMapper	Nesne eşleme	13.0.1
FluentValidation	Request validation	11.9.0
JWT Bearer	Token-based authentication	8.0
Redis	Distributed cache	Alpine
RabbitMQ	Message broker	3.13.7
Seq	Structured logging	2025.2
YARP	Reverse proxy / API Gateway	2.1.0
Docker	Containerization	Latest
Kubernetes	Orchestration (opsiyonel)	-
GitHub Actions	CI/CD pipeline	-

Özellikler

Tamamlanan Özellikler
Auth Service
JWT token üretimi ve doğrulama

Refresh token mekanizması

Microsoft Identity entegrasyonu

Role-based authorization (Admin/User)

Password hashing

Product Service
Onion Architecture implementasyonu

CQRS Pattern (MediatR ile)

Command Handler ile asenkron yazma

Redis Cache ile sorgu optimizasyonu

Cache invalidation stratejisi

Event publishing (ProductCreated, Updated, Deleted)

Log Service
Merkezi log toplama

Structured logging (JSON format)

Log seviyelendirme (Info, Warning, Error, Critical)

Seq entegrasyonu

API Gateway
Rate limiting (kullanıcı bazında)

Routing configuration

JWT doğrulama

Event-Driven Architecture
RabbitMQ entegrasyonu

Event publishing

Queue management

DevOps
Docker containerization

Docker Compose orchestration

CI/CD pipeline (GitHub Actions)

Kubernetes manifests (opsiyonel)

Başlangıç

Gereksinimler
.NET 8.0 SDK

Docker Desktop

Visual Studio 2022 (opsiyonel)

Git

SQL Server LocalDB (veya herhangi bir SQL Server)

Adım 1: Repoyu Clone'la
bash
git clone https://github.com/ibrahimberat/MicroservicesSolution.git
cd MicroservicesSolution
Adım 2: Veritabanlarını Oluştur
bash
# Auth Service için migration
cd AuthService/AuthService.API
dotnet ef database update --context AuthDbContext

# Product Service için migration
cd ../../ProductService/ProductService.API
dotnet ef database update --context ProductDbContext
Adım 3: Docker Container'larını Başlat
bash
cd ..
docker-compose up -d
Adım 4: Servisleri Çalıştır
Terminal 1 - Auth Service:

bash
cd AuthService/AuthService.API
dotnet run
Terminal 2 - Product Service:

bash
cd ProductService/ProductService.API
dotnet run
Terminal 3 - API Gateway (opsiyonel):

bash
cd ApiGateway/YarpGateway
dotnet run

Docker ile Çalıştırma

Tek Komutla Tüm Servisleri Ayağa Kaldır
bash
docker-compose up -d
Container'ları Kontrol Et
bash
docker-compose ps
Log'ları İzle
bash
docker-compose logs -f
Servisleri Durdur
bash
docker-compose down

API Dökümantasyonu

Servisler ayağa kalktıktan sonra Swagger UI üzerinden tüm endpoint'lere erişebilirsin:

Servis	URL	Açıklama
Auth Service	http://localhost:5000/swagger	Kimlik doğrulama işlemleri
Product Service	http://localhost:5002/swagger	Ürün CRUD işlemleri
Log Service	http://localhost:5004/swagger	Log sorgulama
API Gateway	http://localhost:5000/swagger	Tüm servislere tek noktadan erişim
Auth Service Endpoints
Method	Endpoint	Açıklama
POST	/api/Auth/register	Yeni kullanıcı kaydı
POST	/api/Auth/login	Kullanıcı girişi
POST	/api/Auth/refresh-token	Token yenileme
POST	/api/Auth/revoke-token	Token geçersiz kılma
GET	/api/Auth/profile	Kullanıcı profili
Product Service Endpoints
Method	Endpoint	Açıklama	Authorization
GET	/api/Products	Tüm ürünleri listele	JWT Token
GET	/api/Products/{id}	ID'ye göre ürün getir	JWT Token
POST	/api/Products	Yeni ürün ekle	JWT Token
PUT	/api/Products/{id}	Ürün güncelle	JWT Token
DELETE	/api/Products/{id}	Ürün sil	JWT Token

Monitoring

RabbitMQ Management
text
URL: http://localhost:15672
Username: guest
Password: guest
Seq Dashboard
text
URL: http://localhost:5341
Username: admin
Password: Admin123!
Redis CLI
bash
docker exec -it redis-cache redis-cli

KEYS 
GET "ProductService_products:all:all:1:10"

Test

Unit Testleri Çalıştır
bash
dotnet test AuthService/AuthService.Tests
dotnet test ProductService/ProductService.Tests
dotnet test LogService/LogService.Tests
API Testleri (Örnek Curl Komutları)
1. Kullanıcı Kaydı:

bash
curl -X POST "http://localhost:5000/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","password":"Test123!","firstName":"Test","lastName":"User"}'
2. Kullanıcı Girişi:

bash
curl -X POST "http://localhost:5000/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test123!"}'
3. Ürün Ekleme (Token ile):

bash
curl -X POST "http://localhost:5002/api/Products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"name":"Gaming Laptop","price":25000,"stockQuantity":5,"category":"Electronics","sku":"LPT-001"}'

CI/CD
GitHub Actions ile otomatik CI/CD pipeline'ı yapılandırılmıştır:

yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal

Katkıda Bulunma
Fork'la

Feature branch oluştur (git checkout -b feature/amazing-feature)

Değişikliklerini commit et (git commit -m 'Add some amazing feature')

Branch'ini push et (git push origin feature/amazing-feature)

Pull Request aç

Lisans
Bu proje MIT License ile lisanslanmıştır.

İletişim
İbrahim Berat - GitHub

Proje Linki: https://github.com/ibrahimberat/MicroservicesSolution

Teşekkürler
Bu projeyi beğendiyseniz ** bırakmayı unutmayın!

Hazırlayan: İbrahim Berat
Tarih: Mart 2026
Versiyon: v1.0.0