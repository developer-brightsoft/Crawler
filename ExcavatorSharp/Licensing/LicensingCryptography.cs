// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Licensing.LicensingCryptography
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// ------------------------------------------------------------------------------------------------------------------------
/// Cryptography taken from here: https://odan.github.io/2017/08/10/aes-256-encryption-and-decryption-in-php-and-csharp.html
/// ------------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Licensing cryptography environment, version 2.0
/// </summary>

namespace ExcavatorSharp.Licensing
{
	internal class LicensingCryptography
	{
		/// <summary>
		/// Secret keyphrase
		/// </summary>
		private const string KeyPhrase = "data-excavator-keyphrase-08267-bnexq-892";

		/// <summary>
		/// Secret IV
		/// </summary>
		private static byte[] IV = new byte[16]
		{
			5, 9, 0, 1, 6, 4, 8, 2, 17, 7,
			35, 3, 8, 1, 2, 6
		};

		/// <summary>
		/// Encrypts string to secret code
		/// </summary>
		/// <param name="plainText"></param>
		/// <returns></returns>
		public string EncryptString(string plainText)
		{
			SHA256 sHA = SHA256.Create();
			byte[] sourceArray = sHA.ComputeHash(Encoding.ASCII.GetBytes("data-excavator-keyphrase-08267-bnexq-892"));
			byte[] iV = IV;
			Aes aes = Aes.Create();
			aes.Mode = CipherMode.CBC;
			byte[] array = new byte[32];
			Array.Copy(sourceArray, 0, array, 0, 32);
			aes.Key = array;
			aes.IV = iV;
			MemoryStream memoryStream = new MemoryStream();
			ICryptoTransform transform = aes.CreateEncryptor();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			byte[] bytes = Encoding.ASCII.GetBytes(plainText);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			byte[] array2 = memoryStream.ToArray();
			memoryStream.Close();
			cryptoStream.Close();
			return Convert.ToBase64String(array2, 0, array2.Length);
		}

		/// <summary>
		/// Decrypts string from secret code
		/// </summary>
		/// <param name="cipherText"></param>
		/// <returns></returns>
		public string DecryptString(string cipherText)
		{
			SHA256 sHA = SHA256.Create();
			byte[] sourceArray = sHA.ComputeHash(Encoding.ASCII.GetBytes("data-excavator-keyphrase-08267-bnexq-892"));
			byte[] iV = IV;
			Aes aes = Aes.Create();
			aes.Mode = CipherMode.CBC;
			byte[] array = new byte[32];
			Array.Copy(sourceArray, 0, array, 0, 32);
			aes.Key = array;
			aes.IV = iV;
			MemoryStream memoryStream = new MemoryStream();
			ICryptoTransform transform = aes.CreateDecryptor();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			string result = string.Empty;
			try
			{
				byte[] array2 = Convert.FromBase64String(cipherText);
				cryptoStream.Write(array2, 0, array2.Length);
				cryptoStream.FlushFinalBlock();
				byte[] array3 = memoryStream.ToArray();
				result = Encoding.ASCII.GetString(array3, 0, array3.Length);
			}
			finally
			{
				memoryStream.Close();
				cryptoStream.Close();
			}
			return result;
		}
	}
}
