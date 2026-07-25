namespace TrimUrlApi.Exceptions
{
    public class WeakPasswordException : ApiException
    {
        public WeakPasswordException(string passwordErrorText)
            : base(passwordErrorText, StatusCodes.Status400BadRequest)
        {
        }
    }
}
