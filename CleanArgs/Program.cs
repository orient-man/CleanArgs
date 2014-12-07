using System;
using CSharpArgs;

namespace CleanArgs
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var arg = new Args("l,p#,d*", args);
                var logging = arg.GetBoolean('l');
                var port = arg.GetInt('p');
                var directory = arg.GetString('d');
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