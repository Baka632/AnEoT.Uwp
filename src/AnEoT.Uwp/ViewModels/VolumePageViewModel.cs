using System.Collections.ObjectModel;
using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Models.Navigation;
using AnEoT.Uwp.Services;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace AnEoT.Uwp.ViewModels;

/// <summary>
/// <see cref="VolumePage"/> 的视图模型
/// </summary>
public sealed class VolumePageViewModel : NotificationObject
{
    private readonly StorageFolder assetsFolder;
    private readonly IVolumeProvider volumeProvider;
    private VolumeInfo _VolumeInfo;
    private BitmapImage _VolumeCover;

    public VolumePageViewModel(InlineCollection inlines)
    {
        assetsFolder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
        StorageFolder testFolder = assetsFolder.GetFolderAsync("Test").AsTask().Result;
        StorageFolder postsFolder = testFolder.GetFolderAsync("posts").AsTask().Result;

        volumeProvider = new FileVolumeProvider(postsFolder.Path);
        Inlines = inlines;
    }

    public InlineCollection Inlines { get; }

    public VolumeInfo VolumeInfo
    {
        get => _VolumeInfo;
        set
        {
            _VolumeInfo = value;
            OnPropertiesChanged();
        }
    }

    public BitmapImage VolumeCover
    {
        get => _VolumeCover;
        set
        {
            _VolumeCover = value;
            OnPropertiesChanged();
        }
    }

    #region InfoBar Props & Fields

    private bool _InfoBarOpen;
    private string _InfoBarTitle;
    private string _InfoBarMessage;
    private InfoBarSeverity _InfoBarSeverity;

    public bool InfoBarOpen
    {
        get => _InfoBarOpen;
        set
        {
            _InfoBarOpen = value;
            OnPropertiesChanged();
        }
    }

    public string InfoBarTitle
    {
        get => _InfoBarTitle;
        set
        {
            _InfoBarTitle = value;
            OnPropertiesChanged();
        }
    }

    public string InfoBarMessage
    {
        get => _InfoBarMessage;
        set
        {
            _InfoBarMessage = value;
            OnPropertiesChanged();
        }
    }

    public InfoBarSeverity InfoBarSeverity
    {
        get => _InfoBarSeverity;
        set
        {
            _InfoBarSeverity = value;
            OnPropertiesChanged();
        }
    }
    #endregion

    public ObservableCollection<BreadcrumbBarItemInfo> BreadcrumbBarSource { get; } = new();

    /// <summary>
    /// 准备页面内容
    /// </summary>
    /// <param name="volumeRawName">目标原始期刊名称</param>
    public async Task PreparePage(string volumeRawName)
    {
        try
        {
            VolumeInfo volumeInfo = await volumeProvider.GetVolumeInfoAsync(volumeRawName);

            BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo("主页", () =>
            {
                NavigationHelper.Navigate(typeof(Views.MainFrame.MainFrame), null);
            }));
            BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo("期刊列表", () =>
            {
                NavigationHelper.Navigate(typeof(VolumeList), null);
            }));
            BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo(volumeInfo.Name, () =>
            {
                NavigationHelper.Navigate(typeof(VolumePage), volumeInfo.RawName);
            }));
            VolumeInfo = volumeInfo;

            Uri uri = await FileHelper.GetVolumeCover(volumeInfo.RawName);
            BitmapImage volCover = new(uri);
            VolumeCover = volCover;

            string readmeMarkdown = await FileHelper.GetVolumeReadmeMarkdown(volumeInfo.RawName);
            MarkdownDocument doc = Markdown.Parse(readmeMarkdown, MarkdownHelper.Pipeline);

            ListBlock volArticleListBlock = doc.Descendants<ListBlock>().First();

            foreach (Markdig.Syntax.Block item in volArticleListBlock)
            {
                if (item is ListItemBlock listBlock)
                {
                    ParseListBlock(listBlock, Inlines);
                }
            }
        }
        catch (DirectoryNotFoundException)
        {
            SetInfoBar("指定的期刊无效", "请检查输入的期刊名称是否正确", true, InfoBarSeverity.Error);
        }
    }

    private void ParseListBlock(ListItemBlock listBlock, InlineCollection addTarget, bool isBaseElement = true, int indent = 0)
    {
        for (int i = 0; i < indent; i++)
        {
            addTarget.Add(new Run() { Text = "    " });
        }

        IEnumerable<ListItemBlock> innerListBlocks = listBlock.Descendants<ListItemBlock>();

        if (innerListBlocks.Any())
        {
            LiteralInline literalInline = listBlock.Descendants<LiteralInline>().FirstOrDefault();
            if (literalInline is not null)
            {
                addTarget.Add(CreateSoildPaginationDot());
                string displayName = literalInline.Content.ToString();
                addTarget.Add(new Run() { Text = $"  {displayName}" });
                addTarget.Add(new LineBreak());
            }

            foreach (ListItemBlock item in innerListBlocks)
            {
                ParseListBlock(item, addTarget, false, indent + 1);
            }
        }
        else
        {
            if (isBaseElement)
            {
                LiteralInline literalInline = listBlock.Descendants<LiteralInline>().FirstOrDefault();
                LinkInline linkInline = listBlock.Descendants<LinkInline>().FirstOrDefault();
                if (literalInline is not null && linkInline is not null)
                {
                    string displayName = literalInline.Content.ToString();
                    addTarget.Add(CreateSoildPaginationDot());
                    addTarget.Add(new Run() { Text = "  " });

                    Hyperlink hyperlink = new();
                    hyperlink.Inlines.Add(new Run() { Text = $"{displayName}" });
                    hyperlink.Click += (sender, args) =>
                    {
                        NavigationHelper.Navigate(typeof(ReadPage), new ArticleNavigationInfo(
                            _VolumeInfo.RawName,
                            linkInline.Url.TrimEnd(".html".ToCharArray())),new SlideNavigationTransitionInfo());
                    };
                    addTarget.Add(hyperlink);
                }
            }
            else
            {
                LiteralInline literalInline = listBlock.Descendants<LiteralInline>().FirstOrDefault();
                LinkInline linkInline = listBlock.Descendants<LinkInline>().FirstOrDefault();
                if (literalInline is not null)
                {
                    string displayName = literalInline.Content.ToString();

                    addTarget.Add(CreateOutlinePaginationDot());
                    addTarget.Add(new Run() { Text = "  " });

                    if (linkInline is not null)
                    {
                        Hyperlink hyperlink = new();
                        hyperlink.Inlines.Add(new Run() { Text = displayName });
                        hyperlink.Click += (sender, args) =>
                        {
                            NavigationHelper.Navigate(typeof(ReadPage), new ArticleNavigationInfo(
                                _VolumeInfo.RawName,
                                linkInline.Url.TrimEnd(".html".ToCharArray())));
                        };
                        addTarget.Add(hyperlink);
                    }
                }
            }

            addTarget.Add(new LineBreak());
        }
    }

    private static Run CreateSoildPaginationDot()
    {
        return new()
        {
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            Text = "\uF127",
            FontSize = 9
        };
    }
    
    private static Run CreateOutlinePaginationDot()
    {
        return new()
        {
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            Text = "\uF126",
            FontSize = 9
        };
    }

    public void SetInfoBar(string title, string message, bool isOpen, InfoBarSeverity severity)
    {
        InfoBarTitle = title;
        InfoBarMessage = message;
        InfoBarSeverity = severity;
        InfoBarOpen = isOpen;
    }
}
