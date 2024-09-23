namespace MatrixRemote_RemoteAPI.Logging
{
    public class LoggingV2 : ILogging
    {

        public void Log(string message, string type)
        {
            if (type == "error")
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR - " + message);
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                if (type == "warning")
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Error - " + message);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.WriteLine(message);
                }
                
            }
        }

        public void LogInformation(string message, int count)
        {
            Console.WriteLine(count);
        }
    }
}
