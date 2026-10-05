using CalculateurAge.Views;

namespace CalculateurAge.Services;

public class ShellNavigationService : INavigationService
{
	public async Task VersResultatAsync(string nom, int age,
										string statut,
										string prochainAnniversaire)
	{
		string url = $"{nameof(ResultatPage)}"
					 + $"?nom={Uri.EscapeDataString(nom)}"
					 + $"&age={age}"
					 + $"&statut={Uri.EscapeDataString(statut)}"
					 + $"&anniversaire="
					 + Uri.EscapeDataString(prochainAnniversaire);

		await Shell.Current.GoToAsync(url);
	}

	public Task RetourAsync() => Shell.Current.GoToAsync("..");
}
