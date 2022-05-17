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

        internal static List<Sound> GetSoundsAll()
        {
            return sounds;
        }

        internal static List<Sound> GetMusicAll()
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

            string filePath = "war3map.w3s";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var mapSounds = BinaryReaderExtensions.ReadMapSounds(reader);

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
}
