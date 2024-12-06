using Login.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Login.Models
{
    [DataContract]
    public class TableSystem
    {
        [Key]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Domain { get; set; }
        public DateTime? DataDelete { get; set; }
    }
}
