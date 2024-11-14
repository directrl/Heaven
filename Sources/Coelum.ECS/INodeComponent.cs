using System.Text.Json;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.ECS {

	public interface INodeComponent : ISerializable<INodeComponent> {
		
		public Node? Owner { get; set; }
	}
}