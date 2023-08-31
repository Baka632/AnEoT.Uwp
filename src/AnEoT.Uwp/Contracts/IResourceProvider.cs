namespace AnEoT.Uwp.Contracts;

/// <summary>
/// 定义一套与资源获取相关的方法
/// </summary>
public interface IResourceProvider
{

    /// <summary>
    /// 获取资源的基 Uri
    /// </summary>
    public string BaseUri { get; }
}
