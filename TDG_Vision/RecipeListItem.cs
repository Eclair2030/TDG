using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDG_Vision
{
    internal class RecipeListItem
    {
        public RecipeListItem() { }

        public RecipeListItem(string code, string name, string use)
        {
            Code = code;
            Name = name;
            Inuse = use;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Inuse { get; set; }
    }
}
