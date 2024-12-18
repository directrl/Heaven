﻿using Coelum.Configuration;
using Silk.NET.Input;

namespace Coelum.Common.Input {

	public class KeyBindings : IDisposable {

		public GameOptions Options { get; }
		public Dictionary<string, KeyBinding> Bindings { get; } = new();

		public bool Enabled { get; set; } = true;

		private readonly List<int> _combo = new();

		public KeyBindings(string id) {
			Options = new(Path.Join(
				Directories.ConfigurationRoot, $"keys.{id}.json"));
		}

		public void Register(ref KeyBinding binding) {
			if(Options.Has(binding.Name)) {
				var temp = Options.GetObject<KeyBinding>(binding.Name);
				binding.Keys = temp.Keys;
			} else {
				Options.Set(binding.Name, binding);
			}

			Bindings[binding.Name] = binding;
		}

		public KeyBinding Register(KeyBinding tempBinding) {
			var binding = tempBinding;

			if(Options.Has(binding.Name)) {
				binding = Options.GetObject<KeyBinding>(binding.Name);
			} else {
				Options.SetObject(binding.Name, binding);
			}

			Bindings[binding.Name] = binding;
			return binding;
		}

		public void Rebind(KeyBinding binding) {
			Bindings[binding.Name] = binding;
			Options.SetObject(binding.Name, binding);
		}

		public void Input(KeyAction action, int key) {
			if(!Enabled) return;
			if(Bindings.Count <= 0) return;

			switch(action) {
				case KeyAction.Press:
					_combo.Add(key);
					break;
				case KeyAction.Release:
					_combo.Remove(key);
					break;
			}

			if(action == KeyAction.Press) {
				foreach(var binding in Bindings.Values) {
					if(binding.Keys.Length == 1) {
						binding.Pressed = (key == binding.Keys[0]);
					} else {
						binding.Pressed = binding.Keys.SequenceEqual(_combo);
					}
				}
			}
		}

		public void Update(IKeyboard keyboard) {
			foreach(var binding in Bindings.Values) {
				binding.Pressed = false;

				if(!Enabled) {
					binding.Down = false;
					return;
				}

				if(binding.Keys.Length == 1) {
					binding.Down = keyboard.IsKeyPressed(binding.Keys[0]);
				}
			}
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			Options.Save();
		}
	}
}