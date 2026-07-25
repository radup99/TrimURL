using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class UserPutModel
    {
        public string? Password { get; set; } = null;

        [EmailAddress]
        [StringLength(254)]
        public string? EmailAddress { get; set; } = null;
    }
}
