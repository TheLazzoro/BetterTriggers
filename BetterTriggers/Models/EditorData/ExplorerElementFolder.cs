using BetterTriggers.Containers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElementFolder : IExplorerElement
    {
        public string path;
        public List<IExplorerElement> explorerElements = new List<IExplorerElement>();
        public List<IExplorerElementUI> observers = new List<IExplorerElementUI>();
        public bool isExpanded;
        private DateTime LastWrite;
        private long Size;

        private IExplorerElement Parent;

        /// <summary>Reserved for copy-pasting purposes.</summary>
        public ExplorerElementFolder()
        {
        }

        public ExplorerElementFolder(string path)
        {
            this.path = path;
            UpdateMetadata();
            Folders.AddFolder(this);
        }

        public string GetName()
        {
            return Path.GetFileName(path);
        }

        public string GetPath()
        {
            return path;
        }

        public string GetSaveablePath()
        {
            string path = this.path;
            while (true)
            {
                path = Path.GetDirectoryName(path);
                if (Path.GetFileName(path) == "src")
                {
                    path = path + "\\";
                    path = this.path.Replace(path, "");
                    break;
                }
            }
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
                observers[i].Reload();
            }
        }

        public void DeleteObservers()
        {
            foreach (var observer in observers)
            {
                observer.Delete();
            }
        }

        public long GetId()
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool isEnabled)
        {
            //throw new NotImplementedException();
        }

        public void SetInitiallyOn(bool isInitiallyOn)
        {
            //throw new NotImplementedException();
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
            var info = new DirectoryInfo(path);
            this.Size = info.EnumerateFiles().Sum(file => file.Length);
            this.LastWrite = info.LastWriteTime;
        }

        public List<IExplorerElement> GetExplorerElements()
        {
            return explorerElements;
        }

        public IExplorerElement GetParent()
        {
            return Parent;
        }

        public void SetParent(IExplorerElement parent, int insertIndex)
        {
            Parent = parent;
            parent.GetExplorerElements().Insert(insertIndex, this);
        }

        public void RemoveFromParent()
        {
            Parent.GetExplorerElements().Remove(this);
            Parent = null;
        }

        public void Created(int insertIndex)
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnCreated(insertIndex);
            }
        }

        public void Deleted()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].Delete();
            }
        }

        public void ChangedPosition()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].UpdatePosition();
            }
        }

        public IExplorerElement Clone()
        {
            ExplorerElementFolder newFolder = new ExplorerElementFolder();
            newFolder.path = new string(this.path); // we need this path in paste command.
            newFolder.Parent = this.Parent;
            List<IExplorerElement> explorerElements = new List<IExplorerElement>();
            this.explorerElements.ForEach(element => newFolder.explorerElements.Add(element.Clone()));

            return newFolder;
        }

        public List<ExplorerElementTrigger> GetReferrers()
        {
            return new List<ExplorerElementTrigger>();
        }
    }
}