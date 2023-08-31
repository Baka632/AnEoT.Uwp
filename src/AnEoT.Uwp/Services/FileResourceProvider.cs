using AnEoT.Uwp.Contracts;

namespace AnEoT.Uwp.Services;

/// <summary>
/// 基于本地文件的资源获取器
/// </summary>
public readonly struct FileResourceProvider : IResourceProvider
{
    /// <summary>
    /// 获取资源的基 Uri，末尾带斜杠
    /// </summary>
    public string BaseUri => "ms-appx:///Assets/Test/posts/";
}
