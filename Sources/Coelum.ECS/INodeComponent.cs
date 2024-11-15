using System.Text.Json;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.ECS {

	public interface INodeComponent {
		
		public Node? Owner { get; set; }
	}
}