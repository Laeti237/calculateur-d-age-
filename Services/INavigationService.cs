namespace CalculateurAge.Services;

// Abstraction de la navigation : un ViewModel ne connait
// ni Shell, ni l'URL, ni la page de destination.
public interface INavigationService
{
	Task VersResultatAsync(string nom, int age, string statut,
						   string prochainAnniversaire);
	Task RetourAsync();
}
