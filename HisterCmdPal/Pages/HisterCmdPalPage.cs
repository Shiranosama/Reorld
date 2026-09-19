using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using HisterCmdPal.Toolkit;

using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using histerReorld.Toolkit;
using System.Text.Json;
using System;

namespace HisterCmdPal;

internal partial class HisterCmdPalPage : DynamicListPage, IDisposable
{
  private readonly List<IListItem> _results = [];
  private readonly Channel<string> _queryChannel;
  private readonly CancellationTokenSource _disCts = new();
  internal static readonly SettingsManager _settings = new();

  public HisterCmdPalPage()
  {
    Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
    Name = "Hister";
    ShowDetails = true;

    _queryChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(1)
    {
      FullMode = BoundedChannelFullMode.DropOldest,
      SingleReader = true,
      SingleWriter = false
    });

    _ = Task.Run(QueriesAsync);
  }

  public override IListItem[] GetItems() => [.. _results];

  private async Task QueriesAsync()
  {
    var reader = _queryChannel.Reader;
    try
    {
      await foreach (string query in reader.ReadAllAsync(_disCts.Token))
      {
        IsLoading = true;
        try
        {
          _results.Clear();
          var _queryBody = new QueryBody
          {
            Text = query,
            Limit = _settings.SetLimit,
            Sort = _settings.SetSort
          };

          string queryJson = JsonSerializer.Serialize(_queryBody, AppJsonContext.Default.QueryBody);
          List<SearchResult> results = await Query.Search(queryJson, _disCts.Token);

          if (reader.TryRead(out string newerQuery))
          {
            DisposeResults(results);
            _queryChannel.Writer.TryWrite(newerQuery);
            IsLoading = false;
            continue;
          }

          if (results == null)
          {
            _results.AddRange(Query.APIError());
          }
          else
          {
            foreach (var res in results)
            {
              ListItem resultItem = new(new OpenCommand(res.Url))
              {
                Title = res.Title,
                Subtitle = res.Url,
                Icon = res.Favicon,
              };

              if (res.Description != null)
              {
                resultItem.Details = new Details()
                {
                  Title = res.Title,
                  Body = res.Description,
                };
              }
              else
              {
                resultItem.Details = new Details()
                {
                  Title = res.Title,
                  Body = await Query.Preview(res.Url, _settings.SetPreviewLimit, _disCts.Token),
                };
              }
              _results.Add(resultItem);
            }
            if (_results.Count != 0)
            {
              _results.Add(new ListItem(new SearchInHister(query))
              {
                Title = "Search in hister webUI"
              });
            }
          }
          IsLoading = false;
          RaiseItemsChanged(_results.Count);
        }
        catch (OperationCanceledException) { break; }
        catch { IsLoading = false; }
      }
    }
    catch (OperationCanceledException) { }
  }
  private static void DisposeResults(List<SearchResult> results)
  {
    if (results == null) return;
    results.Clear();
  }


  public void NotifySearchTextChanged()
  {
    OnPropertyChanged(nameof(SearchText));
  }

  public override void UpdateSearchText(string oldSearch, string newSearch)
  {
    _queryChannel.Writer.TryWrite($"{newSearch}");
  }

  public void Dispose()
  {
    _queryChannel.Writer.Complete();
    _disCts.Cancel();
    _disCts.Dispose();
    GC.SuppressFinalize(this);
  }
}
