var builder = DistributedApplication.CreateBuilder(args);

var connectionStringSql = builder.AddConnectionString("sql");

var chatModelId = builder.AddParameter("chatModelId", secret: true);
var gitHubToken = builder.AddParameter("gitHubToken", secret: true);
var endpointAi = builder.AddParameter("endpoint", secret: true);
var embeddingModel = builder.AddParameter("embeddingModel", secret: true);
var visionModel = builder.AddParameter("visionModelId", secret: true);
var googleApiKey = builder.AddParameter("googleApiKey", secret: true);
var googleSearchEngineId = builder.AddParameter("googleSearchEngineId", secret: true);

var useSqlLocal = bool.TryParse(
    builder.Configuration["Parameters:useSqlLocal"] ?? "true",
    out var parseduseSqlLocal) && parseduseSqlLocal;

if (useSqlLocal)
{
    var sqlPassword = builder.AddParameter("sqlpassword", secret: true);

    var sqlServer = builder.AddSqlServer("development-sqlserver")
        .WithHostPort(14330)
        .WithPassword(sqlPassword);
    //.WithLifetime(ContainerLifetime.Persistent);

    var azureSqlData = sqlServer.AddDatabase("FlowerShopDev");
}
var storage = builder.AddAzureStorage("storage").RunAsEmulator(e => e.WithLifetime(ContainerLifetime.Persistent).WithDataVolume());
var blobs = storage.AddBlobs("blobs");

var kafka = ConfigKafKaResource(builder);

var endpointChromaDb = ConfigChromaResource(builder);

var flowershopapi = ConfigShopApi(builder, connectionStringSql
, chatModelId, gitHubToken, endpointAi, embeddingModel, visionModel, googleApiKey, googleSearchEngineId, kafka, endpointChromaDb, blobs);

ConfigShopWebApp(builder, flowershopapi);

ConfigVectorFeederAzureFunction(builder, connectionStringSql, chatModelId, gitHubToken, endpointAi, embeddingModel, kafka, endpointChromaDb);

builder.Build().Run();

static IResourceBuilder<KafkaServerResource> ConfigKafKaResource(IDistributedApplicationBuilder builder)
{
    return builder.AddKafka("kafka")
        .WithKafkaUI(kafkaUI => kafkaUI.WithHostPort(9100))
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume(isReadOnly: false);
}

static EndpointReference ConfigChromaResource(IDistributedApplicationBuilder builder)
{
    var chromaDB = builder.AddContainer("chroma", "chromadb/chroma")
        .WithHttpEndpoint(port: 8000, targetPort: 8000, name: "chromaendpoint")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "https://localhost:21281/")
        .WithEnvironment("OTEL_SERVICE_NAME", "chromadb")
        .PublishAsContainer();

    var endpointChromaDb = chromaDB.GetEndpoint("chromaendpoint");
    return endpointChromaDb;
}

static IResourceBuilder<ProjectResource> ConfigShopApi(IDistributedApplicationBuilder builder
, IResourceBuilder<IResourceWithConnectionString> connectionStringSql
, IResourceBuilder<ParameterResource> chatModelId
, IResourceBuilder<ParameterResource> gitHubToken
, IResourceBuilder<ParameterResource> endpointAi
, IResourceBuilder<ParameterResource> embeddingModel
, IResourceBuilder<ParameterResource> visionModel
, IResourceBuilder<ParameterResource> googleApiKey
, IResourceBuilder<ParameterResource> googleSearchEngineId
, IResourceBuilder<KafkaServerResource> kafka
, EndpointReference endpointChromaDb
, IResourceBuilder<IResourceWithConnectionString> blobs)
{
    return builder.AddProject<Projects.FlowerShop_Api>("flowershop-api")
        .WithEnvironment("GitHubModel__ChatModelId", chatModelId)
        .WithEnvironment("GitHubModel__GitHubToken", gitHubToken)
        .WithEnvironment("GitHubModel__Endpoint", endpointAi)
        .WithEnvironment("GitHubModel__EmbeddingModel", embeddingModel)
        .WithEnvironment("GitHubModel__VisionModelId", visionModel)
        .WithEnvironment("GoogleTextSearchSettings__GoogleApiKey", googleApiKey)
        .WithEnvironment("GoogleTextSearchSettings__GoogleSearchEngineId", googleSearchEngineId)
        .WithReference(connectionStringSql)
        .WithReference(endpointChromaDb)
        .WithReference(kafka)
        .WithReference(blobs)
        .WaitFor(kafka)
        .WithHttpHealthCheck("/health");
}

static void ConfigShopWebApp(IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> flowershopapi)
{
    builder.AddViteApp("flowerspa", "../../../src/flowershopspa/myflowershop")
        .WithReference(flowershopapi)
        .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "https://localhost:21281/")
        .WithEnvironment("OTEL_SERVICE_NAME", "flowerspa")
        .WaitFor(flowershopapi)
        .WithExternalHttpEndpoints()
        .PublishAsDockerFile();
}

static void ConfigVectorFeederAzureFunction(IDistributedApplicationBuilder builder, IResourceBuilder<IResourceWithConnectionString> connectionStringSql, IResourceBuilder<ParameterResource> chatModelId, IResourceBuilder<ParameterResource> gitHubToken, IResourceBuilder<ParameterResource> endpointAi, IResourceBuilder<ParameterResource> embeddingModel, IResourceBuilder<KafkaServerResource> kafka, EndpointReference endpointChromaDb)
{
    builder.AddAzureFunctionsProject<Projects.FlowerShop_VectorInit>("flowershop-vectorinit")
        .WithEnvironment("GitHubModel__ChatModelId", chatModelId)
        .WithEnvironment("GitHubModel__GitHubToken", gitHubToken)
        .WithEnvironment("GitHubModel__Endpoint", endpointAi)
        .WithEnvironment("GitHubModel__EmbeddingModel", embeddingModel)
        .WithReference(connectionStringSql)
        .WithReference(endpointChromaDb)
        .WithReference(kafka)
        .WaitFor(kafka)
        .WithExternalHttpEndpoints();
}