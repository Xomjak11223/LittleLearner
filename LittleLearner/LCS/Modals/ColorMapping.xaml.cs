namespace LittleLearner.LCS.Modals;

public partial class ColorMapping : ContentPage
{
	public ColorMapping() { InitializeComponent(); }

	private async void CloseModal(object? sender, EventArgs arguments) { await Navigation.PopModalAsync(); }
}