using System.Text;

namespace DevFuns
{
    public class GeneratorRepositoryTemplate : IDisposable
    {
        public enum ReplaceTypes
        {
            Class, Interface
        }

        public enum GenerateNamespaceTypes
        {
            Minimal, Full
        }

        private const string CommandFolderName = "Commands";
        private const string QueryFolderName = "Queries";
        private const string PrefixClassName = "@{ClassName}";
        private const string PrefixInterfaceName = "@{InterfaceName}";
        private const string EndsWithName = "Repository";
        private const string CSharpExntesion = ".cs";

        public GeneratorRepositoryTemplate(string template, GenerateNamespaceTypes namespaceTypes)
            : this(new StringBuilder(template), namespaceTypes) { }

        public GeneratorRepositoryTemplate(StringBuilder builder, GenerateNamespaceTypes namespaceTypes) : this()
        {
            ParamBuilder = new StringBuilder(builder.ToString());
            NamespaceTypes = namespaceTypes;
        }

        private GeneratorRepositoryTemplate()
        {
            Builder = new StringBuilder();
            Values = new Dictionary<string, string>();
            UsingValues = new List<string>();
        }

        public Dictionary<string, string> Values { get; private set; }
        public IList<string> UsingValues { get; private set; }
        public GenerateNamespaceTypes NamespaceTypes { get; }

        protected string Namespace { get; private set; }
        protected StringBuilder Builder { get; }
        protected StringBuilder ParamBuilder { get; }

        public void Add(ReplaceTypes type, string value)
        {
            switch (type)
            {
                case ReplaceTypes.Class:
                    Values.Add(PrefixClassName, value);
                    break;
                case ReplaceTypes.Interface:
                    Values.Add(PrefixInterfaceName, value);
                    break;
                default: break;
            }
        }

        public void AddUsing(string value)
            => UsingValues.Add(value);

        public void AddUsing(StringBuilder builder)
            => UsingValues.Add(builder.ToString());

        public void SetNamespace(string value) => Namespace = value;

        public void Write(string folderPath)
        {
            CreateIfExistFolders(folderPath);

            string name = $"{Values.FirstOrDefault(w => w.Key == PrefixClassName).Value}{EndsWithName}{CSharpExntesion}";
            string path = Path.Combine(folderPath, name);

            BuildUsing();
            BuildNamespace();
            BuildValues();
            File.WriteAllText(path, Builder.ToString());
        }

        protected internal void BuildUsing()
        {
            foreach (var item in UsingValues)
            {
                Builder.AppendLine(item);
            }
        }

        protected internal void BuildNamespace()
            => Builder.AppendLine($"namespace {Namespace}");

        protected internal void BuildValues()
        {
            foreach (var item in Values)
            {
                ParamBuilder.Replace(item.Key, item.Value);
            }
            string format = NamespaceTypes == GenerateNamespaceTypes.Minimal ? "{0}" : "{{ {0} }}";
            Builder.AppendFormat(format, ParamBuilder.ToString());
        }

        protected internal void CreateIfExistFolders(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        public void Dispose()
        {
            Builder?.Clear();
            ParamBuilder?.Clear();
            Values?.Clear();
            UsingValues?.Clear();
        }
    }
}