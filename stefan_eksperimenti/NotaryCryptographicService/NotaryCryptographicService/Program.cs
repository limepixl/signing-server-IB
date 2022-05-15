using System.Security.Cryptography;
using System.Text;
using System.Collections;

namespace NotaryCryptographicService
{
    class NotaryServer
    {
        RSACryptoServiceProvider RSA;

        public NotaryServer()
        {
            RSA = new RSACryptoServiceProvider();
        }

        public static byte[] ConvertTripleToBytes(String message, String proxy_ID, long timestamp)
        {
            byte[] timestamp_bytes = BitConverter.GetBytes(timestamp);

            UnicodeEncoding byte_converter = new UnicodeEncoding();
            byte[] message_bytes = byte_converter.GetBytes(message);
            byte[] ID_bytes = byte_converter.GetBytes(proxy_ID);

            byte[] bytes_to_sign = new byte[timestamp_bytes.Length + message_bytes.Length + ID_bytes.Length];
            Buffer.BlockCopy(timestamp_bytes, 0, bytes_to_sign, 0, timestamp_bytes.Length);
            Buffer.BlockCopy(message_bytes, 0, bytes_to_sign, timestamp_bytes.Length, message_bytes.Length);
            Buffer.BlockCopy(ID_bytes, 0, bytes_to_sign, timestamp_bytes.Length + message_bytes.Length, ID_bytes.Length);

            return bytes_to_sign;
        }

        public byte[] CreateSignature(String message, String proxy_ID, long timestamp, HashAlgorithm hash)
        {
            byte[] bytes_to_sign = ConvertTripleToBytes(message, proxy_ID, timestamp);
            return Sign(bytes_to_sign, hash);
        }

        public bool VerifySignature(String message, String proxy_ID, long timestamp, byte[] signature, HashAlgorithm hash)
        {
            byte[] bytes_to_sign = ConvertTripleToBytes(message, proxy_ID, timestamp);
            return Verify(bytes_to_sign, signature, hash);
        }

        public byte[] Sign(byte[] message_bytes, HashAlgorithm hash)
        {
            byte[] signature_bytes = RSA.SignData(message_bytes, hash);
            return signature_bytes;
        }

        public bool Verify(byte[] message_bytes, byte[] signed_message_bytes, HashAlgorithm hash)
        {
            return RSA.VerifyData(message_bytes, hash, signed_message_bytes);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // The count of notary servers that need to verify
            // a signature in order to be 
            const uint THRESHOLD_COUNT = 2;

            String message_to_be_signed = "Test Message";
            String proxy_ID = "proxy112312";
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            byte[] bytes_to_sign = NotaryServer.ConvertTripleToBytes(message_to_be_signed, proxy_ID, timestamp);

            NotaryServer[] servers = { new NotaryServer(), new NotaryServer(), new NotaryServer() };

            ArrayList signatures = new ArrayList(3);
            for(int i = 0; i < 3; i++)
            {
                byte[] signature = servers[i].Sign(bytes_to_sign, SHA256.Create());
                signatures.Add(signature);
            }

            // Above this line is the Proxy server's job
            // Below this line is the Notary server's job

            // Verifying signatures (proxy needs to know order of notaries)
            // TODO: actually get the bytes and split them in n different parts.
            uint count = 0;
            for(int i = 0; i < 3; i++)
            {
                object? tmp = signatures[i];
                if (tmp is null)
                {
                    Console.WriteLine($"No signature from notary server #{i} saved!");
                    continue;
                }

                byte[] signature = (byte[])tmp;
                NotaryServer server = servers[i];

                if (server.Verify(bytes_to_sign, signature, SHA256.Create()))
                {
                    Console.Out.WriteLine($"Notary server #{i} has signed this message!");
                    count++;
                }
            }

            if(count > THRESHOLD_COUNT)
            {
                Console.Out.WriteLine($"More than {THRESHOLD_COUNT} notary servers have verified the signature!");
            }
            else
            {
                Console.Out.WriteLine($"Not enough notary servers have verified the signature. Denied.");
            }

            Console.Out.WriteLine("////////////////////////////\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}