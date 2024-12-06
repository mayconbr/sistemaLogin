using Login.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Login.Models
{
    [DataContract]
    public class TableType
    {
        [Key]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public long? SystemId { get; set; }
        public virtual TableSystem System { get; set; }     
        public DateTime? DataDelete {  get; set; }
    }
}
