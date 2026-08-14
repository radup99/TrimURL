using TrimUrlApi.Exceptions;

namespace TrimUrlApi.Validators
{
    public class ShortUrlValidator
    {
        public static void ValidateUrl(string url)
        {
            _ = Uri.TryCreate(url, UriKind.Absolute, out var uriResult);

            if (uriResult == null)
            {
                throw new InvalidUrlStringException(url);
            }

            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                throw new InvalidUrlStringException(url);
            }
        }
    }
}
