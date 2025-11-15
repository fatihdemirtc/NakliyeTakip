var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.NakliyeTakip_API>("nakliyetakip-api");

builder.Build().Run();
