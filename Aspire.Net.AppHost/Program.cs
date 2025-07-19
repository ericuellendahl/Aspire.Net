var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Aspire_Net_ApiService>("apiservice");

builder.AddProject<Projects.Apire_Worker>("worker");

builder.AddProject<Projects.Aspire_ApiGateway>("apigateway");

builder.AddProject<Projects.Aspire_Net_Web>("webfrontend")
       .WithExternalHttpEndpoints();


builder.Build().Run();
