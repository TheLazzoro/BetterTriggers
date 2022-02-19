using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Model.EditorData
{
    public class ExplorerElementVariable : IExplorerElement
    {
        public string path;
        public Variable variable;
        public List<IExplorerElementObserver> observers = new List<IExplorerElementObserver>();

        public ExplorerElementVariable(string path)
        {
            this.path = path;
            string json = File.ReadAllText(path);
            variable = JsonConvert.DeserializeObject<Variable>(json);
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

        public void Attach(IExplorerElementObserver observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IExplorerElementObserver observer)
        {
            this.observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update(this);
            }
        }

        public void DeleteObservers()
        {
            foreach (var observer in observers)
            {
                observer.Delete();
            }
        }

        public void SaveInMemory(string saveableString)
        {
            variable = JsonConvert.DeserializeObject<Variable>(saveableString);
        }

        public int GetId()
        {
            return variable.Id;
        }
    }
}