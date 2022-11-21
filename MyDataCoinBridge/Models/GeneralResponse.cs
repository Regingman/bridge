using System;
namespace MyDataCoinBridge.Models
{
    public class GeneralResponse
    {
        public GeneralResponse(int code, object message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
		/// The HTTP Status Code
		/// </summary>
		/// <example>200</example>
        public int Code { get; set; }

        /// <summary>
		/// The Message from response
		/// </summary>
		/// <example>Success</example>
        public object Message { get; set; }
        
    }
}
