namespace Coelum.Raven.Shader.Cell {
	
	public class BlendingShader : ICellShader {
		
		public RenderContext Context { get; }
		public BlendingType Type { get; }
		
		public BlendingShader(RenderContext ctx, BlendingType type) {
			Context = ctx;
			Type = type;
		}

		public bool Process(ref ICellShader.Parameter input) {
			var cell = Context[input.Position];

			switch(Type) {
				case BlendingType.Soft:
					throw new NotImplementedException();
					break;
				case BlendingType.Hard:
					if(cell.HasValue && input.Cell.BackgroundColor.A == 0) {
						input.Cell.BackgroundColor = cell.Value.BackgroundColor;
					}
					break;
			}

			return true;
		}

		public enum BlendingType {
			
			Soft,
			Hard
		}
	}
}