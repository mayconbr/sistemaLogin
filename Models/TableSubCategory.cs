using Login.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Login.Models
{
    [DataContract]
    public class TableSubCategory
    {
        [Key]
        public long Id { get; set; }
        [DataMember]
        public long? CategoryId { get; set; }
        public virtual TableCategory Category { get; set; }
        [DataMember]
        public string Nome { get; set; }      
    }
}
