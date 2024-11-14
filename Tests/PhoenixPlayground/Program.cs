using Coelum.Phoenix.Editor;
using PhoenixPlayground;
using PhoenixPlayground.Scenes;

var heaven = new Playground();

if(args.Contains("--editor")) {
	var editor = new EditorApplication(heaven.GetType().Assembly, new NodeGraphTest());
	editor.Start(args);
} else {
	heaven.Start(args);
}