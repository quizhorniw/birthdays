using HtmlAgilityPack;

namespace Birthdays.Api.Services;

public static class EmailHtmlParser
{
    public static void Parse(IEnumerable<string> birthdays, string templatePath, string resultPath)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(File.ReadAllText(templatePath));
        
        var ulNode = doc.DocumentNode.SelectSingleNode("//ul[@id='main-content']");
        birthdays.ToList().ForEach(b => ulNode.AppendChild(HtmlNode.CreateNode($"<li>{b}</li>")));
        
        doc.Save(resultPath);
    }
}