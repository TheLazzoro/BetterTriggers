using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public Keybinding NewProject { get; set; }
        public Keybinding OpenProject { get; set; }
        public Keybinding SaveProject { get; set; }
        public Keybinding Undo { get; set; }
        public Keybinding Redo { get; set; }
        public Keybinding NewCategory { get; set; }
        public Keybinding NewTrigger { get; set; }
        public Keybinding NewScript { get; set; }
        public Keybinding NewGlobalVariable { get; set; }
        public Keybinding NewEvent { get; set; }
        public Keybinding NewCondition { get; set; }
        public Keybinding NewAction { get; set; }

        
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
    }
}
