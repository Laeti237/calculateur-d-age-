using CalculateurAge.Views;

namespace CalculateurAge.Services;

// Implementation Shell : la seule classe de l'application
// qui connait le routing et la construction de l'URL.
public class ShellNavigationService : INavigationService
{
	public async Task VersResultatAsync(string nom, int age,
										string statut,
										string prochainAnniversaire)
	{
		// Le ViewModel fournit des valeurs typiques, la
		// lecture des parametres reste faite par Shell.
		string url = $"{nameof(ResultatPage)}"
					 + $"?nom={Uri.EscapeDataString(nom)}"
					 + $"&age={age}"
					 + $"&statut={Uri.EscapeDataString(statut)}"
					 + $"&anniversaire="
					 + Uri.EscapeDataString(prochainAnniversaire);

		await Shell.Current.GoToAsync(url);
	}

	// ".." = revenir a la page precedente.
	public Task RetourAsync() => Shell.Current.GoToAsync("..");
}
