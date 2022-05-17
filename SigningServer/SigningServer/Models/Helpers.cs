using ProxyServer.Data;
using ProxyServer.Models;
using System.Security.Cryptography;
public class Helpers {

    private readonly ApplicationDbContext _context = new ApplicationDbContext(null);
    private Boolean keys_generated = false;

    public static RSACryptoServiceProvider GenerateRSAKeyPair()
    {
        CspParameters cp = new CspParameters();
        cp.KeyContainerName = "ServerRSAKeyPairContainer";
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);
        return rsa;

    }

    public Boolean KeyGeneration()
    {
        if (!keys_generated)
        {
            RSACryptoServiceProvider rsa = Helpers.GenerateRSAKeyPair();
            ServerKeyPair keys = new ServerKeyPair();

            keys.PrivatePublicKeyPair = rsa.ToXmlString(true);
            keys.PublicKeyOnly = rsa.ToXmlString(false);
            _context.ServerKeyPair.Add(keys);
            _context.SaveChanges();
            keys_generated = true;
        }
        return true;
    }

}