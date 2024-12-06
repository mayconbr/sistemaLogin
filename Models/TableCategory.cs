using Login.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Login.Models
{
    [DataContract]
    public class TableCategory
    {
        [Key]
        public long Id { get; set; }
        [DataMember]
        public string Nome { get; set; }
    }
}
