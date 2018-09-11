using System;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ToyRobotSimulator
{
    using Decorators;
    using System.Text.RegularExpressions;

    public class Program
    {
        static IConfigurationRoot _configuration;
        static ServiceProvider _serviceProvider;

        public static void Main(string[] args)
        {
            Configure();

            var robot = _serviceProvider.GetService<IRobot>();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Enter an empty command twice to exit");
            Console.ResetColor();

            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Program will end.\r\nPress enter to confirm or enter another command to continue.");
                    Console.ResetColor();

                    input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) return;
                }

                if (input.Equals("MOVE", StringComparison.InvariantCultureIgnoreCase))
                    robot.MoveForward();

                else if (input.Equals("LEFT", StringComparison.InvariantCultureIgnoreCase))
                    robot.TurnLeft();

                else if (input.Equals("RIGHT", StringComparison.InvariantCultureIgnoreCase))
                    robot.TurnRight();

                else if (input.Equals("REPORT", StringComparison.InvariantCultureIgnoreCase))
                {
                    var report = robot.Report();
                    Console.ForegroundColor = report == null ? ConsoleColor.DarkRed : ConsoleColor.Green;
                    Console.WriteLine(report?.ToString() ?? "Robot has not been placed on the table.");
                    Console.ResetColor();
                }

                else if (Regex.IsMatch(input, "^PLACE\\s+\\d+\\,\\s*\\d+\\s+\\w+\\s*$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500)))
                {
                    var match = Regex.Match(input, "^PLACE\\s+(?'X'\\d+)\\s*\\,\\s*(?'Y'\\d+)\\s+(?'F'\\w+)\\s*$");
                    var location = new Point(int.Parse(match.Groups["X"].Value), int.Parse(match.Groups["Y"].Value));
                    var orientation = Enum.GetNames(typeof(Direction))
                        .SingleOrDefault(n => n.Equals(match.Groups["F"].Value, StringComparison.InvariantCultureIgnoreCase));

                    if (orientation == null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Unrecognized orientation:\t{match.Captures[3].Value}");
                        Console.ResetColor();
                    }
                    else
                        robot.Place(new Placement(location, Enum.Parse<Direction>(orientation)));
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Unrecognized command:\t{input}");
                    Console.ResetColor();
                }
            }
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
                .AddSingleton<IEnvironment>(s => new BasicTableSurface(southwest, northeast))
                .AddTransient<IRobot>(s => new ConstrainedRobot(new Robot(s.GetService<ILogger<Robot>>()),
                    s.GetRequiredService<IEnvironment>(),
                    s.GetService<ILogger<ConstrainedRobot>>()));
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
