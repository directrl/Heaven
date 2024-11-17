using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Text.Json;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.ECS.Serialization {
	
	public static partial class NodeExporter {
		
		public static readonly Dictionary<Type, Action<string, object, Utf8JsonWriter>> PROPERTY_EXPORTERS = new() {
			{ typeof(Vector2), (name, value, writer) => {
				((Vector2) value).Serializer().Serialize(name, writer);
			} },
			{ typeof(Vector3), (name, value, writer) => {
				((Vector3) value).Serializer().Serialize(name, writer);
			} },
			{ typeof(Color), (name, value, writer) => {
				((Color) value).Serializer().Serialize(name, writer);
			} },
			{ typeof(decimal), (name, value, writer) => {
				writer.WriteNumber(name, Convert.ToDecimal(value));
			} },
			{ typeof(bool), (name, value, writer) => {
				writer.WriteBoolean(name, (bool) value);
			} },
			{ typeof(string), (name, value, writer) => {
				writer.WriteString(name, (string) value);
			} },
			{ typeof(IEnumerable), (name, value, writer) => {
				var collection = (IEnumerable) value;
				var enumerator = collection.GetEnumerator();

				writer.WriteStartObject(name);
				{
					int i = 0;

					while(enumerator.MoveNext()) {
						var o = enumerator.Current;

						PROPERTY_EXPORTERS[o.GetType()]
							.Invoke(i.ToString(), o, writer);
						
						i++;
					}
				}
				writer.WriteEndObject();
				
				((IDisposable) enumerator).Dispose();
			} },
			{ typeof(KeyValuePair<object, object>), (name, value, writer) => {
				var kv = (KeyValuePair<object, object>) value;
				
				writer.WriteStartObject(name);
				{
					PROPERTY_EXPORTERS[kv.Key.GetType()]
						.Invoke(kv.Key.ToString(), kv.Value, writer);
				}
				writer.WriteEndObject();
			} },
		};
	}
}