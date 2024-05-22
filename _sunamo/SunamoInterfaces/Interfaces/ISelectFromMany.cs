namespace SunamoFileSystem;


internal interface ISelectFromMany<Data>
{
    void AddControl(Data data, bool b);
    void AddControls();
}