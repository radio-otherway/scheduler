using System.Net;
using System.Security.Cryptography.X509Certificates;
using OtherWay.Radio.Scheduler.Services;
using OtherWay.Radio.Scheduler.Services.Jobs;

var builder = WebApplication.CreateBuilder(args);
var cert = builder.Configuration["SSL:Cert"];
var key = builder.Configuration["SSL:Key"];

if (!string.IsNullOrEmpty(cert) && !string.IsNullOrEmpty(key)) {
  builder.WebHost.ConfigureKestrel(options => {
    options.Listen(IPAddress.Any, 5001, listenOptions => {
      var certPem = File.ReadAllText(cert);
      var keyPem = File.ReadAllText(key);
      var x509 = X509Certificate2.CreateFromPem(certPem, keyPem);
      listenOptions.UseHttps(x509);
    });
  });
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClients(builder.Configuration["ServiceSettings:ApiUrl"]);

builder.Services.AddCors(options => {
  options.AddPolicy(name: "AllowSpecificOrigins",
    policy => {
      policy.WithOrigins(
        "https://otherway.dev.fergl.ie:3000",
        "https://radio-otherway.com",
        "https://www.radio-otherway.com");
      policy
        .AllowAnyHeader()
        .AllowCredentials()
        .AllowAnyMethod();
    });
});
builder.Services.AddJobScheduler();
var app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");

app.MapControllers();

app.MapGet("/", () => "Radio Otherway Job Scheduler");
app.Run();