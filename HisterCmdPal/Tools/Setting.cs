using System.Collections.Generic;
using System.IO;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace HisterCmdPal.Toolkit
{
  internal sealed partial class SettingsManager : JsonSettingsManager
  {
    private const string NameSpace = "HisterCmdPal";
    private static string NameSpaced(string propertyName) => $"{NameSpace}.{propertyName}";

    private readonly ChoiceSetSetting _sort = new(
      NameSpaced(nameof(SetSort)),
      "排序方式",
      "更改搜索排序方式",
      new List<ChoiceSetSetting.Choice>
      {
        new("相关性", "Relevance"),
        new("访问最多", "visits"),
        new("访问最少", "-visits"),
        new("最新", "date"),
        new("最早", "-date"),
        new("域名（A-Z）", "domain"),
        new("域名（Z-A）", "-domain")
      }
    );

    private readonly TextSetting _limit = new(
      NameSpaced(nameof(SetLimit)),
      "结果数量",
      "返回的结果数量",
      "10"
    );

    private readonly TextSetting _server = new(
      NameSpaced(nameof(SetServer)),
      "Hister Server",
      "Hister服务器域名/地址 eg. 127.0.0.1:4433",
      "127.0.0.1:4433"
    );

    private readonly TextSetting _previewLimit = new(
      NameSpaced(nameof(SetPreviewLimit)),
      "预览",
      "（实验性功能，性能不稳定）控制预览渲染字符上限（过高可能影响性能）；设置为0时跳过预览获取，使用短描述替代",
      "0"
    );

    public string SetSort => _sort.Value;
    public int SetLimit => int.TryParse(_limit.Value, out var val) ? val : 10;
    public string SetServer => _server.Value;
    public int SetPreviewLimit => int.TryParse(_previewLimit.Value, out var val) ? val : 2048;

    public SettingsManager()
    {
      var dir = Utilities.BaseSettingsPath("HisterCmdPal");
      Directory.CreateDirectory(dir);
      FilePath = Path.Combine(dir, $"{NameSpace}.settings.json");

      Settings.Add(_sort);
      Settings.Add(_limit);
      Settings.Add(_server);
      Settings.Add(_previewLimit);

      LoadSettings();

      Settings.SettingsChanged += (_, _) => SaveSettings();
    }
  }
}