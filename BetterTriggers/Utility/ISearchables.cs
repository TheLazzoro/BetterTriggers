using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public interface ISearchables
    {
        void AttachList(ISearchablesObserverList list);
        void DetachList(ISearchablesObserverList list);
        void AttachCategories(ISearchablesObserverCategories categories);
        void DetachCategories(ISearchablesObserverCategories categories);
        void NotifyList();
        void NotifyCategories();
    }
}
