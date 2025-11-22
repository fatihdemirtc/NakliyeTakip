# NakliyeTakip Mobil – Keycloak – Gateway – Location Microservice Ýletiþim README

Bu doküman .NET MAUI mobil uygulamasýnýn Keycloak kimlik saðlayýcýsý, Gateway (YARP Reverse Proxy) ve Location mikroservisi ile nasýl haberleþtiðini mimari akýþlarý, yapýlandýrmalarý ve örnek isteklerle açýklar.

## 1. Genel Mimari
```
[MAUI App] --(OAuth2/OIDC PKCE)--> [Keycloak Realm: NakliyeTenant]
[MAUI App] --(Bearer Access Token)--> [Gateway (YARP)] --(Forward)--> [Location.API]
                                 \--(Forward)--> [Diðer Servisler]

Keycloak --> JWT (aud: location.api, iss: realm URL)
Gateway   --> Authentication & Authorization (JwtBearer) + Reverse Proxy
Location.API --> Ýþ mantýðý + Konum kayýtlarý
```

## 2. Kimlik Doðrulama Akýþý (Authorization Code + PKCE)
1. MAUI uygulamasý `OidcClient` ile `mobile-public` (public client, PKCE açýk) istemcisine yönlendirir.
2. Kullanýcý Keycloak login sayfasýnda kullanýcý adý/þifre girer.
3. Keycloak redirect URI (`myapp://auth`) ile `code` döner.
4. `OidcClient` PKCE doðrulamasý ile `code` ? `access_token (+ optional refresh_token)` deðiþimini yapar.
5. Access token SecureStorage’da tutulur; her istek öncesi `AuthenticationDelegatingHandler` / özel HttpClient bearer header ekler.

### Önemli Keycloak Ýstemci Ayarlarý (mobile-public)
- `standardFlowEnabled = true`
- `publicClient = true`
- `redirectUris = [ "myapp://auth" ]`
- `pkce.code.challenge.method = S256`
- Audience / resource eriþimi için Keycloak’ta `oidc-audience-mapper` ile `location.api` eklenir.

## 3. Token Yapýsý
Örnek JWT (kýsaltýlmýþ):
```json
{
  "iss": "http://192.168.68.100:8080/realms/NakliyeTenant",
  "sub": "066ee238-d5c2-4c53-b14e-f51446975158",
  "preferred_username": "fatihh",
  "aud": ["account", "location.api"],
  "realm_access": {"roles": ["user"]},
  "exp": 1730000000
}
```
`sub` claim’i mobilde kullanýcý kimliðini (userId) üretmek / gönderimlerde kullanmak için çýkarýlýr.

## 4. Gateway (YARP) Katmaný
Gateway:
- `AddAuthentication(JwtBearer)` ile `Authority = Keycloak realm base`
- `Audience` doðrulamasý.
- Route / cluster tanýmlarý ile `/v1/locations` ? `Location.API` proxy.
- Rol & policy tanýmlarý ile eriþim kontrolü (ör. `AdminPolicy`).

## 5. Location Microservice Ýstek Akýþý
Mobil uygulama konum gönderme örneði:
```
POST {GatewayBase}/v1/locations
Authorization: Bearer <ACCESS_TOKEN>
Content-Type: application/json

{
  "longitude": 29.123456,
  "latitude": 41.234567,
  "userId": "066ee238-d5c2-4c53-b14e-f51446975158"
}
```
Gateway token’ý doðrular ? YARP ile Location.API’ye iletir ? Location.API JWT içindeki kullanýcý bilgilerini isteðe göre (ör. `sub`) kullanabilir.

## 6. MAUI Tarafý Bileþenleri
| Bileþen | Amaç |
|--------|------|
| `KeycloakAuthenticationService` | Login, token yenileme, logout |
| `WebAuthenticatorBrowser` | Yerel tarayýcý / sistem UI ile OIDC yönlendirme |
| `AuthenticationDelegatingHandler` | Her HttpClient isteðine Bearer token ekler |
| `ILocationRefitService` | Refit ile REST arayüzü |
| `BackgroundLocationService` | Her 10 sn’de konum + token gönderimi |
| `LocationService` | Cihaz konumunu Geolocation API ile alýr |
| `GatewayOption` | BaseAddress yapýlandýrmasý |

## 7. Örnek Refit Arayüzü (Varsayýmsal)
```csharp
public interface ILocationRefitService
{
    [Post("/v1/locations")]
    Task<HttpResponseMessage> PostLocationAsync([Body] LocationSendDto dto);
}

public record LocationSendDto(double longitude, double latitude, string userId);
```
Kullaným:
```csharp
var token = await auth.GetAccessTokenAsync();
var userId = ExtractUserId(token);
await locationRefit.PostLocationAsync(new LocationSendDto(lon, lat, userId));
```

## 8. Arka Plan Gönderim Döngüsü
Pseudo:
```csharp
while (true)
{
  token = await auth.GetAccessTokenAsync();
  if (token == null) wait;
  var (lat, lon) = await locationService.GetCurrentLocationAsync();
  var userId = GetSub(token);
  await http.PostJson(Gateway + "/v1/locations", new { lon, lat, userId }, bearer: token);
  await Task.Delay(10s);
}
```

## 9. Hata Yönetimi
| Durum | Çözüm |
|-------|-------|
| Token süresi doldu | `RefreshTokenAsync()` otomatik yenileme veya yeniden login |
| 401 Unauthorized | Yanlýþ audience, realm veya expired token ? yeniden login |
| Konum izni reddi | Fallback (0,0) veya kullanýcýya izin ekraný |
| Að hatasý | Retry sonraki döngüde (background servis devam eder) |

## 10. Geliþtirme Ortamý Ýpuçlarý
- Android Emulator’da `localhost` yerine API için `http://10.0.2.2:<port>` kullanýn.
- Keycloak HTTP eriþimi için OidcClient `Discovery.RequireHttps = false` yalnýzca geliþtirme modunda.
- Ýstemci gizli anahtarý gerekmeyen public client (PKCE) tercih edildi.

## 11. Güvenlik Notlarý
- Production ortamýnda mutlaka HTTPS kullanýn.
- Refresh token saklanacaksa eriþimi sýnýrlý platform secure storage (MAUI SecureStorage) kullanýn.
- Audience kýsýtlamalarý ile mikroservisler arasý eriþimi ayrýþtýrýn.
- Rolleri (realm veya client roles) API policy’lerinde doðrulayýn.

## 12. Hýzlý Test (cURL)
Access token aldýktan sonra:
```bash
curl -X POST \
  -H "Authorization: Bearer $ACCESS" \
  -H "Content-Type: application/json" \
  "$GATEWAY_BASE/v1/locations" \
  -d '{"longitude":29.123456,"latitude":41.234567,"userId":"066ee238-d5c2-4c53-b14e-f51446975158"}'
```
Baþarýlý ise `201 Created` veya `200 OK` döner (servis tasarýmýna göre).

## 13. Sorun Giderme
| Problem | Neden | Çözüm |
|---------|-------|-------|
| Discovery 404 | Realm adý hatalý | `/realms/NakliyeTenant/.well-known/openid-configuration` kontrol et |
| code yok | Redirect URI mismatch | Keycloak client redirect listesinde `myapp://auth` var mý? |
| audience hatasý | Token’da `aud` yok | Audience mapper ekle / client scopes kontrol et |
| Konum her zaman (0,0) | Ýzin yok veya sensör kapalý | Permission iste / cihaz ayarýný aç |

## 14. Geniþletme Önerileri
- SignalR ile gerçek zamanlý lokasyon takibi.
- Token içi ek claim’ler (araç plaka, rol detaylarý).
- Backoff stratejisi ile að hatalarýnda artan bekleme.
- HealthCheck endpoint’lerini Gateway üzerinden expose.

---
Bu doküman NakliyeTakip mobil entegrasyonunun temel iletiþim modelini özetler. Ek servisler eklendikçe benzer pattern (Auth ? Gateway ? Microservice) izlenmelidir.
