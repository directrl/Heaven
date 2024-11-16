using System.Numerics;
using BepuPhysics;
using BepuUtilities;

namespace Coelum.Phoenix.Physics.Callbacks {
	
	public struct DefaultPoseIntegrator : IPoseIntegratorCallbacks {

		private Vector3Wide _gravityDt;
		private Vector<float> _linearDampingDt;
		private Vector<float> _angularDampingDt;

		public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;
		public bool AllowSubstepsForUnconstrainedBodies => false;
		public bool IntegrateVelocityForKinematics => false;
		
		public Vector3 Gravity { get; set; }
		public float LinearDamping { get; set; }
		public float AngularDamping { get; set; }

		public DefaultPoseIntegrator() {
			_gravityDt = new();
			_linearDampingDt = new();
			_angularDampingDt = new();
			
			Gravity = new(0, -9.81f, 0);
			LinearDamping = 1.0f;
			AngularDamping = 2.0f;
		}

		public void Initialize(Simulation simulation) { }
		
		public void PrepareForIntegration(float dt) {
			_gravityDt = Vector3Wide.Broadcast(Gravity * dt);
			_linearDampingDt = new(MathF.Pow(MathHelper.Clamp(1 - LinearDamping, 0, 1), dt));
			_angularDampingDt = new(MathF.Pow(MathHelper.Clamp(1 - AngularDamping, 0, 1), dt));
		}
		
		public void IntegrateVelocity(Vector<int> bodyIndices,
		                              Vector3Wide position, QuaternionWide orientation,
		                              BodyInertiaWide localInertia, Vector<int> integrationMask,
		                              int workerIndex, Vector<float> dt,
		                              ref BodyVelocityWide velocity) {

			//velocity.Linear = (velocity.Linear + _gravityDt) * _linearDampingDt;
			//velocity.Angular = velocity.Angular * _angularDampingDt;
			velocity.Linear += _gravityDt;
		}
	}
}