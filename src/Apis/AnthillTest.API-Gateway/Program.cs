using AnthillTest.API_Gateway.Extensions;
using AnthillTest.API_Gateway.Handlers;
using AnthillTest.API_Gateway.Middlewares;
using AnthillTest.API_Gateway.Models.LogModels;
using AnthillTest.API_Gateway.Services.ElasticSearch.Abstract;
using AnthillTest.API_Gateway.Services.ElasticSearch.Concrete;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
var assembly = typeof(Program).Assembly.GetName().Name;
IWebHostEnvironment environment = builder.Environment;

var config = ConfigurationExtension.AppConfig;

builder.Configuration.AddConfiguration(config);

builder.Host.AddHostExtensions(environment);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API-gateway", Version = "v1" });
    options.DocInclusionPredicate((docName, description) => true);
    options.CustomSchemaIds(type => type.ToString());
});


builder.Services.AddSingleton<IElasticSearchService, ElasticSearchService>();

builder.Services.AddElasticSearchConfig();
builder.Services.AddLogging();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureCors();
builder.Services.ConfigureAuthentication(config);
builder.Services.AddOcelot(configuration).AddPolly();

builder.Services.AddTransient<HttpClientDelegatingHandler>();

var serviceProvider = builder.Services.BuildServiceProvider();
var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

var elasticSearchService = scope.ServiceProvider.GetRequiredService<IElasticSearchService>();
var elasticLogOptions = configuration.GetSection("ElasticSearchLogOptions").Get<ElasticSearchLogOptions>();
await elasticSearchService.CreateIndexAsync<LogDetail>(elasticLogOptions.LogIndex);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureCustomExceptionMiddleware();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

var ocelotConfig = new OcelotPipelineConfiguration
{
    AuthorizationMiddleware = GatewayAuthorizationMiddleware.Authorize
};
await app.UseOcelot(ocelotConfig);

app.MapControllers();

app.Run();
