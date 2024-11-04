using System.Numerics;
using Coelum.Debug;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.Texture;
using Coelum.Resources;
using Silk.NET.Core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Shader = Coelum.Phoenix.OpenGL.Shader;

namespace Coelum.Phoenix {
	
	public class Framebuffer : IDisposable {

	#region Rendering stuff
		private static Mesh _QUAD_MESH;
		private static ShaderProgram _FBO_SHADER;

		static Framebuffer() {
			_QUAD_MESH = new(
				PrimitiveType.Triangles,
				new Vertex[] {
					new(new(-1.0f, 1.0f, 0.0f), Vector3.Zero, new(0.0f, 1.0f)),
					new(new(-1.0f, -1.0f, 0.0f), Vector3.Zero, new(0.0f, 0.0f)),
					new(new(1.0f, -1.0f, 0.0f), Vector3.Zero, new(1.0f, 0.0f)),

					new(new(-1.0f, 1.0f, 0.0f), Vector3.Zero, new(0.0f, 1.0f)),
					new(new(1.0f, -1.0f, 0.0f), Vector3.Zero, new(1.0f, 0.0f)),
					new(new(1.0f, 1.0f, 0.0f), Vector3.Zero, new(1.0f, 1.0f)),
				},
				new uint[] { 0, 1, 2, 3, 4, 5 }
			);

			_FBO_SHADER = new(
				Module.RESOURCES,
				new(ShaderType.VertexShader, Module.RESOURCES[ResourceType.SHADER, "framebuffer.vert"]),
				new(ShaderType.FragmentShader, Module.RESOURCES[ResourceType.SHADER, "framebuffer.frag"])
			);
			_FBO_SHADER.Validate();
			_FBO_SHADER.Build();
		}
	#endregion
		
		public FramebufferTarget Target { get; }
		public uint Handle { get; }
		
		public uint Width { get; }
		public uint Height { get; }
		
		public Texture2D Texture { get; }

		public bool AutoResize { get; set; } = false;
		public float AutoResizeFactor { get; set; } = 1.0f;
		
		public unsafe Framebuffer(uint width, uint height, 
		                          FramebufferTarget target = FramebufferTarget.Framebuffer) {

			Target = target;
			Handle = Gl.GenFramebuffer();
			
			Gl.BindFramebuffer(Target, Handle);

			Width = width;
			Height = height;

			// create texture
			Texture = new((int) width, (int) height);
			Texture.Bind();
			
			Gl.TexImage2D(
				Texture.Target,
				0,
				InternalFormat.Rgb,
				Width, Height,
				0,
				PixelFormat.Rgb,
				PixelType.UnsignedByte,
				null
			);
			
			Gl.TexParameter(Texture.Target, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
			Gl.TexParameter(Texture.Target, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
			Gl.BindTexture(Texture.Target, 0);
			
			// attach texture to framebuffer
			Gl.FramebufferTexture2D(Target, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D,
			                        Texture.Id, 0);

			// create depth and stencil buffer
			uint rbo = Gl.GenRenderbuffer();
			Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
			Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Width, Height);
			Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
			
			// and attach it to the framebuffer
			Gl.FramebufferRenderbuffer(Target, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);
			
			// check if creation was successful
			if(Gl.CheckFramebufferStatus(target) != GLEnum.FramebufferComplete) {
				throw new PlatformException("Framebuffer is not complete");
			}
			
			// unbind when complete
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		// this constructor is only used for creating the default (0) framebuffer
		internal Framebuffer(IWindow window) {
			Target = FramebufferTarget.Framebuffer;
			Handle = 0;
			
			Width = (uint) window.FramebufferSize.X;
			Height = (uint) window.FramebufferSize.Y;
		}

		public void Bind() {
			Gl.BindFramebuffer(Target, Handle);
			Gl.Viewport(new Vector2D<int>((int) Width, (int) Height));
		}

		public void Render() {
			if(Handle == 0) return;
			
			_FBO_SHADER.Bind();
			Texture.Bind();
			
			_QUAD_MESH.Render();
		}

		public void Dispose() {
			if(Handle == 0) return;
			
			Gl.DeleteFramebuffer(Handle);
			Texture.Dispose();
		}
	}
}