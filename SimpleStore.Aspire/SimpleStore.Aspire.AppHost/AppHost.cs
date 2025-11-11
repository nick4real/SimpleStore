var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("simplestore-db");

var api = builder.AddProject<Projects.SimpleStore_API>("simplestore-api")
    .WithReference(postgresdb)
    .WaitFor(postgres);

var webapp = builder.AddProject<Projects.SimpleStore_WebApp>("simplestore-webapp")
    //.WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
