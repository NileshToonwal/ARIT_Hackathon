using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ARIT_Hackathon.Extensions
{
    public class TokenRequest
    {
        [Required]
        [JsonProperty("loginid")]
        public string LoginId { get; set; }
        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }
        // [Required]
        // [JsonProperty("rolename")]
        // public string RoleName { get; set; }
    }

    public class UserRequest
    {
        public string LoginId { get; set; }
        public string Password { get; set; }
    }
}
