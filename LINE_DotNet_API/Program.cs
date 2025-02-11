using LINE_DotNet_API.Domain;
using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Providers;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: "policy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<JsonProvider>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LINE Services API ",
        Description = "",
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.WebHost.ConfigureKestrel(options =>
{
    var certPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path");
    var certPassword = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password");

    options.ListenAnyIP(8080); // HTTP
    options.ListenAnyIP(8081, listenOptions =>
    {
        if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
        {
            listenOptions.UseHttps(certPath, certPassword);
        }
    });
});


//builder.WebHost.UseKestrel(options =>
//{
//    options.ListenAnyIP(80); // 啟用 HTTP 監聽
//});

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(443, listenOptions =>
//     {
//         listenOptions.UseHttps("./LINE_DotNet_API.pfx", "your_password");
//     });
// });

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(8080); // 讓 asp.net core 在 docker 內部監聽所有 ip
//});

// 讓 ASP.NET Core 正確監聽所有 IP（解決 Docker 無法存取問題）
//builder.WebHost.UseUrls("http://+:8080");

var app = builder.Build();
app.UseCors("policy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // 確保 Swagger URL 是 /swagger
    });
}

app.UseRouting();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "UploadFiles")),
    RequestPath = "/UploadFiles",
});

app.MapControllers();
app.MapGet("/", () => "Hello, Docker!");

app.Run();
