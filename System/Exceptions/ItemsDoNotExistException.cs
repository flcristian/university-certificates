namespace UniversityCertificates.System.Exceptions
{
    public class ItemsDoNotExistException : Exception
    {
        public ItemsDoNotExistException(string message) : base(message)
        {
        }
    }
}