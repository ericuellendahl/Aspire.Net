var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.Aspire_Net_ApiService>("apiservice");

builder.AddProject<Projects.Apire_Worker>("worker");

var apigateway = builder.AddProject<Projects.Aspire_ApiGateway>("apigateway");

var worker = builder.AddProject<Projects.Apire_Worker>("worker");

builder.AddProject<Projects.Aspire_Net_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(apiservice)
       .WithReference(apigateway);


builder.Build().Run();
