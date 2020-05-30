namespace MainProject.AppSettingsEncryptor
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var configuration = new ConfigurationBuilder()
                       .SetBasePath(executingAssemblyPath)
                       .AddJsonFile("configuration.json", optional: false, reloadOnChange: false)
                       .Build();

                    var privateKey = configuration.GetSection("PrivateKey").Value;
                    var appsettingsJsonPath = Path.Combine(args[0], "appsettings.json");
                    var appsettingsDevelopmentJsonPath = Path.Combine(args[0], "appsettings.development.json");

                    var data = File.ReadAllText(appsettingsDevelopmentJsonPath, Encoding.UTF8).Trim();
                    var serialized = Utilities.EncryptString(data, privateKey);

                    var appsettingsJsonText = File.ReadAllText(appsettingsJsonPath).Trim();
                    Regex regex = new Regex("\"AppSettings\": \"(.+)\"");
                    Match match = regex.Match(appsettingsJsonText);

                    if (match.Success)
                    {
                        string gvalue = match.Groups[1].Value;
                        appsettingsJsonText = appsettingsJsonText.Replace(gvalue, serialized);
                        File.WriteAllText(appsettingsJsonPath, appsettingsJsonText, Encoding.UTF8);
                        Console.WriteLine("APPSETTINGS.JSON FILE HAS BEEN ENCRYPTED.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    public static class Utilities
    {
        public static string EncryptString(string data, string privateKey)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using Aes aes = Aes.Create();

            aes.Mode = CipherMode.ECB;
            aes.Key = Encoding.UTF8.GetBytes(privateKey);
            aes.IV = new byte[16];

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(data);
                }

                byte[] array = memoryStream.ToArray();
                return Convert.ToBase64String(array);
            }
        }

        public static string DecryptString(string data, string privateKey)
        {
            byte[] buffer = Convert.FromBase64String(data);
            using MemoryStream memoryStream = new MemoryStream(buffer);
            using Aes aes = Aes.Create();

            aes.Mode = CipherMode.ECB;
            aes.Key = Encoding.UTF8.GetBytes(privateKey);
            aes.IV = new byte[16];

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
