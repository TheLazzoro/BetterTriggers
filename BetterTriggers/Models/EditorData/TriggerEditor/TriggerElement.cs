﻿using BetterTriggers.Commands;
using BetterTriggers.Containers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetterTriggers.Models.EditorData
{
    public abstract class TriggerElement : TreeNodeBase
    {
        public TriggerElementType ElementType { get; set; }
        public ObservableCollection<TriggerElement>? Elements { get; set; }
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

            if(this is LocalVariable localVar)
            {
                Project.CurrentProject.Variables.AddLocalVariable(localVar);
            }
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
