using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NickTax.API.Middleware;
using NickTax.Application.Interfaces;
using NickTax.Application.Mappings;
using NickTax.Application.Services;
using NickTax.Application.Validators;
using NickTax.Domain.Interfaces;
using NickTax.Infrastructure.Context;
using NickTax.Infrastructure.Repositories;
using NickTax.Infrastructure.Services;
using Scalar.AspNetCore;
using System.Text;
using NickTax.API.Extensions;
using AutoMapper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]!);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

const string PoliticaCorsReact = "_politicaCorsReact";

// 2. Adicionar o serviço de CORS configurado
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: PoliticaCorsReact,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                  .AllowAnyHeader()                                             
                  .AllowAnyMethod()                                             
                  .AllowCredentials();
        });
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ISenhaService, SenhaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<INotaFiscalService, NotaFiscalService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();
builder.Services.AddScoped<INfeParserService, NfeParserService>();
builder.Services.AddScoped<IFocusClient, FocusNfeClient>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // Em prod mudar pra true
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateUsuarioRequestValidator>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<NickTax.Application.Mappings.MappingProfile>();
    cfg.AddProfile<NickTax.Application.Mappings.EmpresaMappingProfile>();
    cfg.AddProfile<NickTax.Application.Mappings.NotaFiscalMappingProfile>();
});

var app = builder.Build();

app.UseExceptionHandler();

app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors(PoliticaCorsReact);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }