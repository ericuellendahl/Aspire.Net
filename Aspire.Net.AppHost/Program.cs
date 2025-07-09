var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Aspire_Net_ApiService>("apiservice");

var worker = builder.AddProject<Projects.Apire_Worker>("worker");

builder.AddProject<Projects.Aspire_Net_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(worker);

builder.Build().Run();
