﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CosmosDbExplorer.Messages;
using CosmosDbExplorer.ViewModels;
using Prism.Events;
using Prism.Mvvm;

namespace CosmosDbExplorer.Infrastructure.Models
{
    public interface ITreeViewItemViewModel
    {
        ObservableCollection<TreeViewItemViewModel> Children { get; }
        bool HasDummyChild { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        bool IsLoading { get; set; }
        TreeViewItemViewModel Parent { get; }
    }

    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : BindableBaseEx
    {
        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        private bool _isExpanded;

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, IEventAggregator messenger, bool lazyLoadChildren)
        {
            Parent = parent;
            MessengerInstance = messenger;
            Children = new ObservableCollection<TreeViewItemViewModel>();

            messenger.GetEvent<RemoveNodeMessage>().Subscribe(altLink => OnRemoveNodeMessage(altLink));

            if (lazyLoadChildren)
            {
                Children.Add(DummyChild);
            }
        }

        private void OnRemoveNodeMessage(string altLink)
        {
            if (Parent != null)
            {
                if (this is IContent assetNode && assetNode.ContentId == altLink)
                {
                    //DispatcherHelper.RunAsync(() => Parent.Children.Remove(this));
                    Parent.Children.Remove(this);
                }
            }
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children { get; }

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return Children.Count == 1 && Children[0] == DummyChild; }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged(() => IsExpanded);
                }

                // Expand all the way up to the root.
                if (_isExpanded && Parent != null)
                {
                    Parent.IsExpanded = true;
                }

                // Lazy load the child items, if necessary.
                if (HasDummyChild)
                {
                    Children.Remove(DummyChild);
                    Task.Run(() => LoadChildren());
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected { get; set; }

        public void OnIsSelectedChanged()
        {
            MessengerInstance.GetEvent<TreeNodeSelectedMessage>().Publish(this);
        }

        public bool IsLoading { get; set; }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual Task LoadChildren()
        {
            return Task.FromResult<object>(null);
        }

        public TreeViewItemViewModel Parent { get; }

        public IEventAggregator MessengerInstance { get; }
    }

    public class TreeViewItemViewModel<TParent> : TreeViewItemViewModel
        where TParent : TreeViewItemViewModel
    {
        public TreeViewItemViewModel(TParent parent, IEventAggregator messenger, bool lazyLoadChildren)
            : base(parent, messenger, lazyLoadChildren)
        {
        }

        public new TParent Parent => base.Parent as TParent;
    }
}
