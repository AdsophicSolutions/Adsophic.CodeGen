using System;
using System.Collections.Generic;
using System.Text;

namespace Adsophic.CodeGen.API
{
    public interface ICodeFormatter
    {
        string Format(string unformatted);
    }
}
