using Newtonsoft.Json;
using System.Net;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

// https://github.com/WireMock-Net
// https://www.youtube.com/watch?v=SQRPqBWHeJs&list=PL6tu16kXT9Pr-y-moJz03XkZLH1ir4O_S
// Server for Wiremock.NET
var server = WireMockServer.Start(new WireMockServerSettings
{ 
    Urls = ["http://localhost:9091"],
    StartAdminInterface = true,
    ReadStaticMappings = true,
    ProxyAndRecordSettings = new ProxyAndRecordSettings
    {
        Url = "https://localhost:7051/",
        SaveMapping = true,
        SaveMappingToFile = true,
        ExcludedHeaders = ["Postman-Token", "Cache-Control", "Content-Length", "User-Agent", "Accept", "Connection", "Accept-Encoding"],
    },
    Logger = new WireMockConsoleLogger(),
});

//server.WatchStaticMappings();

Console.WriteLine("Started WireNock.NET server");

//Stub - Test
server.Given(Request.Create()
    .WithPath("/test").UsingGet())
    .RespondWith(Response.Create().WithBody("Welcome to Wiremock .NET Test")
    .WithHeader("Content-Type", "application/json"));

//Stub - Headers
server.Given(Request.Create()
    .WithPath("/headers").UsingGet())
    .RespondWith(Response.Create().WithBody("Welcome to Wiremock .NET headers")
    .WithHeaders(new Dictionary<string, string>
    {
        {"Content-Type", "application/json"},
        {"Accept", "application/json" },
        {"Cache-Control", "no-cache"}
    })
    .WithStatusCode(HttpStatusCode.Accepted));

//Stub - ProductId
server.Given(Request.Create()
    .WithPath(new RegexMatcher("/product/[0-9]+$")).UsingGet())
    .RespondWith(Response.Create().WithBody("Keyboard is the product")
    .WithHeaders(new Dictionary<string, string>
    {
        {"Content-Type", "application/json"},
        {"Accept", "application/json" },
        {"Cache-Control", "no-cache"}
    })
    .WithStatusCode(HttpStatusCode.Accepted));

//Stub - ProductId
server.Given(Request.Create()
    .WithPath(new WildcardMatcher("/product/*")).UsingGet())
    .RespondWith(Response.Create().WithBody("This could be any product")
    .WithHeaders(new Dictionary<string, string>
    {
        {"Content-Type", "application/json"},
        {"Accept", "application/json" },
        {"Cache-Control", "no-cache"}
    })
    .WithStatusCode(HttpStatusCode.Accepted));

//Stub - Login
server.Given(Request.Create()
    .WithPath("/login")
    .UsingGet()
    .WithHeader("Authorization", new WildcardMatcher("Bearer *")))
    .RespondWith(Response.Create().WithBody("Login Successful")
    .WithHeaders(new Dictionary<string, string>
    {
        {"Content-Type", "application/json"},
        {"Accept", "application/json" },
        {"Cache-Control", "no-cache"}
    })
    .WithStatusCode(HttpStatusCode.Accepted));

//Stub - Login
server.Given(Request.Create()
    .WithPath("/login")
    .UsingGet()
    .WithHeader("Authorization", "*", MatchBehaviour.RejectOnMatch))
    .RespondWith(Response.Create().WithBody("Login UnSuccessful")
    .WithHeaders(new Dictionary<string, string>
    {
        {"Content-Type", "application/json"},
        {"Accept", "application/json" },
        {"Cache-Control", "no-cache"}
    })
    .WithStatusCode(HttpStatusCode.Unauthorized));

//Stub - GetAddress
server.Given(Request.Create()
    .WithPath(new WildcardMatcher("/getAddress/*")).UsingGet())
    .RespondWith(Response.Create().WithBodyAsJson(new
    {
        Name = "Bob",
        City = "London",
        Country = "EN",
    })
    .WithHeaders(new Dictionary<string, string>
    {
        {"Content-Type", "application/json"},
        {"Accept", "application/json" },
        {"Cache-Control", "no-cache"}
    })
    .WithStatusCode(HttpStatusCode.Accepted));

//Stub - GetAddress
server.Given(Request.Create()
    .WithPath(new WildcardMatcher("/getAddressFromObject/*")).UsingGet())
    .RespondWith(Response.Create().WithBodyAsJson(new Address("Bob", "London", "EN"))
    .WithHeaders(new Dictionary<string, string>
    {
        {"Content-Type", "application/json"},
        {"Accept", "application/json" },
        {"Cache-Control", "no-cache"}
    })
    .WithStatusCode(HttpStatusCode.Accepted));

server.SaveStaticMappings();

Console.WriteLine("All the Stub Mapping are bound");

Console.WriteLine(JsonConvert.SerializeObject(server.LogEntries, Formatting.Indented));

Console.WriteLine("Hit any key to close the server!");

Console.ReadKey();

public record Address(string Name, string City, string Country);

// http://localhost:8080/__admin/mappings

// Running WireMock as a .NET Tool in CommandLine
// dotnet tool install --global dotnet-wiremock
// dotnet tool install WireMockInspector --global --no-cache --ignore-failed-sources
// dotnet wiremock
// cd to ...\bin\Debug\net8.0 >> dotnet-wiremock --ReadStaticMappings true
// wireMockInspector

//Notice: edit this
//"Name": "Postman-Token",
//"Matchers": [
//{
//  "Name": "RegexMatcher",
//  "Pattern": ".*",
//  "IgnoreCase": true
//}
//],
