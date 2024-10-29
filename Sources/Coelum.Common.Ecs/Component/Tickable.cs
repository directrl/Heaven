namespace Coelum.Common.Ecs.Component {

	public record Tickable(Action<float> OnUpdate);
}