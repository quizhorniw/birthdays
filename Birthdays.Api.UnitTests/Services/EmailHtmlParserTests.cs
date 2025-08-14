using Birthdays.Api.Services;
using HtmlAgilityPack;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Services;

[TestFixture]
public class EmailHtmlParserTests
{
    private string _templateFilePath;
    private string _resultFilePath;

    [SetUp]
    public void Setup()
    {
        _templateFilePath = Path.Combine(Path.GetTempPath(), "template.html");
        _resultFilePath = Path.Combine(Path.GetTempPath(), "result.html");

        File.WriteAllText(_templateFilePath, "<html><body><ul id=\"main-content\"></ul></body></html>");
    }
    
    [Test]
    public void Parse_ValidBirthdays_ShouldAddItemsToUl()
    {
        // Arrange
        var birthdays = new List<string>
        {
            "Марина Сергеева - 10 января",
            "Екатерина Иванова - 21 ноября"
        };

        // Act
        EmailHtmlParser.Parse(birthdays, _templateFilePath, _resultFilePath);

        var resultDoc = new HtmlDocument();
        resultDoc.LoadHtml(File.ReadAllText(_resultFilePath));

        // Assert
        var ulNode = resultDoc.DocumentNode.SelectSingleNode("//ul[@id='main-content']");
        That(ulNode, Is.Not.Null);
        That(ulNode.ChildNodes, Has.Count.EqualTo(2));
        That(ulNode.ChildNodes[0].InnerText.Trim(), Is.EqualTo("Марина Сергеева - 10 января"));
        That(ulNode.ChildNodes[1].InnerText.Trim(), Is.EqualTo("Екатерина Иванова - 21 ноября"));
    }

    [Test]
    public void Parse_EmptyBirthdays_ShouldNotAddAnyItems()
    {
        // Arrange
        var birthdays = new List<string>();

        // Act
        EmailHtmlParser.Parse(birthdays, _templateFilePath, _resultFilePath);

        var resultDoc = new HtmlDocument();
        resultDoc.LoadHtml(File.ReadAllText(_resultFilePath));

        // Assert
        var ulNode = resultDoc.DocumentNode.SelectSingleNode("//ul[@id='main-content']");
        That(ulNode, Is.Not.Null);
        That(ulNode.ChildNodes, Is.Empty);
    }

    [Test]
    public void Parse_MissingUlNode_ShouldThrowException()
    {
        // Arrange
        File.WriteAllText(_templateFilePath, "<html><body></body></html>");

        // Act
        // Assert
        Throws<NullReferenceException>(() => EmailHtmlParser.Parse(new List<string> { "Сергей Миронов - 1 июля" }, _templateFilePath, _resultFilePath));
    }

    [Test]
    public void Parse_TemplateFileDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var nonExistentTemplatePath = Path.Combine(Path.GetTempPath(), "nonexistent.html");

        // Act
        // Assert
        Throws<FileNotFoundException>(() => EmailHtmlParser.Parse(new List<string> { "Дмитрий Носов - 2 июня" }, nonExistentTemplatePath, _resultFilePath));
    }
    
    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_templateFilePath))
            File.Delete(_templateFilePath);
        if (File.Exists(_resultFilePath))
            File.Delete(_resultFilePath);
    }
}
