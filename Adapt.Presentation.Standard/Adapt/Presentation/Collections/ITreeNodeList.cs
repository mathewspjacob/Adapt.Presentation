﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Adapt.Presentation.Collections
{
    public interface ITreeNodeList<T> : IList<ITreeNode<T>>, INotifyPropertyChanged
    {
        // usage: var myNewNode = node.Children.Add(new myNodeType("..."));

        new ITreeNode<T> Add(ITreeNode<T> node);
        //ITreeNode<T> Add(ITreeNode<T> node, bool updateParent);
    }
}