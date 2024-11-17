using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Text.Json.Nodes;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.ECS.Serialization {
	
	public static partial class NodeImporter {

		public static readonly Dictionary<Type, Func<JsonNode, object>> PROPERTY_IMPORTERS = new() {
			{ typeof(Vector2), json => {
				return new Vector2Serializer().Deserialize(json);
			} },
			{ typeof(Vector3), json => {
				return new Vector3Serializer().Deserialize(json);
			} },
			{ typeof(Color), json => {
				return new ColorSerializer().Deserialize(json);
			} },
			{ typeof(decimal), json => {
				return json.GetValue<decimal>();
			} },
			{ typeof(bool), json => {
				return json.GetValue<bool>();
			} },
			{ typeof(string), json => {
				return json.GetValue<string>();
			} },
			{ typeof(IEnumerable), json => {
				throw new NotImplementedException();
			} },
			{ typeof(KeyValuePair<object, object>), json => {
				throw new NotImplementedException();
			} },
		};
	}
}