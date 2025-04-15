using System.Data;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Models;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Interfaces;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Database.UnityOfWork;
using Questao5.Infrastructure.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

// sqlite
var stringConnection = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite");
builder.Services.AddSingleton(new DatabaseConfig { Name = stringConnection });
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
//builder.Services.AddScoped(_ => new SqliteConnection(stringConnection));
builder.Services.AddScoped<IDbConnection>(_ => new SqliteConnection(stringConnection));
builder.Services.AddScoped<IUnityOfWork>(x => new DapperUnityOfWork(x.GetRequiredService<IDbConnection>()));

//commands
builder.Services.AddScoped<IRequestHandler<AdicionarMovimentoRequest, AdicionarMovimentoResponse>, AdicionarMovimentoHandler>();

//Queries
builder.Services.AddScoped<IRequestHandler<ObterSaldoRequest, ObterSaldoResponse>, ObterSaldoHandler>();

//repositories
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IIdempontenciaRepository, IdempontenciaRepository>();
builder.Services.AddScoped<ISaldoRepository, SaldoRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Api de movimentacao de conta",
        Description = "Esta API eh um exemplo de um cadastro de movimento",
        Contact = new OpenApiContact() { Name = "Lucas Sampaio", Email = "lucasssouza29@hotmail.com" },
        License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
    });

    // Configuração para processar os comentários do XML da documentação do projeto
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// sqlite
#pragma warning disable CS8602 // Dereference of a possibly null reference.
app.Services.GetService<IDatabaseBootstrap>().Setup();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

app.Run();

// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html