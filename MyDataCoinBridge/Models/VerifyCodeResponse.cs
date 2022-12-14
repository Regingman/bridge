namespace MyDataCoinBridge.Models
{
	public class Tokens
	{
		public string Access_Token { get; set; }
		public string Refresh_Token { get; set; }
	}

	public class VerifyCodeResponse
	{
		public VerifyCodeResponse(Tokens tokens, int code, string errorMessage)
		{
			Tokens = tokens;
			Code = code;
			ErrorMessage = errorMessage;
		}

		public Tokens Tokens { get; set; }

		public int Code { get; set; }

		public string ErrorMessage { get; set; }
	}
}
