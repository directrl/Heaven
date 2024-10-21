global using static Coelum.ModelLoading.GlobalAssimp;

using Silk.NET.Assimp;

namespace Coelum.ModelLoading {
	
	public class GlobalAssimp {
		
		public static Assimp Ai { get; private set; }

		public GlobalAssimp(Assimp ai) {
			Ai = ai;
		}
	}
}