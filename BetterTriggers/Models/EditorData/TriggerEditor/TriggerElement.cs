﻿using System;
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
        public bool IsSelected { get; set; }
        private TriggerElement? Parent;

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

        public void SetParent(TriggerElementCollection parent)
        {
            this.Parent = parent;
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