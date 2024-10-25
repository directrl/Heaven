global using static Coelum.Phoenix.ModelLoading.GlobalAssimp;

using Silk.NET.Assimp;

namespace Coelum.Phoenix.ModelLoading {
	
	public class GlobalAssimp {
		
		public static Assimp Ai { get; private set; }

		public GlobalAssimp(Assimp ai) {
			Ai = ai;
		}
	}
}