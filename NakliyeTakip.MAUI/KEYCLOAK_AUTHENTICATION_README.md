# Keycloak Authentication - .NET MAUI Projesi

## 🚀 Kurulum Tamamlandı!

### ✅ Eklenen Özellikler

1. **Authentication Servisleri**
   - `IAuthenticationService` - Authentication interface
   - `KeycloakAuthenticationService` - Keycloak OAuth2/OIDC implementasyonu
   - `WebAuthenticatorBrowser` - MAUI WebAuthenticator entegrasyonu
   - `AuthenticationDelegatingHandler` - HTTP isteklerine otomatik token ekleme

2. **Sayfalar**
   - `LoginPage` - Modern ve kullanıcı dostu giriş sayfası
   - `HomePage` - Token bilgileri ve çıkış yap sayfası

3. **Platform Konfigürasyonları**
   - Android: `MainActivity.cs` ve `AndroidManifest.xml` güncellendi
   - iOS: `Info.plist` custom URL scheme için ayarlandı

### 📦 Yüklenen NuGet Paketleri

- `IdentityModel.OidcClient` (v6.0.0)
- `Microsoft.Extensions.Http` (v9.0.0)

### ⚙️ Yapılandırma Gereken Ayarlar

#### 1. Keycloak Sunucu Bilgileri
`NakliyeTakip.MAUI\Services\KeycloakAuthenticationService.cs` dosyasında:

```csharp
Authority = "http://http://192.168.68.100:8080/realms/NakliyeTenant", // Keycloak realm URL'nizi buraya yazın
ClientId = "mobile-public", // Keycloak'ta oluşturacağınız client ID
```

#### 2. Gateway API URL
`NakliyeTakip.MAUI\MauiProgram.cs` dosyasında:

```csharp
client.BaseAddress = new Uri("http://192.168.68.100:5175"); // Gateway URL'nizi buraya yazın
```

### 🔐 Keycloak Client Ayarları

Keycloak Admin Console'da şu ayarları yapın:

1. **Yeni Client Oluşturun:**
   - Client ID: `maui-app`
   - Client Protocol: `openid-connect`
   - Access Type: `public`
   - Standard Flow Enabled: `ON`
   - Valid Redirect URIs: `myapp://callback`
   - Valid Post Logout Redirect URIs: `myapp://callback`
   - Web Origins: `*`

2. **Client Scopes:**
   - `openid` ✓
   - `profile` ✓
   - `email` ✓
   - `offline_access` ✓ (Refresh token için gerekli)

### 🎯 Özellikler

#### LoginPage
- ✅ Modern ve responsive tasarım
- ✅ Keycloak ile OAuth2/OIDC authentication
- ✅ Loading indicator ile kullanıcı geri bildirimi
- ✅ Hata yönetimi ve kullanıcı dostu mesajlar
- ✅ Otomatik yönlendirme (zaten giriş yapılmışsa)

#### HomePage
- ✅ Token bilgilerini görüntüleme
- ✅ Token'ı panoya kopyalama
- ✅ Token yenileme (refresh token)
- ✅ Güvenli çıkış (logout)
- ✅ Otomatik token süre kontrolü

#### AuthenticationService
- ✅ Access token yönetimi
- ✅ Refresh token ile otomatik yenileme
- ✅ Secure storage (SecureStorage) ile token saklama
- ✅ Token süre sonu kontrolü

### 🔧 Kullanım

#### 1. Android Emulator'de Test
```bash
dotnet build -t:Run -f net9.0-android
```

#### 2. iOS Simulator'de Test
```bash
dotnet build -t:Run -f net9.0-ios
```

### 📱 Custom URL Scheme

Uygulama `myapp://callback` URL scheme'ini kullanıyor. OAuth callback için gerekli.

**Değiştirmek isterseniz:**
1. `KeycloakAuthenticationService.cs` → `RedirectUri`
2. Android `MainActivity.cs` → `[IntentFilter]` attribute
3. iOS `Info.plist` → `CFBundleURLSchemes`
4. Keycloak Client → `Valid Redirect URIs`

### 🌐 Network Güvenliği

**Android için HTTP izni:**
- `AndroidManifest.xml` dosyasında `INTERNET` permission mevcut
- Localhost test için `RequireHttpsMetadata = false` kullanılıyor

**Production için:**
- HTTPS kullanın
- `RequireHttpsMetadata = true` yapın
- SSL sertifikası ekleyin

### 🐛 Sorun Giderme

#### Problem: "Login was cancelled"
- Kullanıcı giriş işlemini iptal etti
- Normal davranış, hata değil

#### Problem: "Authority must use HTTPS"
- Development için `RequireHttpsMetadata = false` ayarlanmış
- Production'da HTTPS kullanın

#### Problem: Token alınamıyor
- Keycloak server'ın çalıştığından emin olun
- Client ID ve Realm adının doğru olduğunu kontrol edin
- Redirect URI'nin Keycloak'ta kayıtlı olduğunu kontrol edin

#### Problem: Build hatası (XAML)
- Clean solution: `dotnet clean`
- Rebuild: `dotnet build`

### 📚 Daha Fazla Bilgi

- [IdentityModel.OidcClient Documentation](https://identitymodel.readthedocs.io/en/latest/)
- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [.NET MAUI WebAuthenticator](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/authentication)

### 🎨 UI Özelleştirme

Renkleri `Resources/Styles/Colors.xaml` dosyasından değiştirebilirsiniz.

Sayfa tasarımlarını XAML dosyalarından özelleştirebilirsiniz:
- `Pages/LoginPage.xaml`
- `Pages/HomePage.xaml`

---

**Hazırlayan:** GitHub Copilot
**Tarih:** 18.11.2025
**Proje:** NakliyeTakip - MAUI Mobile App
