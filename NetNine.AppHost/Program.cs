var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.NetNine>("netnine");

builder.Build().Run();
