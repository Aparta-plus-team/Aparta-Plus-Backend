using data_aparta_.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .SetupEntityFrameworkCore(builder.Configuration)
    .SetupGraphQL()
    .ConfugureAWS(builder.Configuration)
    .SetupCors();


var app = builder.Build();

app.UseCors("AllowAnyOrigin");

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
