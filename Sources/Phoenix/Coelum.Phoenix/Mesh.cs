using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Coelum.LanguageExtensions.Serialization;
using Coelum.Phoenix.OpenGL;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix {
	
	public class Mesh : IDisposable, ISerializable<Mesh> {

		internal readonly List<VertexBufferObject> _vbos = new();

		public VertexArrayObject VAO { get; }

		public PrimitiveType Type { get; }
		public int MaterialIndex { get; set; } = 0;
		
		public Vertex[] Vertices { get; }
		public uint[] Indices { get; }

		public unsafe Mesh(PrimitiveType type, Vertex[] vertices, uint[] indices) {
			Type = type;
			Vertices = vertices;
			Indices = indices;

			VAO = new();
			VAO.Bind();

			var vbo = new VertexBufferObject(BufferTargetARB.ArrayBuffer);
			_vbos.Add(vbo);
			
			var ebo = new VertexBufferObject(BufferTargetARB.ElementArrayBuffer);
			_vbos.Add(ebo);

			vbo.Bind();
			vbo.Data<Vertex>(vertices, BufferUsageARB.StaticDraw);
			
			ebo.Bind();
			ebo.Data<uint>(indices, BufferUsageARB.StaticDraw);

		#region vertex value positioning
			var t = VertexAttribPointerType.Float;
			uint s = (uint) sizeof(Vertex);
			
			// position
			Gl.EnableVertexAttribArray(0);
			Gl.VertexAttribPointer(0, 3, t, false, s, Vertex.POSITION_OFFSET);
			//Gl.VertexAttribDivisor(0, 1);
			
			// normal
			Gl.EnableVertexAttribArray(1);
			Gl.VertexAttribPointer(1, 3, t, false, s, Vertex.NORMAL_OFFSET);
			//Gl.VertexAttribDivisor(1, 1);
			
			// texcoords
			Gl.EnableVertexAttribArray(2);
			Gl.VertexAttribPointer(2, 2, t, false, s, Vertex.TEXCOORDS_OFFSET);
			//Gl.VertexAttribDivisor(2, 1);
		#endregion
			
			Gl.BindVertexArray(0);
		}
		
		public Mesh(Mesh other): this(other.Type,
		                              other.Vertices,
		                              other.Indices) {
			
			MaterialIndex = other.MaterialIndex;
		}

		public unsafe virtual void Render() {
			VAO.Bind();
			Gl.DrawElements(Type, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			
			foreach(var vbo in _vbos) {
				vbo.Dispose();
			}
			
			VAO.Dispose();
		}

		public void Serialize(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(VAO.Id.ToString());
			{
				writer.WriteNumber("type", (int) Type);
				writer.WriteNumber("material_index", MaterialIndex);
				
				writer.WriteStartArray("vertices");
				foreach(var vertex in Vertices) {
					var json = JsonSerializer.SerializeToNode(vertex);
					writer.WriteRawValue(json.ToJsonString());
				}
				writer.WriteEndArray();
				
				writer.WriteStartArray("indicies");
				foreach(var index in Indices) {
					writer.WriteNumberValue(index);
				}
				writer.WriteEndArray();
			}
			writer.WriteEndObject();
		}
		
		public Mesh Deserialize(JsonNode node) {
			var type = (PrimitiveType) node["type"].GetValue<int>();
			var materialIndex = node["material_index"].GetValue<int>();

			var vertices = new List<Vertex>();
			var verticesJson = node["vertices"].AsArray();

			var indices = new List<uint>();
			var indicesJson = node["indices"].AsArray();

			foreach(var vertexJson in verticesJson) {
				vertices.Add(vertexJson.Deserialize<Vertex>());
			}

			foreach(var indexJson in indicesJson) {
				indices.Add(indexJson.GetValue<uint>());
			}

			return new(type, vertices.ToArray(), indices.ToArray()) {
				MaterialIndex = materialIndex
			};
		}
	}
}