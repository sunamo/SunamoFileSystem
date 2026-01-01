namespace SunamoFileSystem._sunamo.SunamoCollectionsChangeContent;

internal class CAChangeContent
{
    internal static List<string> ChangeContent0(dynamic /*ChangeContentArgs*/ args, List<string> items,
        Func<string, string> transformFunc)
    {
        for (var i = 0; i < items.Count; i++) items[i] = transformFunc.Invoke(items[i]);

        RemoveNullOrEmpty(args, items);

        return items;
    }

    private static void RemoveNullOrEmpty(dynamic /*ChangeContentArgs*/ args, List<string> items)
    {
        if (args != null)
        {
            if (args.removeNull) items.Remove(null);

            if (args.removeEmpty)
                for (var i = items.Count - 1; i >= 0; i--)
                    if (items[i].Trim() == string.Empty)
                        items.RemoveAt(i);
        }
    }
}