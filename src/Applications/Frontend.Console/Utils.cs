using System;
using System.Collections.Generic;
using System.Text;

namespace Frontend.Console
{
    public static class Utils
    {
        public static async Task WaitForApisAsync() 
        {
            WriteLine("Waiting for all APIs to start...");
            var reportInterval = TimeSpan.FromSeconds(1);


            for (var remaining = TimeSpan.FromSeconds(10); remaining > TimeSpan.Zero; remaining -= reportInterval)
            {
                WriteLine($"\rWaiting for APIs... Remaining: {remaining:mm\\:ss}   ");

                await Task.Delay(reportInterval);
            }

            WriteLine("\rWaiting for APIs... Remaining: 00:00   ");            
            WriteLine("All waiting time completed.");            
            WriteLine("Resume Assistant is ready.");            
            WriteLine("Type 'exit' to quit.");

            Separator();
        }
        public static void Separator()
        {
            System.Console.WriteLine();
            WriteLine("".PadLeft(System.Console.WindowWidth, '-'));
            System.Console.WriteLine();
        }

        public static void WriteLine(string message) 
        {
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine();
        }

        public static bool TryGetUserInput(out string userInput) 
        {
            System.Console.Write("> ");
            userInput = System.Console.ReadLine();

            if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

    }
}
