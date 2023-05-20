using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ARIT_Hackathon.Extensions
{
    public static class FileFromBaseSixtyFourExtension
    {

        private static byte[] cryptkey = Encoding.ASCII.GetBytes("c1f01c0bfb90453a");
        private static byte[] initVector = Encoding.ASCII.GetBytes("c1f01c0bfb90453a");

        public static string GetFileExtension(this String base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
        public static string GetMimeType(this String ext)
        {
            switch (ext)
            {
                case "txt": return "text/plain";
                case "pdf": return "application/pdf";
                case "doc": return "application/vnd.ms-word";
                case "docx": return "application/vnd.ms-word";
                case "xls": return "application/vnd.ms-excel";
                case "png": return "image/png";
                case "jpg": return "image/jpeg";
                case "jpeg": return "image/jpeg";
                case "gif": return "image/gif";
                case "csv": return "text/csv";
                case "zip": return "application/x-zip-compressed";
                default: return "";
            }
        }

        public static string DecryptAES(this string cipherData)
        {
            try
            {
                using (var rijndaelManaged =
                       new RijndaelManaged { Key = cryptkey, IV = initVector, Mode = CipherMode.CBC })
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(cipherData)))
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(cryptkey, initVector),
                           CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }


        public static string CryptAES(this string textToCrypt)
        {
            try
            {
                using (var rijndaelManaged =
                       new RijndaelManaged { Key = cryptkey, IV = initVector, Mode = CipherMode.CBC })
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateEncryptor(cryptkey, initVector),
                           CryptoStreamMode.Write))
                {
                    using (var ws = new StreamWriter(cryptoStream))
                    {
                        ws.Write(textToCrypt);
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
    }
}
