namespace CalculateurAge.Services;

public interface INavigationService
{
	Task VersResultatAsync(string nom, int age, string statut,
						   string prochainAnniversaire);
	Task RetourAsync();
}
