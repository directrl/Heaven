using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.Camera {
	
	public abstract class CameraBase : Node {
		
		public float Width { get; internal set; }
		public float Height { get; internal set; }
		
		public Matrix4x4 ProjectionMatrix { get; protected set; }
		public Matrix4x4 ViewMatrix { get; protected set; }
		
		public Matrix4x4 InverseProjectionMatrix { get; protected set; }
		public Matrix4x4 InverseViewMatrix { get; protected set; }

		private bool _current = false;
		[Obsolete] // TODO Obsolete
		public bool Current {
			get => _current;
			set {
				if(value && Root != null) {
					Root.QueryChildren()
					    .Parallel(true)
					    .Each(node => {
						    if(node is CameraBase camera) {
							    camera._current = false;
						    }
					    })
					    .Execute();
				}

				_current = value;
			}
		}

		protected CameraBase() { }
		
		internal abstract void RecalculateProjectionMatrix();
		internal abstract void RecalculateViewMatrix();
		
		public void Load(CameraMatrices ubo) {
			RecalculateViewMatrix();
			
			ubo.Projection = ProjectionMatrix;
			ubo.View = ViewMatrix;

			ubo.CameraPos = this is Camera3D
				? GetComponent<Transform, Transform3D>().GlobalPosition
				: new(GetComponent<Transform, Transform2D>().GlobalPosition, 0);
		}
	}
}