﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMS.Entities.Models.PageBuilder;

namespace WebCMS.Services.Shared
{
    public class Document
    {
        private HtmlDocument _document { get; set; }

        public string OuterHtml
        {
            get
            {
                return _document.DocumentNode.OuterHtml;
            }
        }

        public Document(string htmlBody)
        {
            _document = new HtmlDocument();

            _document.LoadHtml(htmlBody);
        }

        public void AddElement(string containerElementId, string componentBody)
        {
            var newNode = HtmlNode.CreateNode(componentBody);

            var containerElement = _document.GetElementbyId(containerElementId);

            containerElement.AppendChild(newNode);
        }

        public void UpdateElementContent(string elementId, string elementValue)
        {
            var element = _document.GetElementbyId(elementId);

            element.InnerHtml = elementValue;
        }

        public void UpdateElementAttribute(string elementId, string attributeName, string attributeValue, bool replaceValue)
        {
            var element = _document.GetElementbyId(elementId);

            if (replaceValue)
            {
                element.SetAttributeValue(attributeName, attributeValue);
            }
            else
            {
                var existingAttribute = element.Attributes.SingleOrDefault(x => x.Name == attributeName);
                var existingValue = string.Empty;

                if (existingValue != null)
                    existingValue = existingAttribute.Value;

                element.SetAttributeValue(attributeName, existingValue + attributeValue);
            }
        }

        public void UpdateSectionHeight(string elementId, PageSectionHeight height)
        {
            var element = _document.GetElementbyId(elementId);

            var selectedHeight = string.Format("height-{0}", height).ToLower();

            var heightClasses = new List<string> { "height-tall", "height-medium", "height-small", "height-tiny", "height-standard" };

            var classAttribute = element.Attributes.SingleOrDefault(x => x.Name == "class");

            foreach (var heightClass in heightClasses)
                classAttribute.Value = classAttribute.Value.Replace(heightClass, selectedHeight);
        }

        public void UpdateBackgroundType(string elementId, PageSectionBackgroundType backgroundType)
        {
            var element = _document.GetElementbyId(elementId);

            var selectedHeight = string.Format("background-{0}", backgroundType).ToLower();

            var heightClasses = new List<string> { "background-static", "background-parallax" };

            var classAttribute = element.Attributes.SingleOrDefault(x => x.Name == "class");

            foreach (var heightClass in heightClasses)
                classAttribute.Value = classAttribute.Value.Replace(heightClass, selectedHeight);
        }

        public void DeleteElement(string elementId)
        {
            var element = _document.GetElementbyId(elementId);

            element.Remove();
        }

        public static string ReplaceTokens(string htmlBody, int pageSectionId)
        {
            htmlBody = htmlBody.Replace("<componentStamp>", DateTime.Now.ToString("ddMMyyHHmmss"));
            htmlBody = htmlBody.Replace("<sectionId>", pageSectionId.ToString());

            return htmlBody;
        }
    }
}
