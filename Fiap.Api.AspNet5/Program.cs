using Fiap.Api.AspNet5;
using Fiap.Api.AspNet5.Data;
using Fiap.Api.AspNet5.Repository;
using Fiap.Api.AspNet5.Repository.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("databaseUrl");
builder.Services.AddDbContext<DataContext>(
    opt => opt.UseSqlServer(connectionString)
                .EnableSensitiveDataLogging(true)
);

builder.Services.AddScoped<IUsuarioRepository,UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

#region autenticacao
bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
{
    if (expires != null)
    {
        return expires > DateTime.UtcNow;
    }
    return false;
}

var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddAuthentication(x => {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer( j =>
    {
        j.RequireHttpsMetadata = false;
        j.SaveToken = true;
        j.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            LifetimeValidator = CustomLifetimeValidator,
            RequireExpirationTime = true,
        };
    } 
 );

#endregion



builder.Services.AddControllers();


#region version
builder.Services.AddApiVersioning(options =>
{
    options.UseApiBehavior = false; 
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(3, 0);
    options.ApiVersionReader =
        ApiVersionReader.Combine(
            new HeaderApiVersionReader("x-api-version"),
            new QueryStringApiVersionReader(),
            new UrlSegmentApiVersionReader());
});

builder.Services.AddVersionedApiExplorer(setup => {
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});


builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
#endregion


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        //builder.WithOrigins("https://www2.fiap.com.br");
    });

});



var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseApiVersioning();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( c=>
    {
        foreach( var d in provider.ApiVersionDescriptions )
        {
            c.SwaggerEndpoint(
                $"/swagger/{d.GroupName}/swagger.json",
                d.GroupName.ToUpperInvariant());
        }

        c.DocExpansion(DocExpansion.List);
    });
}

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
