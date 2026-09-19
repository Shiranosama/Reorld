using Microsoft.CommandPalette.Extensions.Toolkit;

namespace HisterCmdPal.Toolkit
{
  internal class SearchResult
  {
    internal string Title { get; set; } = string.Empty;
    internal string Url { get; set; } = string.Empty;
    internal string? Description { get; set; }
    internal IconInfo? Favicon { get; set; }
  }
}