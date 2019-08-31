using System.Collections.Generic;
using System.Text;

namespace Breakdawn.Core
{
    public struct PathBuilder
    {
        private StringBuilder _builder;

        public PathBuilder(string head, string fileName, params string[] paths) : this()
        {
            Init(head, fileName, paths);
        }

        public PathBuilder(string head, string fileName, ICollection<string> paths) : this()
        {
            Init(head, fileName, paths);
        }

        private void Init(string head, string fileName, ICollection<string> paths)
        {
            _builder = new StringBuilder(paths.Count + 2);
            _builder.Append(head);
            foreach (var path in paths)
            {
                _builder.Append(path).Append('/');
            }

            _builder.Append(fileName);
        }

        public string Get()
        {
            return _builder.ToString();
        }
    }
}