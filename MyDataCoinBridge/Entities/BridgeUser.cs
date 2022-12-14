using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Entities
{
    public enum Roles
    {
        Administrator,
        Manager,
        User
    }

    public class BridgeUser
    {
        [Key]
        public Guid Id { get; set; }

        public string Email { get; set; }

        public Roles Role { get; set; }

        public string VerificationCode { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
