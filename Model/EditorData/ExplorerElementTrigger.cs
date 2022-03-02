using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Model.EditorData
{
    public class ExplorerElementTrigger : IExplorerElement
    {
        public string path;
        public Trigger trigger;
        public List<IExplorerElementObserver> observers = new List<IExplorerElementObserver>();

        public ExplorerElementTrigger(string path)
        {
            this.path = path;
            string json = File.ReadAllText(path);
            trigger = JsonConvert.DeserializeObject<Trigger>(json);
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

        public void SaveInMemory(string saveableString)
        {
            trigger = JsonConvert.DeserializeObject<Trigger>(saveableString);
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

        public void AddToList(IExplorerElement element)
        {
            throw new Exception("This is not a directory");
        }

        public void RemoveFromList(IExplorerElement element)
        {
            throw new Exception("This is not a directory");
        }
    }
}