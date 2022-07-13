using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BetterTriggers.Controllers;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElementScript : IExplorerElement, IExplorerSaveable
    {
        public string path;
        public string script;
        public bool isEnabled = true;
        public List<IExplorerElementUI> observers = new List<IExplorerElementUI>();
        private DateTime LastWrite;
        private long Size;

        private IExplorerElement Parent;

        public ExplorerElementScript() { }

        public ExplorerElementScript(string path)
        {
            this.path = path;
            ControllerScript controller = new ControllerScript();
            this.script = controller.LoadScriptFromFile(GetPath());
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
            ControllerScript controller = new ControllerScript();
            this.script = controller.LoadScriptFromFile(GetPath());
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
            throw new NotImplementedException();
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
            //throw new NotImplementedException();
        }

        public bool GetEnabled()
        {
            return this.isEnabled;
        }

        public bool GetInitiallyOn()
        {
            return true;
        }

        public string GetSaveableString()
        {
            return script;
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
            this.Parent = parent;
            parent.GetExplorerElements().Insert(insertIndex, this);
        }

        public void RemoveFromParent()
        {
            this.Parent.GetExplorerElements().Remove(this);
            this.Parent = null;
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
            ExplorerElementScript newScript = new ExplorerElementScript();
            newScript.path = new string(this.path); // we need this path in paste command.
            newScript.Parent = this.Parent;
            newScript.isEnabled = this.isEnabled;
            newScript.script = new string(this.script);

            return newScript;
        }

        public void OnSaved()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnSaved();
            }
        }
    }
}