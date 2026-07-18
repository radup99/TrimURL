using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class UserPutModel
    {
        [StringLength(30, MinimumLength = 10)]
        public string? Password { get; set; } = null;

        [EmailAddress]
        [StringLength(254)]
        public string? EmailAddress { get; set; } = null;
    }
}
