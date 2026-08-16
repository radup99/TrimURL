namespace TrimUrlApi.Exceptions
{
    public class InvalidFieldException : ApiException
    {
        public InvalidFieldException(string fieldErrorText)
            : base(fieldErrorText, StatusCodes.Status400BadRequest)
        {
        }
    }
}
