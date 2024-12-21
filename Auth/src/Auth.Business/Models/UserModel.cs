using System.Text.Json.Serialization;

namespace Auth.Business.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        public string Roles { get; set; }

        public string Password { get; set; }
    }
}
