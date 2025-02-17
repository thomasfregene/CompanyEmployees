using CompanyEmployees.Extensions;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigurCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerSevice();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();

builder.Services.ConfigureSqlContext(builder.Configuration);

/*With this, we are suppressing a default model state validation that is 
implemented due to the existence of the [ApiController] attribute in 
all API controllers*/

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
}).AddXmlDataContractSerializerFormatters()
 .AddCustomCsvFormatter()
.AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

builder.Services.AddAutoMapper(typeof(Program));

/*This function configures support for JSON Patch using 
Newtonsoft.Json while leaving the other formatters unchanged*/
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
    .Services.BuildServiceProvider()
    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
    .OfType<NewtonsoftJsonPatchInputFormatter>().First();

/*the Build method builds the 
WebApplication and registers all the services added with IOC.
*/
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

if(app.Environment.IsProduction())
    app.UseHsts();

// Configure the HTTP request pipeline.
/*if(app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();*/

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
