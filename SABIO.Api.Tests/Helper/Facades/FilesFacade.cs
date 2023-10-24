using System;
using SABIO.ClientApi.Core;

namespace SABIO.Api.Tests.Helper.Facades;

public class FilesFacade : FacadeBase
{
    public const string TestTxt = "test.txt";
    public const string WildRiceRecipes = "WildRiceRecipes.docx";
    private readonly Lazy<byte[]> _testTxtData;
    private readonly Lazy<byte[]> _wildRiceRecipesData;

    public FilesFacade(SabioClient c) : base(c)
    {
        _testTxtData = new Lazy<byte[]>(() => ReadTestFile(TestTxt));
        _wildRiceRecipesData = new Lazy<byte[]>(() => ReadTestFile(WildRiceRecipes));
    }

    public byte[] TestTxtData => _testTxtData.Value;
    public byte[] WildRiceRecipesData => _wildRiceRecipesData.Value;
    public string TestFilePath(string fileName) => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", fileName);
    public byte[] ReadTestFile(string fileName) => System.IO.File.ReadAllBytes(TestFilePath(fileName));
}
