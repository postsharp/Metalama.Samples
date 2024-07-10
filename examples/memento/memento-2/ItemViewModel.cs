using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample;

[Memento]
public partial class ItemViewModel : ObservableRecipient
{
    private string? _name;
    private string? _species;
    private DateTime _dateAdded;

    public string? Name { get => this._name; set => this.SetProperty( this._name, value, v => this._name = v, true ); }

    public string? Species { get => this._species; set => this.SetProperty( this._species, value, v => this._species = v, true ); }

    public DateTime DateAdded { get => this._dateAdded; set => this.SetProperty( this._dateAdded, value, v => this._dateAdded = v, true ); }
}
