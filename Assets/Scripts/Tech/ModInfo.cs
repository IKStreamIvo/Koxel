using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koxel.Modding
{
    public class ModInfo
    {
        public string name;
        public string version;
        public Author[] authors;

        public class Author
        {
            public string name;
            public string title;
            public string link;
        }
    }
}
