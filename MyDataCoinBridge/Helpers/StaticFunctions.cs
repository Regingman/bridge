using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace MyDataCoinBridge.Helpers
{
    public static class StaticFunctions
    {
        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateCode()
        {
            Random random = new Random();
            const string chars = "0123456789";

            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public static void SendCode(string email, string code, string pass)
        {
            var smtpClient = new SmtpClient("smtpout.secureserver.net")
            {
                Port = 587,
                Credentials = new NetworkCredential("help@mydatacoin.io", pass),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("help@mydatacoin.io"),
                Subject = "Verification Code",
                Body = $"<h1>Your verification code is: {code}</h1>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }
    }
}
