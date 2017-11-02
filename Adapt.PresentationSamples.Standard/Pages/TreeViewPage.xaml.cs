﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Adapt.Presentation.Controls.TreeView;


namespace Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TreeViewPage : ContentPage
    {
        protected override void OnAppearing()
        {
            var node = new TreeViewNode();
            node.Content = new Label { Text = "Content"};

            var node2 = new TreeViewNode();
            node2.Content = new Label { Text = "Content 2" };

            var node3 = new TreeViewNode();
            node3.Content = new Label { Text = "Content 3" };
            node3.BackgroundColor = Color.Red;

            node.ChildTreeViewNodes.Add(node2);
            node2.ChildTreeViewNodes.Add(node3);

            TheTreeView.ChildTreeViewNodes.Add(node);
            base.OnAppearing();
        }

        public TreeViewPage()
        {
            InitializeComponent();
        }
    }
}