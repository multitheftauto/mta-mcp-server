using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace MtaWikiMcp.Common.Clients;

/// <summary>
/// This class makes requests to the MTA wiki to gather information.
/// </summary>
public class WikiClient(HttpClient httpClient)
{
    private async Task<HtmlDocument> FetchPageAsync(string pageName)
    {
        var html = await httpClient.GetStringAsync(httpClient.BaseAddress + pageName);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }

    private static HtmlNode? GetParserOutput(HtmlDocument doc) =>
        doc.DocumentNode.SelectSingleNode("//div[contains(@class,'mw-parser-output')]");

    private static IEnumerable<HtmlNode> GetNodesAfterHeading(HtmlNode parserOutput, string headingId)
    {
        var heading = parserOutput.SelectSingleNode($".//span[@id='{headingId}']")?.ParentNode;
        if (heading is null)
            yield break;

        var node = heading.NextSibling;
        while (node is not null)
        {
            if (node.Name is "h2" or "h1")
                yield break;
            yield return node;
            node = node.NextSibling;
        }
    }

    private static string DecodeText(string text) =>
        WebUtility.HtmlDecode(text).Trim();

    private static string ExtractDescription(HtmlNode content)
    {
        var firstParagraph = content.SelectSingleNode("p");
        return firstParagraph is null ? string.Empty : DecodeText(firstParagraph.InnerText);
    }

    private static string ExtractSyntax(HtmlNode content)
    {
        var sb = new StringBuilder();
        string? pendingLabel = null;

        foreach (var node in GetNodesAfterHeading(content, "Syntax"))
        {
            if (node.Name is "h3" or "h2")
                break;

            if (node.Name == "pre")
            {
                sb.AppendLine(DecodeText(node.InnerText));
            }
            else if (node.Name == "p" && !string.IsNullOrWhiteSpace(node.InnerText))
            {
                sb.AppendLine(DecodeText(node.InnerText));
            }
            else if (node.Name == "div")
            {
                var cls = node.GetAttributeValue("class", "");
                if (cls.EndsWith("Header"))
                {
                    pendingLabel = string.Concat(node.ChildNodes
                        .Where(n => n.NodeType == HtmlNodeType.Text)
                        .Select(n => n.InnerText.Trim())).Trim();
                    if (string.IsNullOrEmpty(pendingLabel))
                        pendingLabel = null;
                }
                else if (cls.EndsWith("Content"))
                {
                    var preNodes = node.SelectNodes(".//pre");
                    if (preNodes is not null)
                    {
                        foreach (var preNode in preNodes)
                        {
                            if (pendingLabel is not null)
                            {
                                sb.AppendLine(pendingLabel);
                                pendingLabel = null;
                            }
                            sb.AppendLine(DecodeText(preNode.InnerText));
                        }
                    }
                }
            }
        }

        return sb.ToString().Trim();
    }

    private static string ExtractExample(HtmlNode content)
    {
        var sb = new StringBuilder();

        foreach (var node in GetNodesAfterHeading(content, "Example"))
        {
            if (node.Name == "h2")
                break;

            if (node.Name == "pre")
                sb.AppendLine(DecodeText(node.InnerText));
            else if (node.Name == "p" && !string.IsNullOrWhiteSpace(node.InnerText))
                sb.AppendLine(DecodeText(node.InnerText));
        }

        return sb.ToString().Trim();
    }

    public async Task<string> GetDescriptionAsync(string functionName)
    {
        var doc = await FetchPageAsync(functionName);
        var content = GetParserOutput(doc);
        if (content is null)
            return "Page not found.";
        var result = ExtractDescription(content);
        return string.IsNullOrEmpty(result) ? "Description not found." : result;
    }

    public async Task<string> GetSyntaxAsync(string functionName)
    {
        var doc = await FetchPageAsync(functionName);
        var content = GetParserOutput(doc);
        if (content is null)
            return "Page not found.";
        var result = ExtractSyntax(content);
        return string.IsNullOrEmpty(result) ? "Syntax not found." : result;
    }

    public async Task<string> GetFunctionInformationAsync(string functionName)
    {
        var doc = await FetchPageAsync(functionName);
        var content = GetParserOutput(doc);
        if (content is null)
            return "Page not found.";

        var sb = new StringBuilder();

        var description = ExtractDescription(content);
        if (!string.IsNullOrEmpty(description))
        {
            sb.AppendLine("Description:");
            sb.AppendLine(description);
            sb.AppendLine();
        }

        var syntax = ExtractSyntax(content);
        if (!string.IsNullOrEmpty(syntax))
        {
            sb.AppendLine("Syntax:");
            sb.AppendLine(syntax);
            sb.AppendLine();
        }

        var example = ExtractExample(content);
        if (!string.IsNullOrEmpty(example))
        {
            sb.AppendLine("Example:");
            sb.AppendLine(example);
        }

        var result = sb.ToString().Trim();
        return string.IsNullOrEmpty(result) ? "No information found." : result;
    }

    public Task<string> GetRawPageAsync(string functionName) =>
        httpClient.GetStringAsync(httpClient.BaseAddress + functionName);

    public async Task<string> GetFunctionListAsync(string pageName)
    {
        var doc = await FetchPageAsync(pageName);
        var content = GetParserOutput(doc);
        if (content is null)
            return "Page not found.";

        var sb = new StringBuilder();
        string currentSection = string.Empty;
        bool sectionPrinted = false;

        foreach (var node in content.ChildNodes)
        {
            if (node.Name is "h2" or "h3")
            {
                var span = node.SelectSingleNode(".//span[contains(@class,'mw-headline')]");
                var heading = DecodeText(span?.InnerText ?? node.InnerText);
                if (heading.Equals("Contents", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (sectionPrinted)
                    sb.AppendLine();

                currentSection = heading;
                sectionPrinted = false;
                continue;
            }

            if (node.Name is "ul" or "p")
            {
                if (string.IsNullOrEmpty(currentSection))
                    continue;

                var linkNodes = node.SelectNodes(".//a");
                if (linkNodes is null)
                    continue;

                var functions = new List<string>();
                foreach (var link in linkNodes)
                {
                    var href = link.GetAttributeValue("href", "");
                    var text = DecodeText(link.InnerText);
                    if (href.StartsWith("/wiki/")
                        && !href.Contains(':')
                        && !string.IsNullOrWhiteSpace(text)
                        && !text.Contains(' '))
                        functions.Add(text);
                }

                if (functions.Count == 0)
                    continue;

                if (!sectionPrinted)
                {
                    sb.AppendLine(currentSection + ":");
                    sectionPrinted = true;
                }

                foreach (var fn in functions)
                    sb.AppendLine("  " + fn);
            }
        }

        var result = sb.ToString().Trim();
        return string.IsNullOrEmpty(result) ? "No functions found." : result;
    }

    public async Task<string> GetEventParametersAsync(string eventName)
    {
        var doc = await FetchPageAsync(eventName);
        var content = GetParserOutput(doc);
        if (content is null)
            return "Page not found.";

        var sb = new StringBuilder();

        bool hasParameters = false;
        foreach (var node in GetNodesAfterHeading(content, "Parameters"))
        {
            if (node.Name is "h2" or "h3")
                break;

            if (node.Name == "pre")
            {
                if (!hasParameters)
                {
                    sb.AppendLine("Parameters:");
                    hasParameters = true;
                }
                sb.AppendLine(DecodeText(node.InnerText));
            }
            else if (node.Name == "ul")
            {
                var items = node.SelectNodes("li");
                if (items is not null)
                {
                    foreach (var li in items)
                        sb.AppendLine("- " + DecodeText(li.InnerText));
                }
            }
        }

        if (!hasParameters)
            sb.AppendLine("No parameters.");

        sb.AppendLine();

        bool hasSource = false;
        foreach (var node in GetNodesAfterHeading(content, "Source"))
        {
            if (node.Name is "h2" or "h3")
                break;

            if (node.Name == "p" && !string.IsNullOrWhiteSpace(node.InnerText))
            {
                sb.AppendLine("Source:");
                sb.AppendLine(DecodeText(node.InnerText));
                hasSource = true;
                break;
            }
        }

        if (!hasSource)
            sb.AppendLine("Source: Not specified.");

        return sb.ToString().Trim();
    }

    public async Task<string> GetExampleAsync(string functionName)
    {
        var doc = await FetchPageAsync(functionName);
        var content = GetParserOutput(doc);
        if (content is null)
            return "Page not found.";
        var result = ExtractExample(content);
        return string.IsNullOrEmpty(result) ? "Example not found." : result;
    }

    public async Task<string> SearchWikiAsync(string query)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var doc = await FetchPageAsync($"Special:Search?search={encodedQuery}&go=Go");

        var resultNodes = doc.DocumentNode.SelectNodes("//ul[contains(@class,'mw-search-results')]/li");

        if (resultNodes is null || resultNodes.Count == 0)
        {
            // go=Go redirected to a direct page match — extract the page title from the heading
            var heading = doc.DocumentNode.SelectSingleNode("//h1[@id='firstHeading']");
            if (heading is not null)
            {
                var title = DecodeText(heading.InnerText);
                return $"Direct match found:\nPage: {title}\nUse '{Uri.EscapeDataString(title)}' with GetFunctionInformation or GetPageSource.";
            }

            return "No results found.";
        }

        var sb = new StringBuilder();
        foreach (var li in resultNodes)
        {
            var link = li.SelectSingleNode(".//div[contains(@class,'mw-search-result-heading')]//a")
                       ?? li.SelectSingleNode(".//a");
            if (link is null)
                continue;

            var href = link.GetAttributeValue("href", "");
            var title = DecodeText(link.InnerText);
            var pageName = href.StartsWith("/wiki/") ? href["/wiki/".Length..] : href;

            sb.AppendLine($"Page: {title}");
            sb.AppendLine($"Use with other tools: {pageName}");

            var snippet = li.SelectSingleNode(".//div[contains(@class,'searchresult')]");
            if (snippet is not null)
            {
                var description = DecodeText(snippet.InnerText);
                if (!string.IsNullOrWhiteSpace(description))
                    sb.AppendLine($"Description: {description}");
            }

            sb.AppendLine();
        }

        return sb.ToString().Trim();
    }
}
