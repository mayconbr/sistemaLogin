using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Login.Models
{

	[Serializable]
	public class AuxSenha
	{
		[DataMember]
		public string Email { get; set; }
		[DataMember]
		public string Key { get; set; }
	}
}
