using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using War3Net.Build.Audio;
using War3Net.Build.Extensions;

namespace BetterTriggers.WorldEdit
{
    public class Sounds
    {
        internal static List<Sound> sounds = new List<Sound>();
        internal static List<Sound> music = new List<Sound>();

        public static List<Sound> GetSoundsAll()
        {
            return sounds;
        }

        public static List<Sound> GetMusicAll()
        {
            return music;
        }

        internal static List<Sound> GetAll()
        {
            List<Sound> list = new List<Sound>();
            list.AddRange(sounds);
            list.AddRange(music);
            return list;
        }

        internal static void Load()
        {
            sounds.Clear();
            music.Clear();

            MapSounds mapSounds = CustomMapData.MPQMap.Sounds;
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
