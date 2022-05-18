using System.ComponentModel.DataAnnotations;

namespace ProxyServer.Models
{
    public class ServerKeyPair
    {
        [Key]
        public int Id { get; set; }
        public String  PrivatePublicKeyPair { get; set; }
        public String PublicKeyOnly { get; set; }
    }
}
