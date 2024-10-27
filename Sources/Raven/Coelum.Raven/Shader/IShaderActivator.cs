namespace Coelum.Raven.Shader {
	
	public interface IShaderActivator {
		
		public List<ICellShader> CellShaders { get; }
		public List<ICellShader> SpatialShaders { get; }

		public void ActivateShaders(RenderContext ctx) {
			ctx.CellShaders.AddRange(CellShaders);
			ctx.CellShaders.AddRange(SpatialShaders);
		}

		public void DeactivateShaders(RenderContext ctx) {
			foreach(var cell in CellShaders) {
				ctx.CellShaders.Remove(cell);
			}
			
			foreach(var spatial in SpatialShaders) {
				ctx.CellShaders.Remove(spatial);
			}
		}
	}
}