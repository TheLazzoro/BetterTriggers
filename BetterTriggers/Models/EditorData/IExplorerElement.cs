using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public interface IExplorerElement
    {
        int GetId();
        string GetPath();
        string GetSaveablePath(); // Used in the project file.
        void SetPath(string newPath);
        string GetName();
        long GetSize();
        DateTime GetLastWrite();
        IExplorerElement GetParent();
        void SetParent(IExplorerElement parent, int insertIndex);
        void RemoveFromParent();
        List<IExplorerElement> GetExplorerElements();
        void UpdateMetadata();
        void Created(int insertIndex);
        void Deleted();
        void SetEnabled(bool isEnabled);
        void SetInitiallyOn(bool isInitiallyOn);
        bool GetEnabled();
        bool GetInitiallyOn();

        // Attach an observer to the subject.
        void Attach(IExplorerElementUI observer);

        // Detach an observer from the subject.
        void Detach(IExplorerElementUI observer);

        // Notify all observers about an event.
        void Notify();
        void DeleteObservers();
        void ChangedPosition();
        IExplorerElement Clone();
        List<ExplorerElementTrigger> GetReferrers();
    }
}
