var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("simplestore-db");

var api = builder.AddProject<Projects.SimpleStore_API>("simplestore-api")
    .WithReference(postgresdb)
    .WaitFor(postgres);

var webapp = builder.AddProject<Projects.SimpleStore_WebApp>("simplestore-webapp")
    .WithReference(api)
    .WaitFor(api);

var gateway = builder.AddProject<Projects.SimpleStore_Gateway>("simplestore-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(webapp)
    .WaitFor(webapp);

builder.Build().Run();