using System;

namespace SABIO.Api.Tests.Helper;

public class FilesFacade
{
    public const string TestTxt = "test.txt";
    public const string WildRiceRecipes = "WildRiceRecipes.docx";
    private readonly Lazy<byte[]> _testTxtData;
    private readonly Lazy<byte[]> _wildRiceRecipesData;

    public FilesFacade()
    {
        _testTxtData = new Lazy<byte[]>(() => ReadTestFile(TestTxt));
        _wildRiceRecipesData = new Lazy<byte[]>(() => ReadTestFile(WildRiceRecipes));
    }

    public byte[] TestTxtData => _testTxtData.Value;
    public byte[] WildRiceRecipesData => _wildRiceRecipesData.Value;
    public string TestFilePath(string fileName) => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", fileName);
    public byte[] ReadTestFile(string fileName) => System.IO.File.ReadAllBytes(TestFilePath(fileName));
}
