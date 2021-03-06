﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace Adapt.Presentation.Controls
{
    public partial class TreeViewItem : StackLayout, IDisposable
    {
        #region Fields

        private TreeViewItem _ParentTreeViewItem;

        private DateTime _ExpandButtonClickedTime;

        private readonly BoxView _SpacerBoxView = new BoxView();

        /// <summary>
        /// TODO: Remove this. It's nasty. It's here because double tap causes strange behaviour. Ultimately, it would be best to double tap the item to expand it, but the gesture recognizer ends up firing for the parent node as well which looks like a bug in XF
        /// </summary>
        private readonly Button _ExpandButton = new Button { Text = "-", HeightRequest = 20, WidthRequest = 30 };

        private readonly Grid _MainGrid = new Grid
        {
            VerticalOptions = LayoutOptions.StartAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            RowSpacing = 2
        };

        private readonly StackLayout _ContentStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

        private readonly ContentView _HeaderView = new ContentView
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
        };

        private readonly StackLayout _ChildrenStackLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Spacing = 0
        };

        private ObservableCollection<TreeViewItem> _ItemsSource = new ObservableCollection<TreeViewItem>();
        private readonly TapGestureRecognizer _TapGestureRecognizer = new TapGestureRecognizer();
        #endregion

        #region Internal Fields
        internal readonly BoxView SelectionBoxView = new BoxView { Color = Color.Blue, Opacity = .5, IsVisible = false };
        #endregion

        #region Private Properties
        private TreeView ParentTreeView => Parent?.Parent as TreeView;
        private double IndentWidth => Depth * SpacerWidth;
        private int SpacerWidth { get; set; } = 30;
        private int Depth => (ParentTreeViewItem == null ? 0 : ParentTreeViewItem.Depth + 1);
        #endregion

        #region Protected Overrides
        protected override void OnParentSet()
        {
            base.OnParentSet();
            Render();
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// TODO: Remove this. We should be able to get the ParentTreeViewNode by traversing up through the Visual Tree by 'Parent', but this not working for some reason.
        /// </summary>
        internal TreeViewItem ParentTreeViewItem
        {
            get
            {
                return _ParentTreeViewItem;
            }
            set
            {
                _ParentTreeViewItem = value;
                Render();
            }
        }
        #endregion

        #region Public Properties

        public bool IsSelected
        {
            get
            {
                return SelectionBoxView.IsVisible;
            }
            set
            {
                SelectionBoxView.IsVisible = value;
            }
        }
        public bool IsExpanded
        {
            get
            {
                return _ChildrenStackLayout.IsVisible;
            }
            set
            {
                _ChildrenStackLayout.IsVisible = value;


                _ExpandButton.Text = value ? "-" : "+";
            }
        }

        public View Header
        {
            get { return _HeaderView.Content; }
            set { _HeaderView.Content = value; }
        }

        public ObservableCollection<TreeViewItem> ItemsSource
        {
            get
            {
                return _ItemsSource;
            }
            set
            {
                if (_ItemsSource != null)
                {
                    _ItemsSource.CollectionChanged -= ItemsSource_CollectionChanged;
                }

                _ItemsSource = value;
                _ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;
                TreeView.RenderNodes(_ItemsSource, _ChildrenStackLayout, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), this);
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a new TreeViewItem
        /// </summary>
        public TreeViewItem()
        {
            _ExpandButton.Clicked += ExpandButton_Clicked;

            _ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

            IsExpanded = true;

            _TapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            GestureRecognizers.Add(_TapGestureRecognizer);

            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            _MainGrid.Children.Add(SelectionBoxView);

            _ContentStackLayout.Children.Add(_SpacerBoxView);
            _ContentStackLayout.Children.Add(_ExpandButton);
            _ContentStackLayout.Children.Add(_HeaderView);

            _MainGrid.Children.Add(_ContentStackLayout);
            _MainGrid.Children.Add(_ChildrenStackLayout, 0, 1);

            Children.Add(_MainGrid);

            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.Start;

            Render();
        }

        #endregion

        #region Public Methods
        public void Dispose()
        {
            foreach (var childTreeViewItem in _ItemsSource)
            {
                childTreeViewItem.Dispose();
            }

            Children.Clear();

            _ItemsSource.CollectionChanged -= ItemsSource_CollectionChanged;
            _TapGestureRecognizer.Tapped -= TapGestureRecognizer_Tapped;
            _ExpandButton.Clicked -= ExpandButton_Clicked;
            GestureRecognizers.Remove(_TapGestureRecognizer);
        }
        #endregion

        #region Internal Methods
        internal void Render()
        {
            _SpacerBoxView.WidthRequest = IndentWidth;

            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    item.Render();
                }
            }
        }

        /// <summary>
        /// TODO: This is a little stinky...
        /// </summary>
        internal void ChildSelected(TreeViewItem child)
        {
            ParentTreeViewItem?.ChildSelected(child);
            ParentTreeView?.ChildSelected(child);
        }
        #endregion

        #region Event Handlers
        private void ExpandButton_Clicked(object sender, EventArgs e)
        {
            _ExpandButtonClickedTime = DateTime.Now;
            IsExpanded = !IsExpanded;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //TODO: Hack. We don't want the node to become selected when we are clicking on the expanded button
            if (DateTime.Now - _ExpandButtonClickedTime > new TimeSpan(0, 0, 0, 0, 50))
            {
                ChildSelected(this);
            }
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TreeView.RenderNodes(_ItemsSource, _ChildrenStackLayout, e, this);
        }

        #endregion
    }
}