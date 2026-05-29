using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace StuScoManager.Models
{
    public class RuntimeConfigs
    {
        public static bool logined = false;
        public RuntimeConfigs() { }
    }
    public class Configs
    {
        static public List<string> configs = [.. File.ReadAllLines("./config/config.txt")];
        static public string pwd;
        static public string inited;
        static public byte[] key;
        static public List<string> keys;
        static public bool AreConfigsCorrect(List<string> configs)
        {
            if (configs.Count != 2)
            {
                return false;
            }
            if (configs[0] == null || configs[1] == null)
            {
                return false;
            }
            return true;
        }
        public static void WriteConfigs()
        {
            configs = [pwd, inited];
            File.WriteAllLines("./config/config.txt", configs);
        }
        public static void ReadConfigsAndKey()
        {
            if (!AreConfigsCorrect(configs))
            {
                WriteAndSetDefaultConfig();
                WriteAndSetDefaultKey();
                return;
            }
            pwd = configs[0]; // 读取加密后的密码
            inited = configs[1]; // 读取是否被初始化了

            // 读取加密密钥
            keys = [.. File.ReadAllLines("./config/key.txt")];
            key = TurnStringsIntoBytes(keys);
        }
        public static void WriteAndSetDefaultKey()
        {
            byte[] defaultKey = AesGcmPasswordProtector.CreateKey();
            File.WriteAllLines("./config/key.txt", TurnBytesIntoStrings(key));
        }
        public static void WriteAndSetDefaultConfig()
        {
            byte[] defaultKey = AesGcmPasswordProtector.CreateKey();
            key = defaultKey;
            string defaultPwd = AesGcmPasswordProtector.Encrypt("000000000000", key);
            configs = [defaultPwd, "false"];
            File.WriteAllLines("./config/config.txt", configs);

        }
        public static List<string> TurnBytesIntoStrings(byte[] bytes)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Add(bytes[i].ToString());
            }
            return result;
        }

        public static byte[] TurnStringsIntoBytes(List<string> strings)
        {
            byte[] result = new byte[strings.Count];
            for (int i = 0; i < strings.Count; i++)
            {
                result[i] = byte.Parse(strings[i]);
            }
            return result;
        }

        public Configs()
        {
            // 检测到配置文件损坏后，强制写入并读取默认配置
            if (!AreConfigsCorrect(configs))
            {
                Debug.WriteLine("Configs are incorrect. Writing and setting default config.");
                WriteAndSetDefaultConfig();
                WriteAndSetDefaultKey();
            } 
            else
            {
                ReadConfigsAndKey();
            }
        }
    }
    /// <summary>
    /// AES-GCM 加密解密器（适用于.NET Core 3.0 / .NET 5+）
    /// </summary>
    public static class AesGcmPasswordProtector
    {
        private const int NonceSize = 12;   // GCM 推荐的 Nonce 长度（字节）
        private const int TagSize = 16;     // 认证标签长度（字节），128 位足够安全
        private const int KeySize = 32;     // 256 位密钥

        public static byte[] CreateKey()
        {
            string keyBase64 = AesGcmPasswordProtector.GenerateKeyBase64();
            byte[] key = AesGcmPasswordProtector.KeyFromBase64(keyBase64);
            return key;
        }
        /// <summary>
        /// 加密明文字符串
        /// </summary>
        /// <param name="plainText">明文（如密码）</param>
        /// <param name="key">32 字节密钥（256 位）</param>
        /// <returns>Base64 格式的密文（包含 Nonce + Tag + 密文）</returns>
        public static string Encrypt(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length != KeySize)
                throw new ArgumentException($"密钥长度必须为 {KeySize} 字节", nameof(key));

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];

            // 生成随机 Nonce（每次加密都不同）
            RandomNumberGenerator.Fill(nonce);

            using var aesGcm = new AesGcm(key);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            // 组合：nonce + tag + 密文
            byte[] result = new byte[NonceSize + TagSize + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 解密密文（Base64 格式）
        /// </summary>
        /// <param name="encryptedData">Encrypt 方法返回的 Base64 字符串</param>
        /// <param name="key">加密时使用的同一密钥</param>
        /// <returns>原始明文字符串</returns>
        public static string Decrypt(string encryptedData, byte[] key)
        {
            if (string.IsNullOrEmpty(encryptedData))
                throw new ArgumentNullException(nameof(encryptedData));
            if (key == null || key.Length != KeySize)
                throw new ArgumentException($"密钥长度必须为 {KeySize} 字节", nameof(key));

            byte[] combined = Convert.FromBase64String(encryptedData);
            if (combined.Length < NonceSize + TagSize)
                throw new ArgumentException("无效的密文数据");

            // 拆分
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipherBytes = new byte[combined.Length - NonceSize - TagSize];

            Buffer.BlockCopy(combined, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(combined, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(combined, NonceSize + TagSize, cipherBytes, 0, cipherBytes.Length);

            byte[] plainBytes = new byte[cipherBytes.Length];

            using var aesGcm = new AesGcm(key);
            aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <summary>
        /// 生成一个安全的 256 位密钥（Base64 格式）
        /// </summary>
        public static string GenerateKeyBase64()
        {
            byte[] key = new byte[KeySize];
            RandomNumberGenerator.Fill(key);
            return Convert.ToBase64String(key);
        }

        /// <summary>
        /// 从 Base64 字符串还原密钥字节数组
        /// </summary>
        public static byte[] KeyFromBase64(string base64Key)
        {
            byte[] key = Convert.FromBase64String(base64Key);
            if (key.Length != KeySize)
                throw new ArgumentException($"密钥长度不正确，应为 {KeySize} 字节");
            return key;
        }
    }
}