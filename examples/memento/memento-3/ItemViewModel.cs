using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample;

[Memento]
internal partial class ItemViewModel : ObservableRecipient
{
    private string _name;
    private string _species;
    private DateTime _dateAdded;

    public string Name { get => this._name; set => this.SetProperty( ref this._name, value, true ); }

    public string Species { get => this._species; set => this.SetProperty( ref this._species, value, true ); }

    public DateTime DateAdded { get => this._dateAdded; set => this.SetProperty( ref this._dateAdded, value, true ); }
}
