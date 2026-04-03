using BetterTriggers.Containers;
using System.Collections.Generic;
using War3Net.Build.Audio;

namespace BetterTriggers.WorldEdit
{
    public class Sounds
    {
        internal List<Sound> sounds = new List<Sound>();
        internal List<Sound> music = new List<Sound>();

        public List<Sound> GetSoundsAll()
        {
            return sounds;
        }

        public List<Sound> GetMusicAll()
        {
            return music;
        }

        internal List<Sound> GetAll()
        {
            List<Sound> list = new List<Sound>();
            list.AddRange(sounds);
            list.AddRange(music);
            return list;
        }

        internal void Load(Project project)
        {
            sounds.Clear();
            music.Clear();
            MapSounds mapSounds = project.MPQMap.Sounds;
            if (mapSounds == null)
                return;

            for (int i = 0; i < mapSounds.Sounds.Count; i++)
            {
                if (mapSounds.Sounds[i].Flags.HasFlag(SoundFlags.Music))
                    music.Add(mapSounds.Sounds[i]);
                else
                    sounds.Add(mapSounds.Sounds[i]);
            }
        }

    }
}
