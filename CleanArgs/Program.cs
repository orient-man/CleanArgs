using System;
using CSharpArgs;

namespace CleanArgs
{
    public static class Program
    {
        // Example usage: Args.exe -l -p 4444 -d "C:\Windows\Temp"
        private static void Main(string[] args)
        {
            try
            {
                var arg = new Args("l,p#,d*", args);
                var logging = arg.Get<bool>('l');
                var port = arg.Get<int>('p');
                var directory = arg.Get<string>('d');
                ExecuteApplication(logging, port, directory);
            }
            catch (ArgsException e)
            {
                Console.WriteLine("Argument error: {0}", e.GetErrorMessage());
            }
        }

        private static void ExecuteApplication(bool logging, int port, string directory)
        {
            Console.WriteLine(
                "logging is {0}, port:{1}, directory:{2}",
                logging,
                port,
                directory);
        }
    }
}