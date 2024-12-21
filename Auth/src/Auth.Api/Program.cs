using Auth.Api.Extensions;
using Auth.Business.Extensions;
using Auth.Infra.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSecurity();
builder.Services.AddBusiness();
builder.Services.AddInfra(builder.Configuration);
builder.Services.AddControllers();
builder.Services
    .AddSwaggerGen(c =>
    {
        c.ExampleFilters();
    })
    .AddSwaggerExamplesFromAssemblyOf<Program>()
    .AddApiVersioning().AddMvc();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
