namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

/// <summary>
///     In Comparing
/// </summary>
internal class TextOutputGenerator //: ITextOutputGenerator
{
    private static readonly string s_znakNadpisu = "*";

    // při převádění na nugety jsem to změnil na ITextBuilder stringBuilder = TextBuilder.Create();
    // ale asi to byla blbost, teď mám v _sunamo Create() která je ale null místo abych použil ctor
    // takže vracím nazpět.
    //internal TextBuilder stringBuilder = new TextBuilder();
    internal StringBuilder stringBuilder = new();

    //internal string prependEveryNoWhite
    //{
    //    get => stringBuilder.prependEveryNoWhite;
    //    set => stringBuilder.prependEveryNoWhite = value;
    //}

    public override string ToString()
    {
        var ts = stringBuilder.ToString();
        return ts;
    }

    internal void Undo()
    {
        ThrowEx.NotImplementedMethod();
        //stringBuilder.Undo();
    }




    #region Other adding methods



    #endregion

    #region List









    #endregion

    #region Dictionary







    #endregion
}