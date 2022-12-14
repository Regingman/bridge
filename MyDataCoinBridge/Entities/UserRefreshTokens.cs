using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Entities
{
    public class UserRefreshToken
    {
		[Key]
		public int Id { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string RefreshToken { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
