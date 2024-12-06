using Login.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Login.Models
{
    [DataContract]
    public class TableUser
    {
        [Key]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string? Key { get; set; }
        [DataMember]
        public string Hash { get; set; }       
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public DateTime? DateDelete { get; set; }

        [DataMember]
        public long? SystemId { get; set; }
        public virtual TableSystem System { get; set; }

        [DataMember]
        public long? TypeId { get; set; }
        public virtual TableType Type { get; set; }

        [DataMember]
        public long? SubCategoryId { get; set; }
        public virtual TableSubCategory SubCategory { get; set; }

        [DataMember]
        public string? AddInfo { get; set; }

    }
}
