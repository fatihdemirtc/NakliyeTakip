var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.NakliyeTakip_API>("nakliyetakip-api");

builder.AddProject<Projects.NakliyeTakip_Location_API>("nakliyetakip-location-api");

builder.Build().Run();
