using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElementTrigger : IExplorerElement, IExplorerSaveable
    {
        public string path;
        public Trigger trigger;
        public bool isEnabled = true;
        public bool isInitiallyOn = true;
        public List<IExplorerElementUI> observers = new List<IExplorerElementUI>();
        private DateTime LastWrite;
        private long Size;

        private IExplorerElement Parent;

        public ExplorerElementTrigger() { }

        public ExplorerElementTrigger(string path)
        {
            this.path = path;
            string json = string.Empty;
            bool isReadyForRead = false;
            int sleepTolerance = 100;
            while (!isReadyForRead)
            {
                try
                {
                    json = File.ReadAllText(path);
                    isReadyForRead = true;
                }
                catch (Exception ex)
                {
                    if (sleepTolerance < 0)
                        throw new Exception(ex.Message);

                    Thread.Sleep(100);
                    sleepTolerance--;
                }
            }
            trigger = JsonConvert.DeserializeObject<Trigger>(json);
            UpdateMetadata();
        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(path);
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

        public int GetId()
        {
            return trigger.Id;
        }

        public void SetEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
            foreach (var observer in observers)
            {
                observer.RefreshHeader();
            }
        }

        public void SetInitiallyOn(bool isInitiallyOn)
        {
            this.isInitiallyOn = isInitiallyOn;
            foreach (var observer in observers)
            {
                observer.RefreshHeader();
            }
        }

        public bool GetEnabled()
        {
            return this.isEnabled;
        }


        public bool GetInitiallyOn()
        {
            return this.isInitiallyOn;
        }

        public string GetSaveableString()
        {
            return JsonConvert.SerializeObject(trigger, Formatting.Indented);
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
            var info = new FileInfo(path);
            this.Size = info.Length;
            this.LastWrite = info.LastWriteTime;
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

        public List<IExplorerElement> GetExplorerElements()
        {
            throw new Exception("'" + path + "' is not a folder.");
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
            ExplorerElementTrigger newTrigger = new ExplorerElementTrigger();
            newTrigger.path = new string(this.path); // we need this path in paste command.
            newTrigger.Parent = this.Parent;
            newTrigger.isInitiallyOn = this.isInitiallyOn;
            newTrigger.isEnabled = this.isEnabled;
            newTrigger.trigger = this.trigger.Clone();

            return newTrigger;
        }

        public void OnSaved()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnSaved();
            }
        }

        public List<ExplorerElementTrigger> GetReferrers()
        {
            return References.GetReferreres(trigger);
        }
    }
}