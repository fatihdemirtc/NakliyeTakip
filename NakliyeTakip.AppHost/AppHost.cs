var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.NakliyeTakip_Location_API>("nakliyetakip-location-api");


builder.Build().Run();
