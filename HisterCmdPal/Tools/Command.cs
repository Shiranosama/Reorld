using System;
using HisterCmdPal;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace histerReorld.Toolkit
{
  internal sealed partial class OpenCommand : InvokableCommand
  {
    private readonly string _url;

    internal OpenCommand(string url)
    {
      _url = url;
    }

    public override CommandResult Invoke()
    {
      if (ShellHelpers.OpenInShell(_url))
      {
        return CommandResult.Dismiss();
      }
      else
      {
        return CommandResult.KeepOpen();
      }
    }
  }

  internal sealed partial class SearchInHister : InvokableCommand
  {
    private readonly string _queryUrl;

    internal SearchInHister(string query)
    {
      _queryUrl = "http://" + HisterCmdPalPage._settings.SetServer + "/?q=" + Uri.EscapeDataString(query);
    }

    public override CommandResult Invoke()
    {
      if (ShellHelpers.OpenInShell(_queryUrl))
      {
        return CommandResult.Dismiss();
      }
      else
      {
        return CommandResult.KeepOpen();
      }
    }
  }
}