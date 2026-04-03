using BetterTriggers.Containers;
using System.Collections.Generic;
using War3Net.Build.Environment;

namespace BetterTriggers.WorldEdit
{
    public class Cameras
    {
        private List<Camera> cameras = new List<Camera>();

        public List<Camera> GetAll()
        {
            return cameras;
        }

        internal void Load(Project project)
        {
            cameras.Clear();

            MapCameras mapCameras;
            mapCameras = project.MPQMap.Cameras;
            if (mapCameras == null)
                return;

            mapCameras.Cameras.ForEach(c => cameras.Add(c));
        }
    }
}
