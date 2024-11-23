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

	#region Delegates
		public delegate void ResizeEventHandler(Vector2D<int> newSize);
	#endregion

	#region Events
		public event ResizeEventHandler? Resize;
	#endregion
		
		private SilkWindow? _window;
		
		public FramebufferTarget Target { get; }
		public uint Handle { get; private set; }

		private Vector2D<int> _size;
		public Vector2D<int> Size {
			get => _size;
			set {
				Resize?.Invoke(value);
				
				var nfb = new Framebuffer(value, _window, Target);

				Dispose();
			
				Handle = nfb.Handle;
				_size = value;
				Texture = nfb.Texture;
			}
		}
		
		public Texture2D Texture { get; private set; }

		private bool _autoResize = false;
		public bool AutoResize {
			get => _autoResize;
			set {
				if(value && _window != null) {
					_autoResize = true;
					_window.SilkImpl.FramebufferResize += _WindowFramebufferResizeHandler;
					return;
				} else if(_window != null) {
					_window.SilkImpl.FramebufferResize -= _WindowFramebufferResizeHandler;
				}

				_autoResize = false;
			}
		}

		public float AutoResizeFactor { get; set; } = 1.0f;
		
		// TODO transparency in framebuffer (Rgba)
		public unsafe Framebuffer(Vector2D<int> size,
		                          SilkWindow? window = null,
		                          FramebufferTarget target = FramebufferTarget.Framebuffer) {

			_window = window;
			
			Target = target;
			Handle = Gl.GenFramebuffer();
			
			Gl.BindFramebuffer(Target, Handle);

			_size = size;

			// create texture
			Texture = new(Size.X, Size.Y);
			Texture.Bind();
			
			Gl.TexImage2D(
				Texture.Target,
				0,
				InternalFormat.Rgb,
				(uint) Size.X, (uint) Size.Y,
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
			                        Texture.Handle, 0);

			// create depth and stencil buffer
			uint rbo = Gl.GenRenderbuffer();
			Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
			Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, (uint) Size.X, (uint) Size.Y);
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
		internal Framebuffer(SilkWindow window) {
			Target = FramebufferTarget.Framebuffer;
			Handle = 0;

			_size = window.SilkImpl.FramebufferSize;
			_autoResize = true;
			
			window.SilkImpl.FramebufferResize += newSize => {
				var size = new Vector2D<int>((int) (newSize.X * AutoResizeFactor),
				                             (int) (newSize.Y * AutoResizeFactor));
				
				Resize?.Invoke(size);
				_size = size;
			};
		}

		public void Bind() {
			Gl.BindFramebuffer(Target, Handle);
			Gl.Viewport(Size);
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

			Handle = uint.MaxValue;

			if(_window != null) {
				_window.SilkImpl.FramebufferResize -= _WindowFramebufferResizeHandler;
			}
		}

	#region Event handlers
		private void _WindowFramebufferResizeHandler(Vector2D<int> newSize) {
			var size = new Vector2D<int>((int) (newSize.X * AutoResizeFactor),
			                             (int) (newSize.Y * AutoResizeFactor));

			Size = size;
		}
	#endregion
	}
}