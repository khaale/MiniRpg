# MiniRpg
Simple console RPG game with CQRS-inspired design

Build requirements:
- .NET Core 2.x

Features:
- Loading settings from configuration file _appsettings.json_
- Scripted formulas in configuration
- Global randomness source with configurable seed, which allows reproducible games

Technologies in use:
- Configuration - [.NET Core configuration and options facilities](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?tabs=basicconfiguration)
- Execution of scripted functions - [Roslyn scripting API](https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples)
- Application components composition - [.NET Core DI container](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

