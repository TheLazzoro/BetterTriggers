using BetterTriggers.Models.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElementRoot : IExplorerElement
    {
        public War3Project project;
        public List<IExplorerElement> explorerElements = new List<IExplorerElement>();
        public List<IExplorerElementUI> observers = new List<IExplorerElementUI>();
        private string path;
        private DateTime LastWrite;
        private long Size;

        public ExplorerElementRoot(War3Project project, string path)
        {
            this.path = path;
            this.project = project;
            UpdateMetadata();
        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetPath()
        {
            return project.Root;
        }

        public string GetProjectPath()
        {
            return path;
        }

        public void SetPath(string newPath)
        {
            this.path = newPath;
        }

        public void InsertIntoList(IExplorerElement element, int insertIndex)
        {
            explorerElements.Insert(insertIndex, element);
        }

        public void RemoveFromList(IExplorerElement element)
        {
            explorerElements.Remove(element);
        }

        public void Attach(IExplorerElementUI observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IExplorerElementUI observer)
        {
            this.observers.Remove(observer);
        }

        public void Notify()
        {
            //foreach (var observer in observers)
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].Update(this);
            }
        }

        public void DeleteObservers()
        {
            foreach (var observer in observers)
            {
                observer.Delete();
            }
        }

        public int GetId()
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }

        public bool GetEnabled()
        {
            return true;
        }

        public bool GetInitiallyOn()
        {
            return true;
        }

        public long GetSize()
        {
            return Size;
        }

        public DateTime GetLastWrite()
        {
            return LastWrite;
        }

        public void UpdateMetadata()
        {
            //throw new NotImplementedException();
        }

        public void RemoveFromParent()
        {
            throw new NotImplementedException();
        }

        public void Created(int insertIndex)
        {
            throw new NotImplementedException();
        }

        public void Deleted()
        {
            throw new NotImplementedException();
        }

        IExplorerElement IExplorerElement.GetParent()
        {
            throw new NotImplementedException();
        }

        public void SetParent(IExplorerElement parent, int insertIndex)
        {
            throw new NotImplementedException();
        }

        public List<IExplorerElement> GetExplorerElements()
        {
            return explorerElements;
        }

        public void ChangedPosition()
        {
            throw new NotImplementedException();
        }

        // Root cannot be copied.
        public IExplorerElement Clone()
        {
            return null;
        }
    }
}