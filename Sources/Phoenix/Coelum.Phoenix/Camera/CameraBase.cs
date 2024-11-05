using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;

namespace Coelum.Phoenix.Camera {
	
	public abstract class CameraBase : Node {
		
		protected float Width { get; set; }
		protected float Height { get; set; }
		
		public Matrix4x4 ProjectionMatrix { get; protected set; }
		public Matrix4x4 ViewMatrix { get; protected set; }
		
		public Matrix4x4 InverseProjectionMatrix { get; protected set; }
		public Matrix4x4 InverseViewMatrix { get; protected set; }

		private bool _current = false;
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

		protected CameraBase() {
			AddComponent(new ECS.Component.Camera(this));
		}
		
		internal abstract void RecalculateProjectionMatrix();
		internal abstract void RecalculateViewMatrix();
	}
}