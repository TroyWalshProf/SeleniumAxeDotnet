using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Selenium.Axe
{
    public static class HtmlReport
    {
        public static void CreateAxeHtmlReport(this IWebDriver webDriver, string destination)
        {
            AxeBuilder axeBuilder = new AxeBuilder(webDriver);
            webDriver.CreateAxeHtmlReport(axeBuilder.Analyze(), destination);
        }

        public static void CreateAxeHtmlReport(this IWebDriver webDriver, IWebElement context, string destination)
        {
            AxeBuilder axeBuilder = new AxeBuilder(webDriver);
            context.CreateAxeHtmlReport(axeBuilder.Analyze(context), destination);
        }
        public static void CreateAxeHtmlReport(this ISearchContext context, AxeResult results, string destination)
        {
            // Get the unwrapped element if we are using a wrapped element
            context = context is IWrapsElement ? (context as IWrapsElement).WrappedElement : context;
            
            HashSet<string> selectors = new HashSet<string>();
            int violationCount = GetCount(results.Violations, ref selectors);
            int incompleteCount = GetCount(results.Incomplete, ref selectors);
            int passCount = GetCount(results.Passes, ref selectors);
            int inapplicableCount = GetCount(results.Inapplicable, ref selectors);

            var doc = new HtmlDocument();

            var node = HtmlNode.CreateNode("<html lang=\"en\"><head><meta charset=\"utf-8\"><title>Accessibility Check</title><style></style></head><body></body></html>");
            doc.DocumentNode.AppendChild(node);

            HtmlCommentNode hcn = doc.CreateComment("<!DOCTYPE html>\r\n");
            HtmlNode htmlNode = doc.DocumentNode.SelectSingleNode("/html");
            doc.DocumentNode.InsertBefore(hcn, htmlNode);

            StringBuilder content = new StringBuilder();
            content.AppendLine(@".fullImage{");
            content.AppendLine($"content: url('{GetDataImageString(context)};border: 1px solid black;margin-left:1em;");
            content.AppendLine(@"}
.fullImage:hover {transform:scale(2.75);transform-origin: top left;}
p {}
.wrap .wrapTwo .wrapThree{margin:2px;max-width:70vw;}
.wrapOne {margin-left:1em;overflow-wrap:anywhere;}
.wrapTwo {margin-left:2em;overflow-wrap:anywhere;}
.wrapThree {margin-left:3em;overflow-wrap:anywhere;}
.emOne {margin-left:1em;overflow-wrap:anywhere;}
.emTwo {margin-left:2em;overflow-wrap:anywhere;}
.emThree {margin-left:3em;overflow-wrap:anywhere;}
.majorSection{border: 1px solid black;}
.findings{border-top:1px solid black;}
.htmlTable{border-top:double lightgray;width:100%;display:table;}");

            HtmlNode body = doc.DocumentNode.SelectSingleNode("//body");
            doc.DocumentNode.SelectSingleNode("//style").InnerHtml = content.ToString();

            var element = doc.CreateElement("h1");
            element.InnerHtml = "Accessiblity Check";
            body.AppendChild(element);

            element = doc.CreateElement("h3");
            element.InnerHtml = "Context:";
            body.AppendChild(element);

            content = new StringBuilder();
            content.AppendLine($"Url: {results.Url}<br>");
            content.AppendLine($"Orientation: {results.TestEnvironment.OrientationType}<br>");
            content.AppendLine($"Size: {results.TestEnvironment.WindowWidth} x {results.TestEnvironment.WindowHeight}<br>");
            content.AppendLine($"Time: {results.Timestamp}<br>");
            content.AppendLine($"User agent: {results.TestEnvironment.UserAgent}<br>");
            content.AppendLine($"Using: {results.TestEngineName} ({results.TestEngineVersion})");

            element = doc.CreateElement("div");
            element.SetAttributeValue("class", "emOne");
            element.SetAttributeValue("id", "reportContext");
            element.InnerHtml = content.ToString();
            body.AppendChild(element);

            element = doc.CreateElement("h3");
            element.InnerHtml = "Counts:" ;
            body.AppendChild(element);

            element = doc.CreateElement("div");
            element.SetAttributeValue("class", "emOne");
            content = new StringBuilder();
            content.AppendLine($" Violation: {violationCount}<br>");
            content.AppendLine($" Incomplete: {incompleteCount}<br>");
            content.AppendLine($" Pass: {passCount}<br>");
            content.AppendLine($" Inapplicable: {inapplicableCount}");
            element.InnerHtml = content.ToString();
            body.AppendChild(element);

            element = doc.CreateElement("h3");
            element.InnerHtml = "Image:";
            body.AppendChild(element);

            element = doc.CreateElement("img");
            element.SetAttributeValue("class", "fullImage");
            element.SetAttributeValue("width", "33%");
            element.SetAttributeValue("height", "auto");
            body.AppendChild(element);

            if (!string.IsNullOrEmpty(results.Error))
            {
                element = doc.CreateElement("h2");
                element.InnerHtml = "SCAN ERRORS:";
                body.AppendChild(element);

                var error = doc.CreateElement("div");
                error.SetAttributeValue("id", "ErrorMessage");
                error.InnerHtml = HttpUtility.HtmlEncode(results.Error);
                body.AppendChild(error);
            }

            element = doc.CreateElement("br");
            body.AppendChild(element);

            element = doc.CreateElement("br");
            body.AppendChild(element);

            var area = doc.CreateElement("div");
            body.AppendChild(area);

            if (violationCount > 0)
            {
                area.AppendChild(doc.CreateElement("br"));
                GetReadableAxeResults(results.Violations, "Violations", doc, area);
            }

            if (incompleteCount > 0)
            {
                area.AppendChild(doc.CreateElement("br"));
                GetReadableAxeResults(results.Incomplete, "Incomplete", doc, area);
            }

            if (passCount > 0)
            {
                area.AppendChild(doc.CreateElement("br"));
                GetReadableAxeResults(results.Passes, "Passes", doc, area);
            }

            if (inapplicableCount > 0)
            {
                area.AppendChild(doc.CreateElement("br"));
                GetReadableAxeResults(results.Inapplicable, "Inapplicable", doc, area);
            }

            doc.Save(destination, Encoding.UTF8);
        }

        private static int GetCount(AxeResultItem[] results, ref HashSet<string> uniqueList)
        {
            int count = 0;
            foreach (AxeResultItem item in results)
            {
                foreach (AxeResultNode node in item.Nodes)
                {
                    foreach (var target in node.Target)
                    {
                        count++;
                        uniqueList.Add(target.ToString());
                    }
                }

                // Still add one if no targets are included
                if (item.Nodes.Length == 0)
                {
                    count++;
                }
            }
            return count;
        }

        private static void GetReadableAxeResults(AxeResultItem[] results, string type, HtmlDocument doc, HtmlNode body)
        {
            var section = doc.CreateElement("div");
            section.SetAttributeValue("class", "majorSection");
            section.SetAttributeValue("id", type + "Section");
            body.AppendChild(section);

            var childEl = doc.CreateElement("h2");
            childEl.InnerHtml = type;
            section.AppendChild(childEl);

            int loops = 1;

            foreach (var element in results)
            {
                childEl = doc.CreateElement("div");
                childEl.SetAttributeValue("class", "findings");
                childEl.InnerHtml = $@"{loops++}: {HttpUtility.HtmlEncode(element.Help)}";
                section.AppendChild(childEl);

                StringBuilder content = new StringBuilder();
                content.AppendLine($"Description: {HttpUtility.HtmlEncode(element.Description)}<br>");
                content.AppendLine($"Help: {HttpUtility.HtmlEncode(element.Help)}<br>");
                content.AppendLine($"Help URL: <a href=\"{element.HelpUrl}\">{element.HelpUrl}</a><br>");

                if (!string.IsNullOrEmpty(element.Impact))
                {
                    content.AppendLine($"Impact: {HttpUtility.HtmlEncode(element.Impact)}<br>");
                }

                content.AppendLine($"Tags: {HttpUtility.HtmlEncode(string.Join(", ", element.Tags))}<br>");

                if (element.Nodes.Length > 0)
                {
                    content.AppendLine($"Element(s):");
                }

                var childEl2 = doc.CreateElement("div");
                childEl2.SetAttributeValue("class", "emTwo");
                childEl2.InnerHtml = content.ToString();
                childEl.AppendChild(childEl2);

                foreach (var item in element.Nodes)
                {
                    var elementNodes = doc.CreateElement("div");
                    elementNodes.SetAttributeValue("class", "htmlTable");
                    childEl.AppendChild(elementNodes);

                    var htmlAndSelectorWrapper = doc.CreateElement("div");
                    htmlAndSelectorWrapper.SetAttributeValue("class", "emThree");
                    elementNodes.AppendChild(htmlAndSelectorWrapper);

                    HtmlNode htmlAndSelector = doc.CreateTextNode("Html:");
                    htmlAndSelectorWrapper.AppendChild(htmlAndSelector);

                    htmlAndSelector = doc.CreateElement("p");
                    htmlAndSelector.SetAttributeValue("class", "wrapOne");
                    htmlAndSelector.InnerHtml = $"{HttpUtility.HtmlEncode(item.Html)}";
                    htmlAndSelectorWrapper.AppendChild(htmlAndSelector);

                    htmlAndSelector = doc.CreateTextNode("Selector(s):");
                    htmlAndSelectorWrapper.AppendChild(htmlAndSelector);

                    content = new StringBuilder();
                    htmlAndSelector = doc.CreateElement("p");
                    htmlAndSelector.SetAttributeValue("class", "wrapTwo");

                    foreach (var target in item.Target)
                    {
                        content.AppendLine($"{HttpUtility.HtmlEncode(target)}");
                    }

                    htmlAndSelector.InnerHtml = content.ToString();
                    htmlAndSelectorWrapper.AppendChild(htmlAndSelector);
                }
            }
        }

        private static string GetDataImageString(ISearchContext context)
        {
            ITakesScreenshot newScreen = (ITakesScreenshot)context;
            return $"data:image/png;base64,{Convert.ToBase64String(newScreen.GetScreenshot().AsByteArray)}');";
        }
    }
}
