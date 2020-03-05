using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using ReflectionMethodFactory;
using Microsoft.Extensions.Configuration;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var Configuration = ReadFromAppSettings();

            var methodInvoker = new MethodFactory(new MethodFactoryConfiguration()
            {
                AssemblyPrefix = new List<string>() { "CustomMethod" },
                AssemblySuffix = new List<string>() { "MethodLib" },
                AssemblyRelatedPath = Configuration["AssemblyRelatedPath"] ?? string.Empty
            });

            var regex = new Regex(@"(?<first>[0-9 ]+)(?<expression>\+|\-|\*|\/)(?<second>[0-9 ]+)", RegexOptions.Compiled);

            while (true)
            {
                Console.WriteLine($"Key in any arithmetic like '123 + 321' or 'Exit' to leave.");

                var input = Console.ReadLine();

                if(input.Equals("Exit", StringComparison.InvariantCultureIgnoreCase)) break;

                var match = regex.Match(input);

                if(!match.Success)
                {
                    Console.WriteLine($"Format error!");
                    continue;
                }

                var firstNumber = Convert.ToInt32(match.Groups["first"].Value);
                var secondNumber = Convert.ToInt32(match.Groups["second"].Value);
                var expression = match.Groups["expression"].Value.ToString();

                // Get Method
                var method = methodInvoker.GetInvokeMethod(GetMethodName(expression), null);

                // Call function
                var returnResult = (int)method.DynamicInvoke(new object[] { firstNumber, secondNumber });

                Console.WriteLine($"The answer is : {returnResult}");
            }
        }

        public static IConfigurationRoot ReadFromAppSettings()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static string GetMethodName(string expression)
        {
            var methodName = string.Empty;

            switch(expression)
            {
                case "+":
                    methodName = "AddtionProcess";
                    break;
                case "-":
                    methodName = "MinusProcess";
                    break;
                case "*":
                    methodName = "MultiplicationProcess";
                    break;
                case "/":
                    methodName = "DivisionProcess";
                    break;
            }

            return methodName;
        }
    }
}
