using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

public class ControllerLogging
{
    
    //private static ILoggerFactory _Factory = null;

    public static ILoggerFactory CreateLoggerFactory()
    {
        return LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
    }

    public static ILogger<T> CreateLogger<T>()
    {
        var loggerFactory = CreateLoggerFactory();
        return loggerFactory.CreateLogger<T>();
    }
}
