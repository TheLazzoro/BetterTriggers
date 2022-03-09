using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.EditorData
{
    public interface IExplorerElement
    {
        int GetId();
        string GetPath();
        void SetPath(string newPath);
        string GetName();
        void SetEnabled(bool isEnabled);
        void SetInitiallyOn(bool isInitiallyOn);
        bool GetEnabled();
        bool GetInitiallyOn();
        void InsertIntoList(IExplorerElement element, int insertIndex);
        void RemoveFromList(IExplorerElement element);

        void SaveInMemory(string saveableString);
        // Attach an observer to the subject.
        void Attach(IExplorerElementObserver observer);

        // Detach an observer from the subject.
        void Detach(IExplorerElementObserver observer);

        // Notify all observers about an event.
        void Notify();
        void DeleteObservers();
    }
}
