namespace MatrixRemote_RemoteAPI.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
            if (type == "error")
            {
                Console.WriteLine("ERROR - " + message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public void LogInformation(string message, int count)
        {
            
            Console.WriteLine(message, count);
           
        }
    }
}
