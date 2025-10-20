var builder = DistributedApplication.CreateBuilder(args);

//var postgres = builder.AddPostgres("postgres-db");

var api = builder.AddProject<Projects.SimpleStoreAPI>("simplestoreapi");
  //  .WithReference(postgres)
    //.WaitFor(postgres);

var webapp = builder.AddProject<Projects.SimpleStoreWebApp>("simplestorewebapp")
    //.WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();