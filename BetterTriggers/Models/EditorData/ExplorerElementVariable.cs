using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElementVariable : IExplorerElement, IExplorerSaveable
    {
        public string path;
        public Variable variable;
        public List<IExplorerElementUI> observers = new List<IExplorerElementUI>();
        private DateTime LastWrite;
        private long Size;

        private IExplorerElement Parent;

        public ExplorerElementVariable() { }

        public ExplorerElementVariable(string path)
        {
            this.path = path;
            string json = File.ReadAllText(path);
            variable = JsonConvert.DeserializeObject<Variable>(json);
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
            this.variable.Name = Path.GetFileNameWithoutExtension(newPath);
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
            return variable.Id;
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

        public string GetSaveableString()
        {
            return JsonConvert.SerializeObject(variable, Formatting.Indented);
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
        }

        public void Created(int insertIndex)
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnCreated(insertIndex);
            }
        }

        public void OnRemoteChange()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnRemoteChange();
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
            ExplorerElementVariable newVariable = new ExplorerElementVariable(); 
            newVariable.path = new string(this.path); // we need this path in paste command.
            newVariable.Parent = this.Parent;
            newVariable.variable = this.variable.Clone();
            return newVariable;
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
            return References.GetReferreres(variable);
        }
    }
}