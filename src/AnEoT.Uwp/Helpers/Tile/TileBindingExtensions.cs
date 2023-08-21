﻿using System.IO;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AnEoT.Uwp.Helpers.Tile;

/// <summary>
/// 为 <see cref="TileBinding"/> 提供扩展方法的类
/// </summary>
public static class TileBindingExtensions
{
    /// <summary>
    /// 添加磁贴背景图片，此图片将位于所有磁贴内容之后，并以全出血（Full bleed）方式显示
    /// </summary>
    /// <param name="tile">要设置背景图片的 <see cref="TileBinding"/></param>
    /// <param name="source">图片的 Uri，此 Uri 可指向应用包、应用数据或者网络。来自网络的图像大小应小于 200 KB</param>
    /// <param name="hintOverlay">设置图像上黑色图层的透明度。设置成 0 将没有此黑色图层，设置为 100 会将此图层完全设置为黑色，默认值为 20</param>
    /// <param name="alternateText">设置图像的替代文本，以供辅助功能使用</param>
    /// <param name="addImageQuery">指示 Windows 是否应当向图像添加查询字符串的值，此查询字符串将限定图像的缩放、语言及高对比度设置。此功能需要 Uri 目标的支持</param>
    /// <param name="hintCrop">设置图像的裁剪类型</param>
    /// <returns>添加了背景图像的 <see cref="TileBinding"/>，其可用于进一步的配置</returns>
    public static TileBinding AddBackgroundImage(this TileBinding tile,
                                                                string source, int hintOverlay = 20,
                                                                string alternateText = "", bool? addImageQuery = null,
                                                                TileBackgroundImageCrop hintCrop = TileBackgroundImageCrop.Default)
    {
        TileBindingContentAdaptive content = ConvertITileBindingContentAsAdaptive(tile);

        TileBackgroundImage backgroundImage = new()
        {
            Source = source,
            HintOverlay = hintOverlay,
            AlternateText = alternateText,
            AddImageQuery = addImageQuery,
            HintCrop = hintCrop
        };

        content.BackgroundImage = backgroundImage;
        return tile;
    }

    /// <summary>
    /// 添加磁贴速览图片，此图片将从磁贴顶部滑入磁贴，之后滑出磁贴以显示主要信息
    /// </summary>
    /// <param name="tile">要设置“预览”图片的 <see cref="TileBinding"/></param>
    /// <param name="source">图片的 Uri，此 Uri 可指向应用包、应用数据或者网络。来自网络的图像大小应小于 200 KB</param>
    /// <param name="hintOverlay">设置图像上黑色图层的透明度。设置成 0 将没有此黑色图层，设置为 100 会将此图层完全设置为黑色，默认值为 20</param>
    /// <param name="alternateText">设置图像的替代文本，以供辅助功能使用</param>
    /// <param name="addImageQuery">指示 Windows 是否应当向图像添加查询字符串的值，此查询字符串将限定图像的缩放、语言及高对比度设置。此功能需要 Uri 目标的支持</param>
    /// <param name="hintCrop">设置图像的裁剪类型</param>
    /// <returns>添加了速览图像的 <see cref="TileBinding"/>，其可用于进一步的配置</returns>
    public static TileBinding AddPeekImage(this TileBinding tile,
                                                                string source, int hintOverlay = 20,
                                                                string alternateText = "", bool? addImageQuery = null,
                                                                TilePeekImageCrop hintCrop = TilePeekImageCrop.Default)
    {
        TileBindingContentAdaptive content = ConvertITileBindingContentAsAdaptive(tile);

        TilePeekImage peekImage = new()
        {
            Source = source,
            HintOverlay = hintOverlay,
            AlternateText = alternateText,
            AddImageQuery = addImageQuery,
            HintCrop = hintCrop,
        };

        content.PeekImage = peekImage;
        return tile;
    }

    /// <summary>
    /// 设置磁贴的显示名称。注意：这里的设置将覆盖全局设置
    /// </summary>
    /// <param name="tile">要设置显示名称的 <see cref="TileBinding"/></param>
    /// <param name="displayName">磁贴的显示名称</param>
    /// <returns>设置了显示名称的 <see cref="TileBinding"/>，其可用于进一步的配置</returns>
    public static TileBinding SetDisplayName(this TileBinding tile, string displayName)
    {
        tile.DisplayName = displayName;
        return tile;
    }

    /// <summary>
    /// 向磁贴中添加文本
    /// </summary>
    /// <param name="tile">要添加文本的 <see cref="TileBinding"/></param>
    /// <param name="text">文本字符串</param>
    /// <param name="hintWrap">指示是否启用文本换行的值</param>
    /// <param name="hintStyle">文本样式</param>
    /// <param name="hintAlign">用以配置文本水平布局的 <see cref="AdaptiveTextAlign"/></param>
    /// <param name="hintMaxLines">允许在磁贴中显示的最大行数，若设为 <see langword="null"/>，即为无限制</param>
    /// <param name="hintMinLines">在磁贴中显示的最少行数，若设为 <see langword="null"/>，即为无限制</param>
    /// <param name="language">配置磁贴文本的语言，应传入如“zh-CN”的 BCP-47 标记</param>
    /// <param name="bindings">包含磁贴文本的数据绑定的字典</param>
    /// <returns>添加了磁贴文本的 <see cref="TileBinding"/>，其可用于进一步的配置</returns>
    public static TileBinding AddAdaptiveText(this TileBinding tile, string text,
                                                             bool hintWrap = false,
                                                             AdaptiveTextStyle hintStyle = AdaptiveTextStyle.Default,
                                                             AdaptiveTextAlign hintAlign = AdaptiveTextAlign.Default,
                                                             int? hintMaxLines = null, int? hintMinLines = null,
                                                             string language = null,
                                                             IDictionary<AdaptiveTextBindableProperty, string> bindings = null)
    {
        TileBindingContentAdaptive content = ConvertITileBindingContentAsAdaptive(tile);

        AdaptiveText adaptiveText = new()
        {
            Text = text,
            HintWrap = hintWrap,
            HintStyle = hintStyle,
            HintAlign = hintAlign,
            HintMaxLines = hintMaxLines,
            HintMinLines = hintMinLines,
            Language = language,
        };

        if (bindings is not null && bindings.Count > 0)
        {
            foreach (KeyValuePair<AdaptiveTextBindableProperty, string> item in bindings)
            {
                adaptiveText.Bindings.Add(item);
            }
        }

        content.Children.Add(adaptiveText);
        return tile;
    }

    /// <summary>
    /// 配置磁贴文本的垂直布局
    /// </summary>
    /// <param name="tile">要设置磁贴文本垂直布局的 <see cref="TileBinding"/></param>
    /// <param name="stacking">用以配置磁贴文本垂直布局的 <see cref="TileTextStacking"/></param>
    /// <returns>配置了文本垂直布局的 <see cref="TileBinding"/>，其可用于进一步的配置</returns>
    public static TileBinding ConfigureTextStacking(this TileBinding tile, TileTextStacking stacking)
    {
        TileBindingContentAdaptive content = ConvertITileBindingContentAsAdaptive(tile);

        content.TextStacking = stacking;
        return tile;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TileBindingContentAdaptive ConvertITileBindingContentAsAdaptive(TileBinding tile)
    {
        if (tile.Content is TileBindingContentAdaptive adaptive)
        {
            return adaptive;
        }
        else
        {
            throw new ArgumentException("指定 TileBinding 的 Content 不是 TileBindingContentAdaptive，这是不可接受的", nameof(tile));
        }
    }
}
