namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

/// <summary>
///     In Comparing
/// </summary>
internal class TextOutputGenerator //: ITextOutputGenerator
{
    private static readonly string s_znakNadpisu = "*";

    // při převádění na nugety jsem to změnil na ITextBuilder sb = TextBuilder.Create();
    // ale asi to byla blbost, teď mám v _sunamo Create() která je ale null místo abych použil ctor
    // takže vracím nazpět.
    //internal TextBuilder sb = new TextBuilder();
    internal StringBuilder sb = new();

    //internal string prependEveryNoWhite
    //{
    //    get => sb.prependEveryNoWhite;
    //    set => sb.prependEveryNoWhite = value;
    //}

    public override string ToString()
    {
        var ts = sb.ToString();
        return ts;
    }

    internal void Undo()
    {
        ThrowEx.NotImplementedMethod();
        //sb.Undo();
    }




    #region Other adding methods



    #endregion

    #region List









    #endregion

    #region Dictionary







    #endregion
}
