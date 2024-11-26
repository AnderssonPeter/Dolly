using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolly.Exmaple;

[Clonable]
public partial class ConstructorClass(string onlyCtorCanSet)
{

    public string OnlyCtorCanSet { get; } = onlyCtorCanSet;
    public string AnyOneCanSet { get; set; }
}
