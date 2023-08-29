using System.Collections.ObjectModel;

namespace AnEoT.Uwp.Models;


public readonly struct ArticleTreeViewItem
{
    public string DisplayName { get; }
    public Uri NavigationUri { get; }
    public ICollection<ArticleTreeViewItem> Children { get; }
    public bool ContainsNavigationUri { get => NavigationUri is not null; }

    public ArticleTreeViewItem(string name, ICollection<ArticleTreeViewItem> children, Uri navigationUri = null)
    {
        DisplayName = name;
        Children = children;
        NavigationUri = navigationUri;
    }

    public override string ToString()
    {
        return DisplayName;
    }
}
