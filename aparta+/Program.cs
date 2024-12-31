using data_aparta_.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .SetupEntityFrameworkCore(builder.Configuration)
    .SetupGraphQL()
    .ConfugureAWS(builder.Configuration);


var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
