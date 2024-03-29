﻿using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Text;
using System.Web;

namespace Selenium.Axe
{
    /// <summary>
    /// Findings to include in your HTML
    /// </summary>
    [Flags]
    public enum ReportTypes
    {
        Violations = 1,
        Incomplete = 2,
        Inapplicable = 4,
        Passes = 8,
        All = 15
    }

    public static class HtmlReport
    {
        public static void CreateAxeHtmlReport(this IWebDriver webDriver, string destination)
        {
            webDriver.CreateAxeHtmlReport(destination, ReportTypes.All);
        }

        public static void CreateAxeHtmlReport(this IWebDriver webDriver, string destination, ReportTypes requestedResults)
        {
            var axeBuilder = new AxeBuilder(webDriver);
            webDriver.CreateAxeHtmlReport(axeBuilder.Analyze(), destination, requestedResults);
        }

        public static void CreateAxeHtmlReport(this IWebDriver webDriver, IWebElement context, string destination)
        {
            webDriver.CreateAxeHtmlReport(context, destination, ReportTypes.All);
        }

        public static void CreateAxeHtmlReport(this IWebDriver webDriver, IWebElement context, string destination, ReportTypes requestedResults)
        {
            var axeBuilder = new AxeBuilder(webDriver);
            context.CreateAxeHtmlReport(axeBuilder.Analyze(context), destination, requestedResults);
        }

        public static void CreateAxeHtmlReport(this ISearchContext context, AxeResult results, string destination)
        {
            context.CreateAxeHtmlReport(results, destination, ReportTypes.All);
        }

        public static void CreateAxeHtmlReport(this ISearchContext context, AxeResult results, string destination, ReportTypes requestedResults)
        {
            // Get the unwrapped element if we are using a wrapped element
            context = context is IWrapsElement ? (context as IWrapsElement).WrappedElement : context;

            var violationCount = GetCount(results.Violations);
            var incompleteCount = GetCount(results.Incomplete);
            var passCount = GetCount(results.Passes);
            var inapplicableCount = GetCount(results.Inapplicable);

            var doc = new HtmlDocument();

            doc.CreateComment("<!DOCTYPE html>\r\n");

            var htmlStructure = HtmlNode.CreateNode("<html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Accessibility Check</title><style></style></head><body><content></content><script></script></body></html>");
            doc.DocumentNode.AppendChild(htmlStructure);

            doc.DocumentNode.SelectSingleNode("//style").InnerHtml = GetCss(context);

            var contentArea = doc.DocumentNode.SelectSingleNode("//content");

            var reportTitle = doc.CreateElement("h1");
            reportTitle.InnerHtml = "Accessibility Check";
            contentArea.AppendChild(reportTitle);

            var metaFlex = doc.CreateElement("div");
            metaFlex.SetAttributeValue("id", "metadata");
            contentArea.AppendChild(metaFlex);

            var contextGroup = doc.CreateElement("div");
            contextGroup.SetAttributeValue("id", "context");
            metaFlex.AppendChild(contextGroup);

            var contextHeader = doc.CreateElement("h3");
            contextHeader.InnerHtml = "Context:";
            contextGroup.AppendChild(contextHeader);

            var contextContent = doc.CreateElement("div");
            contextContent.SetAttributeValue("class", "emOne");
            contextContent.SetAttributeValue("id", "reportContext");
            contextContent.InnerHtml = GetContextContent(results);
            contextGroup.AppendChild(contextContent);

            var imgGroup = doc.CreateElement("div");
            imgGroup.SetAttributeValue("id", "image");
            metaFlex.AppendChild(imgGroup);

            var imageHeader = doc.CreateElement("h3");
            imageHeader.InnerHtml = "Image:";
            imgGroup.AppendChild(imageHeader);

            var imageContent = doc.CreateElement("img");
            imageContent.SetAttributeValue("class", "thumbnail");
            imageContent.SetAttributeValue("id", "screenshotThumbnail");
            imageContent.SetAttributeValue("alt", "A Screenshot of the page");
            imageContent.SetAttributeValue("width", "33%");
            imageContent.SetAttributeValue("height", "auto");
            imgGroup.AppendChild(imageContent);

            var countsGroup = doc.CreateElement("div");
            countsGroup.SetAttributeValue("id", "counts");
            metaFlex.AppendChild(countsGroup);

            var countsHeader = doc.CreateElement("h3");
            countsHeader.InnerHtml = "Counts:";
            countsGroup.AppendChild(countsHeader);

            var countsContent = doc.CreateElement("div");
            countsContent.SetAttributeValue("class", "emOne");
            var countsString = GetCountContent(violationCount, incompleteCount, passCount, inapplicableCount, requestedResults);

            countsContent.InnerHtml = countsString.ToString();
            countsGroup.AppendChild(countsContent);

            var resultsFlex = doc.CreateElement("div");
            resultsFlex.SetAttributeValue("id", "results");
            contentArea.AppendChild(resultsFlex);

            if (!string.IsNullOrEmpty(results.Error))
            {
                var errorHeader = doc.CreateElement("h2");
                errorHeader.InnerHtml = "SCAN ERRORS:";
                contentArea.AppendChild(errorHeader);

                var errorContent = doc.CreateElement("div");
                errorContent.SetAttributeValue("id", "ErrorMessage");
                errorContent.InnerHtml = HttpUtility.HtmlEncode(results.Error);
                contentArea.AppendChild(errorContent);
            }

            if (violationCount > 0 && requestedResults.HasFlag(ReportTypes.Violations))
            {
                GetReadableAxeResults(results.Violations, ResultType.Violations, doc, resultsFlex);
            }

            if (incompleteCount > 0 && requestedResults.HasFlag(ReportTypes.Incomplete))
            {
                GetReadableAxeResults(results.Incomplete, ResultType.Incomplete, doc, resultsFlex);
            }

            if (passCount > 0 && requestedResults.HasFlag(ReportTypes.Passes))
            {
                GetReadableAxeResults(results.Passes, ResultType.Passes, doc, resultsFlex);
            }

            if (inapplicableCount > 0 && requestedResults.HasFlag(ReportTypes.Inapplicable))
            {
                GetReadableAxeResults(results.Inapplicable, ResultType.Inapplicable, doc, resultsFlex);
            }


            var modal = doc.CreateElement("div");
            modal.SetAttributeValue("id", "modal");
            contentArea.AppendChild(modal);

            var modalClose = doc.CreateElement("div");
            modalClose.InnerHtml = "X";
            modalClose.SetAttributeValue("id", "modalclose");
            modal.AppendChild(modalClose);

            var modalImage = doc.CreateElement("img");
            modalImage.SetAttributeValue("id", "modalimage");
            modal.AppendChild(modalImage);


            doc.DocumentNode.SelectSingleNode("//script").InnerHtml = EmbeddedResourceProvider.ReadEmbeddedFile("htmlReporterElements.js");

            doc.Save(destination, Encoding.UTF8);
        }

        private static string GetDataImageString(ISearchContext context)
        {
            ITakesScreenshot newScreen = (ITakesScreenshot)context;
            return $"data:image/png;base64,{Convert.ToBase64String(newScreen.GetScreenshot().AsByteArray)}";
        }

        private static string GetCss(ISearchContext context)
        {
            return EmbeddedResourceProvider.ReadEmbeddedFile("htmlReporter.css").Replace("url('", $"url('{GetDataImageString(context)}");
        }

        private static string GetContextContent(AxeResult results)
        {
            var contextContent = new StringBuilder()
                .AppendLine($"Url: {results.Url}<br>")
                .AppendLine($"Orientation: {results.TestEnvironment.OrientationType}<br>")
                .AppendLine($"Size: {results.TestEnvironment.WindowWidth} x {results.TestEnvironment.WindowHeight}<br>")
                .AppendLine($"Time: {results.Timestamp}<br>")
                .AppendLine($"User agent: {results.TestEnvironment.UserAgent}<br>")
                .AppendLine($"Using: {results.TestEngineName} ({results.TestEngineVersion})")
                .ToString();
            return contextContent;
        }

        private static int GetCount(AxeResultItem[] results)
        {
            int count = 0;
            foreach (AxeResultItem item in results)
            {
                foreach (AxeResultNode node in item.Nodes)
                {
                    count++;
                }

                // Still add one if no targets are included
                if (item.Nodes.Length == 0)
                {
                    count++;
                }
            }
            return count;
        }

        private static string GetCountContent(int violationCount, int incompleteCount, int passCount, int inapplicableCount, ReportTypes requestedResults)
        {
            StringBuilder countString = new StringBuilder();

            if (requestedResults.HasFlag(ReportTypes.Violations))
            {
                countString.AppendLine($" Violation: {violationCount}<br>");
            }

            if (requestedResults.HasFlag(ReportTypes.Incomplete))
            {
                countString.AppendLine($" Incomplete: {incompleteCount}<br>");
            }

            if (requestedResults.HasFlag(ReportTypes.Passes))
            {
                countString.AppendLine($" Pass: {passCount}<br>");
            }

            if (requestedResults.HasFlag(ReportTypes.Inapplicable))
            {
                countString.AppendLine($" Inapplicable: {inapplicableCount}");
            }

            return countString.ToString();
        }

        private static void GetReadableAxeResults(AxeResultItem[] results, ResultType type, HtmlDocument doc, HtmlNode body)
        {
            var resultWrapper = doc.CreateElement("div");
            resultWrapper.SetAttributeValue("class", "resultWrapper");
            body.AppendChild(resultWrapper);

            var sectionButton = doc.CreateElement("button");
            sectionButton.SetAttributeValue("class", "sectionbutton active");
            resultWrapper.AppendChild(sectionButton);

            var sectionButtonHeader = doc.CreateElement("h2");
            sectionButtonHeader.SetAttributeValue("class", "buttonInfoText");
            sectionButtonHeader.InnerHtml = $"{type}: {GetCount(results)}";
            sectionButton.AppendChild(sectionButtonHeader);

            var sectionButtonExpando = doc.CreateElement("h2");
            sectionButtonExpando.SetAttributeValue("class", "buttonExpandoText");
            sectionButtonExpando.InnerHtml = "-";
            sectionButton.AppendChild(sectionButtonExpando);

            var section = doc.CreateElement("div");
            section.SetAttributeValue("class", "majorSection");
            section.SetAttributeValue("id", type + "Section");
            resultWrapper.AppendChild(section);

            var loops = 1;

            foreach (var element in results)
            {
                var childEl = doc.CreateElement("div");
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

                    htmlAndSelector = doc.CreateTextNode("Selector:");
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

                    AddFixes(item, type, doc, htmlAndSelectorWrapper);
                }
            }
        }

        private static void AddFixes(AxeResultNode resultsNode, ResultType type, HtmlDocument doc, HtmlNode htmlAndSelectorWrapper)
        {
            HtmlNode htmlAndSelector;

            var anyCheckResults = resultsNode.Any;
            var allCheckResults = resultsNode.All;
            var noneCheckResults = resultsNode.None;

            int checkResultsCount = anyCheckResults.Length + allCheckResults.Length + noneCheckResults.Length;

            // Add fixes if this is for violations
            if (ResultType.Violations.Equals(type) && checkResultsCount > 0)
            {
                htmlAndSelector = doc.CreateTextNode("To solve:");
                htmlAndSelectorWrapper.AppendChild(htmlAndSelector);

                htmlAndSelector = doc.CreateElement("p");
                htmlAndSelector.SetAttributeValue("class", "wrapTwo");
                htmlAndSelectorWrapper.AppendChild(htmlAndSelector);

                if (allCheckResults.Length > 0 || noneCheckResults.Length > 0)
                {
                    FixAllIssues(doc, htmlAndSelectorWrapper, allCheckResults, noneCheckResults);
                }

                if (anyCheckResults.Length > 0)
                {
                    FixAnyIssues(doc, htmlAndSelectorWrapper, anyCheckResults);
                }
            }
        }

        private static void FixAllIssues(HtmlDocument doc, HtmlNode htmlAndSelectorWrapper, AxeResultCheck[] allCheckResults, AxeResultCheck[] noneCheckResults)
        {
            HtmlNode htmlAndSelector;

            htmlAndSelector = doc.CreateElement("p");
            htmlAndSelector.SetAttributeValue("class", "wrapOne");
            StringBuilder content = new StringBuilder();

            content.AppendLine("Fix all of the following issues:");
            content.AppendLine("<ul>");

            foreach (var checkResult in allCheckResults)
            {
                content.AppendLine($"<li>{HttpUtility.HtmlEncode(checkResult.Impact.ToUpper())}: {HttpUtility.HtmlEncode(checkResult.Message)}</li>");
            }

            foreach (var checkResult in noneCheckResults)
            {
                content.AppendLine($"<li>{HttpUtility.HtmlEncode(checkResult.Impact.ToUpper())}: {HttpUtility.HtmlEncode(checkResult.Message)}</li>");
            }

            content.AppendLine("</ul>");
            htmlAndSelector.InnerHtml = content.ToString();
            htmlAndSelectorWrapper.AppendChild(htmlAndSelector);
        }

        private static void FixAnyIssues(HtmlDocument doc, HtmlNode htmlAndSelectorWrapper, AxeResultCheck[] anyCheckResults)
        {
            StringBuilder content = new StringBuilder();

            HtmlNode htmlAndSelector = doc.CreateElement("p");
            htmlAndSelector.SetAttributeValue("class", "wrapOne");
            content.AppendLine("Fix at least one of the following issues:");
            content.AppendLine("<ul>");

            foreach (var checkResult in anyCheckResults)
            {
                content.AppendLine($"<li>{HttpUtility.HtmlEncode(checkResult.Impact.ToUpper())}: {HttpUtility.HtmlEncode(checkResult.Message)}</li>");
            }

            content.AppendLine("</ul>");
            htmlAndSelector.InnerHtml = content.ToString();
            htmlAndSelectorWrapper.AppendChild(htmlAndSelector);
        }
    }
}