
namespace SunamoFileSystem._sunamo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


internal class SHReplace
{
    internal static string ReplaceOnce(string input, string what, string zaco)
    {
        return new Regex(what).Replace(input, zaco, 1);
    }

    internal static string ReplaceAllDoubleSpaceToSingle2(string text, bool alsoHtml = false)
    {
        if (alsoHtml)
        {
            text = text.Replace(" &nbsp;", " ");
            text = text.Replace("&nbsp; ", " ");
            text = text.Replace("&nbsp;", " ");
        }
        var p = text.Split(AllCharsSE.whiteSpacesChars.ToArray()); //SHSE.Split(text, AllCharsSE.whiteSpacesChars.ConvertAll(d => d.ToString()).ToArray());
        return string.Join(" ", p);
    }
}

