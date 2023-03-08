// See https://aka.ms/new-console-template for more information

// https://github.com/NLog/NLog/wiki/JsonLayout
// https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-properties-with-Microsoft-Extension-Logging

using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

Console.WriteLine("Hello, World!");

string correlationId = "12345";

LogManager.Setup()
    .SetupExtensions(s => s.RegisterLayoutRenderer("correlationId", (logEvent) => correlationId))
    .LoadConfigurationFromFile("NLog.config");

ILoggerFactory factory = new NLogLoggerFactory();
ILogger logger = factory.CreateLogger("HelloWorld");

var currentUser = new User() { Id = 123, Name = "John Doe" };

var state = new Dictionary<string, object>()
{
    { "CurrentUser", currentUser }
};

using var scope = logger.BeginScope(state);

logger.LogInformation("Lorem ipsum dolor sit amet 1");
logger.LogInformation("Lorem ipsum dolor sit amet 2");

currentUser.Name = "John DOE";
logger.LogInformation("Lorem ipsum dolor sit amet 3");

correlationId = "54321";
logger.LogError(new Exception("Something went wrong"), "Lorem ipsum dolor sit amet");

class User
{
    public int Id { get; set; }
    public string Name { get; set; }

}

// The log.json file should contain something similar to it
//{ "time": "2023-03-08 22:34:26.2116", "level": "INFO", "correlationId": "12345", "nested": { "message": "Lorem ipsum dolor sit amet 1" }, "CurrentUser": { "Id":123, "Name":"John Doe"} }
//{ "time": "2023-03-08 22:34:29.6123", "level": "INFO", "correlationId": "12345", "nested": { "message": "Lorem ipsum dolor sit amet 2" }, "CurrentUser": { "Id":123, "Name":"John Doe"} }
//{ "time": "2023-03-08 22:34:29.6123", "level": "INFO", "correlationId": "12345", "nested": { "message": "Lorem ipsum dolor sit amet 3" }, "CurrentUser": { "Id":123, "Name":"John DOE"} }
//{ "time": "2023-03-08 22:34:29.6123", "level": "ERROR", "correlationId": "54321", "nested": { "message": "Lorem ipsum dolor sit amet", "exception": "System.Exception: Something went wrong" }, "CurrentUser": { "Id":123, "Name":"John DOE"} }