using System.Security.Cryptography;
using System.Text;

namespace Shevastream.Services
{
	public interface ICryptoService
	{
		/// <summary>
		/// Generates MD5 hash of the input string
		/// Result is all uppercase
		/// </summary>
		/// <param name="input">Input for hash function</param>
		/// <returns>MD5 hash of input</returns>
		string CalculateHash(string input);
	}

	public class CryptoService : ICryptoService
	{
		// Credits: https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
		public string CalculateHash(string input)
		{
			// Step 1, calculate MD5 hash from input
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// Step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString();
		}
	}
}
