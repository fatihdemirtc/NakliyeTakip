using Projects;
using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);


#region MongoDB
var mongoUser = builder.AddParameter("MONGO-USERNAME");
var mongoPassword = builder.AddParameter("MONGO-PASSWORD");
var mongoLocationDb = builder.AddMongoDB("mongo-db-location", 27930, mongoUser, mongoPassword)
    .WithImage("mongo:8.0-rc").WithDataVolume("mongo.db.location.volume").AddDatabase("location-db");
var locationApi = builder.AddProject<Projects.NakliyeTakip_Location_API>("nakliyetakip-location-api");
locationApi.WithReference(mongoLocationDb).WaitFor(mongoLocationDb); 
#endregion

#region Keycloak

var postgresUser = builder.AddParameter("POSTGRES-USER");
var postgresPassword = builder.AddParameter("POSTGRES-PASSWORD");

var postgresDb = builder
    .AddPostgres("postgres-db-keycloak", postgresUser, postgresPassword, 5432)
    .WithImage("postgres", "16.2")
    .WithVolume("nakliyetakip_postgres.db.keycloak.volume", "/var/lib/postgresql/data").AddDatabase("keycloak-db", databaseName: "keycloak_db");


var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "25.0")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "password")
    .WithEnvironment("KC_DB", "postgres")
    .WithEnvironment("KC_DB_URL", "jdbc:postgresql://postgres-db-keycloak/keycloak_db")
    .WithEnvironment("KC_DB_USERNAME", postgresUser)
    .WithEnvironment("KC_DB_PASSWORD", postgresPassword)
    .WithEnvironment("KC_HOSTNAME_PORT", "8080")
    .WithEnvironment("KC_HOSTNAME_STRICT_BACKCHANNEL", "false")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME_STRICT_HTTPS", "false")
    .WithEnvironment("KC_HOSTNAME_STRICT", "false")
    .WithEnvironment("KC_HEALTH_ENABLED", "true")
    .WithArgs("start").WaitFor(postgresDb)
    .WithEndpoint("keycloak-http-endpoint", endpoint =>
    {
        endpoint.Port = 8080;         // host port
        endpoint.TargetPort = 8080;   // container port
        endpoint.IsExternal = true;   // DIÞ DÜNYAYA AÇ!
    });


var keycloakEndpoint = keycloak.GetEndpoint("keycloak-http-endpoint");

#endregion

#region Gateway-API

builder.AddProject<NakliyeTakip_Gateway>("nakliyetakip-gateway").WithReference(keycloakEndpoint)
    .WaitFor(keycloak);

#endregion



builder.Build().Run();
