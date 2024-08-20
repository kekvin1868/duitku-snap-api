using FastEndpoints;
using FastEndpoints.Swagger;
using Refit;
using Serilog;
using SnapFunctionalTest;
using SnapFunctionalTest.Handler;
using SnapFunctionalTest.Interface;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument();

builder.Services.Configure<SnapSettings>(
    builder.Configuration.GetSection(SnapSettings.Position));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISnapTokenStore, SnapTokenStore>();
builder.Services.AddScoped<AuthSignatureHandler>();
builder.Services.AddScoped<TestSignatureHandler>();

builder.Services.AddMvcCore().AddApiExplorer();

builder.Services.AddRefitClient<ISnapTokenApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://snapdev.duitku.com"));
builder.Services.AddRefitClient<ISnapVaApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://snapdev.duitku.com"))
    .AddHttpMessageHandler<AuthSignatureHandler>();
builder.Services.AddRefitClient<ISnapTest>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5191"))
    .AddHttpMessageHandler<TestSignatureHandler>();

var app = builder.Build();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.MapGet("/", () => "Hello World!");

app.Run();