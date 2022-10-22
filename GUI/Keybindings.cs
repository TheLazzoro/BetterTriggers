using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GUI
{
    public class Keybinding
    {
        public Key key;
        public ModifierKeys modifier;
    }

    /// <summary>
    /// Keybindings are dynamically added to the keybinding menu based on the fields in this class.
    /// Just remember to add the get and set actions in the main window as well when adding new keybindings.
    /// </summary>
    public class Keybindings
    {
        public Keybinding NewProject { get; set; } = new Keybinding() { key = Key.N, modifier = ModifierKeys.Control };
        public Keybinding OpenProject { get; set; } = new Keybinding() { key = Key.O, modifier = ModifierKeys.Control };
        public Keybinding SaveProject { get; set; } = new Keybinding() { key = Key.S, modifier = ModifierKeys.Control };
        public Keybinding Undo { get; set; } = new Keybinding() { key = Key.Z, modifier = ModifierKeys.Control };
        public Keybinding Redo { get; set; } = new Keybinding() { key = Key.Y, modifier = ModifierKeys.Control };
        public Keybinding NewCategory { get; set; } = new Keybinding() { key = Key.G, modifier = ModifierKeys.Control };
        public Keybinding NewTrigger { get; set; } = new Keybinding() { key = Key.T, modifier = ModifierKeys.Control };
        public Keybinding NewScript { get; set; } = new Keybinding() { key = Key.U, modifier = ModifierKeys.Control };
        public Keybinding NewGlobalVariable { get; set; } = new Keybinding() { key = Key.L, modifier = ModifierKeys.Control };
        public Keybinding NewEvent { get; set; } = new Keybinding() { key = Key.E, modifier = ModifierKeys.Control };
        public Keybinding NewCondition { get; set; } = new Keybinding() { key = Key.D, modifier = ModifierKeys.Control };
        public Keybinding NewAction { get; set; } = new Keybinding() { key = Key.W, modifier = ModifierKeys.Control };
        public Keybinding TestMap { get; set; } = new Keybinding() { key = Key.F9, modifier = ModifierKeys.Control };
        public Keybinding BuildMap { get; set; } = new Keybinding() { key = Key.B, modifier = ModifierKeys.Control };


        public static string GetModifierText(ModifierKeys modifier)
        {
            string text = string.Empty;
            switch (modifier)
            {
                case ModifierKeys.None:
                    text = null;
                    break;
                case ModifierKeys.Alt:
                    text = "Alt";
                    break;
                case ModifierKeys.Control:
                    text = "Ctrl";
                    break;
                case ModifierKeys.Shift:
                    text = "Shift";
                    break;
                case ModifierKeys.Windows:
                    text = "Windows";
                    break;
                default:
                    break;
            }

            return text;
        }


        private static string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "settings/");
        private static string fileName = "keybindings.json";
        public static void Save(Keybindings keybindings)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string path = Path.Combine(dirPath, fileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(keybindings));
        }

        public static Keybindings Load()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "settings/keybindings.json");
            if (!File.Exists(path))
                return null;

            string file = File.ReadAllText(path);
            Keybindings keybindings = JsonConvert.DeserializeObject<Keybindings>(file);
            return keybindings;
        }

        public bool IsKeybindingAlreadySet(ModifierKeys modifier, Key key)
        {
            bool alreadySet = false;
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                Keybinding keybinding = (Keybinding)property.GetValue(this);
                if(modifier == keybinding.modifier && key == keybinding.key)
                {
                    alreadySet = true;
                    break;
                }
            }
            return alreadySet;
        }

        /// <summary>
        /// Unbinds a command with the given key combinations.
        /// </summary>
        public void UnbindKeybinding(ModifierKeys modifier, Key key)
        {
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                Keybinding keybinding = (Keybinding)property.GetValue(this);
                if (modifier == keybinding.modifier && key == keybinding.key)
                {
                    keybinding.key = Key.None;
                    break;
                }
            }
        }
    }
}
