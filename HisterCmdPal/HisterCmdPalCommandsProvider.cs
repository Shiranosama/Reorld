using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace HisterCmdPal;

public partial class HisterCmdPalCommandsProvider : CommandProvider
{
  private readonly ICommandItem[] _commands;

  public HisterCmdPalCommandsProvider()
  {
    DisplayName = "Reorld";
    Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
    Settings = HisterCmdPalPage._settings.Settings;
    _commands = [
        new CommandItem(new HisterCmdPalPage()) { Title = DisplayName },
        ];
  }

  public override ICommandItem[] TopLevelCommands()
  {
    return _commands;
  }

}
