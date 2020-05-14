using GongSolutions.Wpf.DragDrop;
using Loadify.Extension;
using Loadify.Mod;
using Loadify.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Loadify.Models
{
    public class BaseView : IDropTarget, INotifyPropertyChanged
    {
        private bool isEnabled = false;
        private int index = 0;

        public Visibility Visibility { get; protected set; } = Visibility.Visible;
        public Brush brush { get; set; }
        public string Name { get; set; } = "Not Set";
        public string Id { get; set; } = string.Empty;
        public int Index { get => index; set { index = value; NotifyPropertyChanged(); } }
        public bool IsEnabled { get => isEnabled; set { isEnabled = value; NotifyPropertyChanged(); } }
        public ViewType ViewType { get; protected set; } = ViewType.Undef;
        public bool AllowDrop { get; set; } = false;
        public CollectionView Parent { get; set; }
        public bool AllowRename { get; set; } = false;
        DefaultDropHandler handler = new DefaultDropHandler();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void DragOver(IDropInfo dropInfo)
        {
            BaseView targetItem = dropInfo.TargetItem as BaseView;
            if (targetItem != null && !targetItem.AllowDrop)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
                handler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            BaseView sourceItem = dropInfo.Data as BaseView;
            BaseView targetItem = dropInfo.TargetItem as BaseView;

            if (dropInfo.DropTargetAdorner == DropTargetAdorners.Highlight)
            {
                if (targetItem is CollectionView item)
                {
                    if (item.Children == null)
                        item.Children = new System.Collections.ObjectModel.ObservableCollection<BaseView>();
                    item.Children.DispatchedAdd(sourceItem);
                    if (sourceItem.Parent is null)
                    {
                        sourceItem.Parent = item;
                        ((dropInfo.VisualTarget as TreeListView.TreeListView).ItemsSource as System.Collections.ObjectModel.ObservableCollection<BaseView>).DispatchedRemove(sourceItem);
                    }
                    else
                    {
                        sourceItem.Parent.Children.DispatchedRemove(sourceItem);
                        sourceItem.Parent = item;
                    }
                }
            }
            else
            {
                if (sourceItem == targetItem)
                    return;
                else if (targetItem is CollectionView CollectionItem)
                {
                    RemoveFromParentOrTree(dropInfo, sourceItem);
                    if (CollectionItem.Parent != null)
                    {
                        int count = CollectionItem.Parent.Children.Count;
                        if (dropInfo.InsertIndex < count)
                            CollectionItem.Parent.Children.DispatchedInsert(dropInfo.InsertIndex, sourceItem);
                        else
                            CollectionItem.Parent.Children.DispatchedAdd(sourceItem);

                        sourceItem.Parent = CollectionItem.Parent;
                    }
                    else
                    {
                        Reorder(dropInfo, sourceItem);
                    }
                }
                else if (targetItem is ModDataView ModItem)
                {
                    RemoveFromParentOrTree(dropInfo, sourceItem);
                    if (ModItem.Parent != null)
                    {
                        int count = ModItem.Parent.Children.Count;
                        if (dropInfo.InsertIndex < count)
                            ModItem.Parent.Children.DispatchedInsert(dropInfo.InsertIndex, sourceItem);
                        else
                            ModItem.Parent.Children.DispatchedAdd(sourceItem);

                        sourceItem.Parent = ModItem.Parent;
                    }
                    else
                    {
                        Reorder(dropInfo, sourceItem);
                    }
                }
                else
                {
                    Reorder(dropInfo, sourceItem);
                }
            }
            CollectionReorderHelper.ReOrderIndex((dropInfo.VisualTarget as TreeListView.TreeListView).ItemsSource as System.Collections.ObjectModel.ObservableCollection<BaseView>);
        }

        private void Reorder(IDropInfo dropInfo, BaseView sourceItem)
        {
            var collection = ((dropInfo.VisualTarget as TreeListView.TreeListView).ItemsSource as System.Collections.ObjectModel.ObservableCollection<BaseView>);
            RemoveFromParentOrTree(dropInfo, sourceItem);
            sourceItem.Parent = null;

            if (collection.Count < dropInfo.InsertIndex)
                collection.DispatchedAdd(sourceItem);
            else
                collection.DispatchedInsert(dropInfo.InsertIndex, sourceItem);
        }

        private void RemoveFromParentOrTree(IDropInfo dropInfo, BaseView source)
        {
            var collection = ((dropInfo.VisualTarget as TreeListView.TreeListView).ItemsSource as System.Collections.ObjectModel.ObservableCollection<BaseView>);
            if (source.Parent != null)
                source.Parent.Children.DispatchedRemove(source);
            if (source.Parent is null)
                collection.DispatchedRemove(source);
        }
    }
}
