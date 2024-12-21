using Microsoft.OpenApi.Models;
using Product.Api.Extensions;
using Product.Business.Extensions;
using Product.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddBusiness();
builder.Services.AddInfra(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = @"Insert JWT with Bearer in field; Ex: ""Bearer &lt;TOKEN&gt;"".",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    });
}).AddApiVersioning().AddMvc();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
