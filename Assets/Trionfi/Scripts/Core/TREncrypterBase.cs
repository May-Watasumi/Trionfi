using System.Text;
using System.IO;
using System.IO.Compression;

namespace Trionfi
{

	public abstract class TRCrypterBase
	{
		abstract public byte[] Encrypt(string data);
		abstract public string Decrypt(byte[] data);
	}

	public class GZCrypter : TRCrypterBase
	{
		override public byte[] Encrypt(string data)
		{
			byte[] compressed;

			using (var outStream = new MemoryStream())
			{
				using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
				using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
					mStream.CopyTo(tinyStream);

				compressed = outStream.ToArray();
				return compressed;
			}
		}

		override public string Decrypt(byte[] data)
		{
			string output;

			using (var inStream = new MemoryStream(data))
			using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
			using (var bigStreamOut = new MemoryStream())
			{
				bigStream.CopyTo(bigStreamOut);
				output = Encoding.UTF8.GetString(bigStreamOut.ToArray());
			}
			return output;
		}
	}
}
