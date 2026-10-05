using CalculateurAge.Views;

namespace CalculateurAge;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	// Gestionnaire appele au clic du bouton Calculer.
	// e = donnees de l'evenement.
	private async void OnCalculerClicked(object? sender, EventArgs e)
	{
		// Validation : on refuse un nom vide.
		if (string.IsNullOrWhiteSpace(entryNom.Text))
		{
			await DisplayAlertAsync("Erreur", "Entrez un nom", "OK");
			return;   // on sort sans rien calculer
		}

		DateTime d = pickerDate.Date ?? DateTime.Today;
		int age = DateTime.Today.Year - d.Year;
		// Si l'anniversaire n'est pas encore passe cette annee,
		// on retire une annee.
		if (d.Date > DateTime.Today.AddYears(-age)) age--;

		// Navigation vers la page de resultat avec les parametres
		// passes dans l'URL (separateur "?" puis "&").
		await Shell.Current.GoToAsync(
			$"{nameof(ResultatPage)}?nom={Uri.EscapeDataString(entryNom.Text)}&age={age}");
	}
}
