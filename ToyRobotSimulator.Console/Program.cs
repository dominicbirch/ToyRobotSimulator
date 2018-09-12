using System;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ToyRobotSimulator
{
    using Decorators;
    using System.IO;
    using System.Text.RegularExpressions;
    using ToyRobotSimulator.Commands;

    public class Program
    {
        static IConfigurationRoot _configuration;
        static ServiceProvider _serviceProvider;

        public static void Main(string[] args)
        {
            Configure();

            // Started with path(s) or command(s)
            if (args.Any()) ProcessArgs(args);

            var robot = _serviceProvider.GetRequiredService<IRobot>();
            var parser = _serviceProvider.GetRequiredService<ICommandParser<IRobot>>();

            Print("=== INTERACTIVE MODE ===", ConsoleColor.Cyan);
            Print("Enter an empty command twice to exit.", ConsoleColor.DarkGray);
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Print("Program will end.\r\nPress enter to confirm or enter another command to continue.", ConsoleColor.DarkBlue, ConsoleColor.White);

                    input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) return;
                }

                if (parser.TryParse(input, out var command))
                    command.Execute(robot);

                else
                    Print($"Unrecognized command: {input}", ConsoleColor.DarkRed);
            }
        }


        static void ProcessArgs(string[] args)
        {
            var robot = _serviceProvider.GetRequiredService<IRobot>();
            var parser = _serviceProvider.GetRequiredService<ICommandParser<IRobot>>();

            foreach (var arg in args)
                if (File.Exists(arg))
                {
                    Print($"=== FILE: {arg} ===", ConsoleColor.Cyan);

                    // Input from file, builds a single command representing all valid commands, then executes it
                    using (var reader = File.OpenText(arg))
                        reader
                            .ReadToEnd()
                            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                            .Where(command => !string.IsNullOrWhiteSpace(command))
                            .Aggregate(_serviceProvider.GetService<ICommandBuilder<IRobot>>(), (builder, input) =>
                            {
                                if (parser.TryParse(input, out var command))
                                    builder.Add(command);

                                return builder;
                            })
                            .Build()
                            .Execute(robot);
                }
                // Otherwise try to parse a single command
                else if (parser.TryParse(arg, out var command)) command.Execute(robot);
                else if (!string.IsNullOrWhiteSpace(arg)) Print($"Unrecognized command: {arg}", ConsoleColor.DarkRed);

            Print("End of commandline input.", ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        static void Print(string message = "", ConsoleColor colour = ConsoleColor.White, ConsoleColor backgroundColour = ConsoleColor.Black)
        {
            Console.ForegroundColor = colour;
            Console.BackgroundColor = backgroundColour;
            Console.WriteLine(message);
            Console.ResetColor();
        }


        static void Configure()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(b => b.AddConsole().SetMinimumLevel(Enum.Parse<LogLevel>(_configuration["Logging:Level"])))
                .AddOptions();

            services
                .AddTransient(typeof(Lazy<>), typeof(LazyService<>))
                .AddTransient(typeof(IFactory<>), typeof(ServiceFactory<>));

            var southwest = new Point(
                _configuration.GetValue<int>("TableDimensions:Southwest:X"),
                _configuration.GetValue<int>("TableDimensions:Southwest:Y"));
            var northeast = new Point(
                _configuration.GetValue<int>("TableDimensions:Northeast:X"),
                _configuration.GetValue<int>("TableDimensions:Northeast:Y"));

            services
                .AddSingleton<RobotCommandParser.RobotReportCallback>(report => Print(report?.ToString() ?? "Robot has not been placed on the table.", report == null ? ConsoleColor.DarkRed : ConsoleColor.Green))
                .AddSingleton<ICommandParser<IRobot>, RobotCommandParser>()
                .AddTransient(typeof(ICommandBuilder<>), typeof(CommandBuilder<>))
                .AddSingleton<IEnvironment>(s => new BasicTableSurface(southwest, northeast));

            // C#7 local function in place of Func<IServiceProvider, IRobot> ^.^/
            IRobot initializeRobot(IServiceProvider s) =>
                new ConstrainedRobot(new Robot(s.GetService<ILogger<Robot>>()),
                    s.GetRequiredService<IEnvironment>(),
                    s.GetService<ILogger<ConstrainedRobot>>());

            if (_configuration.GetValue<bool>("NewRobotForInteractive"))
                services.AddTransient(initializeRobot);
            else
                services.AddSingleton(initializeRobot);
        }



        public class ServiceFactory<T> : IFactory<T>
        {
            readonly IServiceProvider _provider;

            public ServiceFactory(IServiceProvider provider)
            {
                _provider = provider;
            }

            // T should be registered as transient
            public T Create() => _provider.GetRequiredService<T>();
        }

        public class LazyService<T> : Lazy<T>
        {
            public LazyService(IServiceProvider provider)
                : base(provider.GetRequiredService<T>)
            {
            }
        }
    }
}
