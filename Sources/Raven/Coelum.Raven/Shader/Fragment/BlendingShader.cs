namespace Coelum.Raven.Shader.Fragment {
	
	public class BlendingShader : IFragmentShader {
		
		public RenderContext Context { get; }
		public BlendingType Type { get; }
		
		public BlendingShader(RenderContext ctx, BlendingType type) {
			Context = ctx;
			Type = type;
		}

		public void Process(ref IFragmentShader.Parameter input) {
			var cell = Context[input.Position];

			switch(Type) {
				case BlendingType.Soft:
					throw new NotImplementedException(); // TODO
					break;
				case BlendingType.Hard:
					if(cell.HasValue && cell.Value.Character != ' ' && input.Cell.BackgroundColor.A == 0) {
						input.Cell.BackgroundColor = cell.Value.BackgroundColor;
					}
					break;
			}
		}

		public enum BlendingType {
			
			Soft,
			Hard
		}
	}
}