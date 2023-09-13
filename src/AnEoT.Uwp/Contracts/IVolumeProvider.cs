namespace AnEoT.Uwp.Contracts;

/// <summary>
/// 定义一套与获取期刊信息相关的方法
/// </summary>
public interface IVolumeProvider
{
    /// <summary>
    /// 获取指定的期刊
    /// </summary>
    /// <param name="volume">形如“2023-05”的期刊名</param>
    /// <returns>表示指定期刊的 <see cref="VolumeInfo"/></returns>
    Task<VolumeDetail> GetVolumeAsync(string volume);

    /// <summary>
    /// 获取指定期刊的信息
    /// </summary>
    /// <param name="volume">形如“2023-05”的期刊名</param>
    /// <returns>指定期刊的信息</returns>
    Task<VolumeInfo> GetVolumeInfoAsync(string volume);

    /// <summary>
    /// 获取最新的期刊
    /// </summary>
    /// <returns>表示最新期刊的 <see cref="VolumeInfo"/></returns>
    Task<VolumeDetail> GetLatestVolumeAsync();

    /// <summary>
    /// 获取最新期刊的信息
    /// </summary>
    /// <returns>最新期刊的信息</returns>
    Task<VolumeInfo> GetLatestVolumeInfoAsync();

    /// <summary>
    /// 获取所有期刊的列表
    /// </summary>
    /// <returns>期刊列表</returns>
    Task<IEnumerable<VolumeListItem>> GetVolumeListAsync();
}
