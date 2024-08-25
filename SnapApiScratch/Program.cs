using FastEndpoints;
using SnapApiScratch.Contracts.Factories;
using SnapApiScratch.Contracts.Helpers;
using SnapApiScratch.Endpoints.VirtualAccounts;
using SnapApiScratch.Endpoints.AllPaymentGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("DuitkuClient", client =>
{
    client.BaseAddress = new Uri("https://snapdev.duitku.com");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<GetBearerToken>(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    var partnerId = "DS19890";
    var privateKeyPath = "/Users/kevin/Desktop/PrivateKey.pem"; // or load from configuration
    return new GetBearerToken(httpClient, partnerId, privateKeyPath);
});

builder.Services.AddScoped<VirtualAccountRequestFactory>();
builder.Services.AddScoped<GetAllPgRequests>();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseFastEndpoints();
app.UseHttpsRedirection();

app.Run();