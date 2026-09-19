using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CommandPalette.Extensions;

namespace HisterCmdPal;

[Guid("d9ab3aca-2db6-4889-891b-be08d134e152")]
public sealed partial class HisterCmdPal : IExtension, IDisposable
{
  private readonly ManualResetEvent _extensionDisposedEvent;

  private readonly HisterCmdPalCommandsProvider _provider = new();

  public HisterCmdPal(ManualResetEvent extensionDisposedEvent)
  {
    this._extensionDisposedEvent = extensionDisposedEvent;
  }

  public object? GetProvider(ProviderType providerType)
  {
    return providerType switch
    {
      ProviderType.Commands => _provider,
      _ => null,
    };
  }

  public void Dispose() => this._extensionDisposedEvent.Set();
}
