using NUnit.Framework;
using System.IO;
using System.Reflection.Emit;
using lab7;

public class TreeBuilderTests
{
    [Test]
    public void BuildTree_PreservesNesting()
    {
        var tree = TreeBuilder.Build(File.ReadAllLines("tree.txt"));
        Assert.That(tree.Children[0].Children[0].Name, Is.EqualTo("C"));
    }
}