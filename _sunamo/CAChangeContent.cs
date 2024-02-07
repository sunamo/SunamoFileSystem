
namespace SunamoFileSystem._sunamo;
using System;
using System.Collections.Generic;


internal class CAChangeContent
{
    internal static List<string> ChangeContent0(dynamic /*ChangeContentArgs*/ a, List<string> files_in, Func<string, string> func)
    {
        for (int i = 0; i < files_in.Count; i++)
        {
            files_in[i] = func.Invoke(files_in[i]);
        }

        RemoveNullOrEmpty(a, files_in);

        return files_in;
    }

    private static void RemoveNullOrEmpty(dynamic /*ChangeContentArgs*/ a, List<string> files_in)
    {
        if (a != null)
        {
            if (a.removeNull)
            {
                files_in.Remove(null);
            }

            if (a.removeEmpty)
            {
                for (int i = files_in.Count - 1; i >= 0; i--)
                {
                    if (files_in[i].Trim() == string.Empty)
                    {
                        files_in.RemoveAt(i);
                    }
                }
            }

        }
    }
}

