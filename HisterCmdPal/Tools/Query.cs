using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using HisterCmdPal;
using HisterCmdPal.Toolkit;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace histerReorld.Toolkit
{
  internal class QueryBody
  {
    public string Text { get; set; }
    public int Limit { get; set; }
    public string Sort { get; set; }
  }
  [JsonSerializable(typeof(QueryBody))]
  internal partial class AppJsonContext : JsonSerializerContext { }

  internal static class Query
  {
    private readonly static HttpClient httpcli = new();
    internal static async Task<List<SearchResult>> Search(string queryJson, CancellationToken token)
    {
      string url = "http://" + HisterCmdPalPage._settings.SetServer + "/search?format=json&query=" + Uri.EscapeDataString(queryJson);
      var resultList = new List<SearchResult>();

      try
      {
        var respone = await httpcli.GetAsync(url, token);
        if (respone.IsSuccessStatusCode)
        {
          string reJson = await respone.Content.ReadAsStringAsync();
          JsonNode? jsonNode = JsonNode.Parse(reJson);

          if (jsonNode != null)
          {
            JsonArray? documents = jsonNode["documents"]?.AsArray();
            if (documents != null)
            {
              foreach (JsonNode? doc in documents)
              {
                SearchResult res = new();
                res.Title = doc?["title"]?.GetValue<string>() ?? "¯\\\\_(ツ)_/¯";
                res.Url = doc?["url"]?.GetValue<string>() ?? "¯\\\\_(ツ)_/¯";

                string faviconUrl = "http://" + HisterCmdPalPage._settings.SetServer + "/api/favicon?key=" + doc?["favicon_key"]?.GetValue<string>();
                var data = new IconData(faviconUrl);
                res.Favicon = new IconInfo(data, data);

                if (HisterCmdPalPage._settings.SetPreviewLimit == 0)
                {
                  res.Description = doc?["metadata"]?["description"]?.GetValue<string>() ?? "¯\\\\_(ツ)_/¯";
                }
                else
                {
                  res.Description = null;
                }

                resultList.Add(res);
              }
            }
          }
        }
      }
      catch
      {
        return null;
      }
      return resultList;
    }
    internal static async Task<string> Preview(string queryUrl, int previewLimit, CancellationToken token)
    {
      string url = "http://" + HisterCmdPalPage._settings.SetServer + "/api/preview?url=" + Uri.EscapeDataString(queryUrl) + "&extractor=Basic";
      string result = "¯\\\\_(ツ)_/¯";

      try
      {
        var respone = await httpcli.GetAsync(url, token);

        if (respone.IsSuccessStatusCode)
        {
          string reJson = await respone.Content.ReadAsStringAsync();
          JsonNode? jsonNode = JsonNode.Parse(reJson);

          if (jsonNode != null)
          {
            var content = jsonNode["content"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(content))
            {
              if (content.Length > previewLimit)
              {
                result = content[..previewLimit];
              }
              else
              {
                result = content;
              }
            }
          }
        }
      }
      catch { }
      return result;
    }

    internal static List<ListItem> APIError()
    {
      List<ListItem> items = [new ListItem(new NoOpCommand()) { Title = "Failed to query. Check your server setting", },];
      return items;
    }
  }
}