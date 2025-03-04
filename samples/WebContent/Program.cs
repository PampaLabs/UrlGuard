using Microsoft.Extensions.Options;

using UrlGuard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SignedUrlOptions>(builder.Configuration.GetSection("SignedUrl"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!").AddSignedUrlEndpointFilter();

app.MapGet("/backdoor", (IOptions<SignedUrlOptions> options, HttpContext httpContext) =>
{
    var guard = new SignedUrlGuard(options.Value.Secret);

    var targetUri = new Uri("/", UriKind.Relative);
    var signedUrl = guard.GenerateSignedUrl(targetUri, TimeSpan.FromSeconds(5), httpContext.Connection.RemoteIpAddress!);

    return Results.Redirect(signedUrl.ToString());
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
