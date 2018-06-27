using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koxel.Modding
{
    public interface IModComponent
    {
        string tag { get; }
        string display { get; }
    }
}
