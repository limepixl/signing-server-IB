using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProxyServer.Models
{
    public class SignatureStatement
    {
        [Key]
        public int Id{ get; set; }
        [NotMapped]
        public IFormFile[] Files{ get; set; }
        public string MessageDigest { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        [DataType(DataType.Date)]
        public DateTime SignedOn { get; set; }
    }
}
