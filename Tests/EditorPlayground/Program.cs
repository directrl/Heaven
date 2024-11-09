using Coelum.Phoenix.Editor;
using Coelum.Phoenix.Editor.Scenes;
using EditorPlayground;

var playground = new Playground();
var editor = new EditorApplication(playground.GetType().Assembly, new EmptyScene());
editor.Start(args);
