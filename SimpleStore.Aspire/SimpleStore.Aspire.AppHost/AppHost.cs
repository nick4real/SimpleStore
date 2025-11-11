var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var sqlserverdb = sqlserver.AddDatabase("simplestore-db");

var api = builder.AddProject<Projects.SimpleStore_API>("simplestore-api")
    .WithReference(sqlserverdb)
    .WaitFor(sqlserverdb);

var webapp = builder.AddProject<Projects.SimpleStore_WebApp>("simplestore-webapp")
    .WithReference(api)
    .WaitFor(api);

var gateway = builder.AddProject<Projects.SimpleStore_Gateway>("simplestore-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WithReference(webapp)
    .WaitFor(webapp);

builder.Build().Run();