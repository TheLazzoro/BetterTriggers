using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public abstract class TriggerElement : TreeNodeBase
    {
        public TriggerElementType ElementType { get; set; }
        public ObservableCollection<TriggerElement>? Elements { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Fix for UI multi-select because 'IsSelected' calls itself recursively, causing a stackoverflow.
        /// </summary>
        public bool IsSelected_Multi
        {
            get => _isSelected_Multi;
            set
            {
                _isSelected_Multi = value;
                OnPropertyChanged();
            }
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
        private TriggerElement? Parent;
        private bool _isSelected;
        private bool _isSelected_Multi;
        private bool _isExpanded = true;

        public virtual TriggerElement Clone()
        {
            throw new Exception("'Clone' method must be overriden.");
        }

        public virtual SaveableData.TriggerElement_Saveable Serialize()
        {
            throw new Exception("'Serialize' method must be overriden.");
        }

        public void SetParent(TriggerElement Parent, int insertIndex)
        {
            Parent?.Elements?.Insert(insertIndex, this);
            this.Parent = Parent;
        }

        public void RemoveFromParent()
        {
            this.Parent?.Elements?.Remove(this);
            this.Parent = null;
        }

        public TriggerElement? GetParent()
        {
            return this.Parent;
        }

        public int Count()
        {
            return Elements.Count;
        }

        public void Insert(TriggerElement triggerElement, int insertIndex)
        {
            Elements.Insert(insertIndex, triggerElement);
        }

        public int IndexOf(TriggerElement element)
        {
            return Elements.IndexOf(element);
        }
    }
}
