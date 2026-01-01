namespace SunamoFileSystem._sunamo.SunamoStringSplit;

internal class SHSplit
{
    internal static List<string> Split(string text, params string[] delimiters)
    {
        return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
    }


    #region SplitToPartsFromEnd

    internal static List<string> SplitToPartsFromEnd(string text, int parts, params char[] delimiters)
    {
        List<char> chars = null;
        List<bool> isNotDelimiter = null;
        List<int> delimitersIndexes = null;
        SplitCustom(text, out chars, out isNotDelimiter, out delimitersIndexes, delimiters);

        var partsList = new List<string>(parts);
        var stringBuilder = new StringBuilder();
        for (var i = chars.Count - 1; i >= 0; i--)
            if (!isNotDelimiter[i])
            {
                while (i != 0 && !isNotDelimiter[i - 1]) i--;
                var part = stringBuilder.ToString();
                stringBuilder.Clear();
                if (part != "") partsList.Add(part);
            }
            else
            {
                stringBuilder.Insert(0, chars[i]);
                //stringBuilder.Append(chars[i]);
            }

        var remainingText = stringBuilder.ToString();
        stringBuilder.Clear();
        if (remainingText != "") partsList.Add(remainingText);
        var finalParts = new List<string>(parts);
        for (var i = 0; i < partsList.Count; i++)
            if (finalParts.Count != parts)
            {
                finalParts.Insert(0, partsList[i]);
            }
            else
            {
                var delimiterString = text[delimitersIndexes[i - 1]].ToString();
                finalParts[0] = partsList[i] + delimiterString + finalParts[0];
            }

        return finalParts;
    }

    internal static void SplitCustom(string text, out List<char> chars, out List<bool> isNotDelimiter,
        out List<int> delimitersIndexes, params char[] delimiters)
    {
        chars = new List<char>(text.Length);
        isNotDelimiter = new List<bool>(text.Length);
        delimitersIndexes = new List<int>(text.Length / 6);
        for (var i = 0; i < text.Length; i++)
        {
            var charIsNotDelimiter = true;
            var currentChar = text[i];
            foreach (var delimiter in delimiters)
                if (delimiter == currentChar)
                {
                    delimitersIndexes.Add(i);
                    charIsNotDelimiter = false;
                    break;
                }

            chars.Add(currentChar);
            isNotDelimiter.Add(charIsNotDelimiter);
        }

        delimitersIndexes.Reverse();
    }

    #endregion
}