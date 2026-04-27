namespace LittleLearner.CFG.Modal;

public partial class Konfiguration : ContentPage
{
    public bool confirmChanges;

	public Konfiguration() { InitializeComponent(); }

    public async void CancleConfiguratoin(object? sender, EventArgs arguments) { confirmChanges = false;  await Navigation.PopModalAsync(); }
    public async void ConfirmConfiguration(object? sender, EventArgs arguments) { confirmChanges = true; await Navigation.PopModalAsync(); }

}