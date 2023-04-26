namespace DevFuns
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string template = @"public class @{ClassName}Repository : @{InterfaceName}Repository {}";
            string path = $"D:\\Repositories\\";

            var generate = new GeneratorRepositoryTemplate(template
                , GeneratorRepositoryTemplate.GenerateNamespaceTypes.Minimal);

            generate.SetNamespace("Simple.Generate.Home");
            generate.AddUsing("using System.Text;");
            generate.AddUsing($"using System.Text;{Environment.NewLine}using System.Text;");
            generate.Add(GeneratorRepositoryTemplate.ReplaceTypes.Class, "Example");
            generate.Add(GeneratorRepositoryTemplate.ReplaceTypes.Interface, "IExample");
            generate.Write(path);
        }
    }
}