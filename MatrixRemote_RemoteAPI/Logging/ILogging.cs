namespace MatrixRemote_RemoteAPI.Logging
{
    public interface ILogging
    {
        public void Log(string message, string type);
        void LogInformation(string message, int count); 
    }
}
