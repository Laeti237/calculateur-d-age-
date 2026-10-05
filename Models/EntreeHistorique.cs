namespace CalculateurAge.Models;

public class EntreeHistorique
{
	public string Nom { get; init; } = "";
	public DateTime DateNaissance { get; init; }
	public int Age { get; init; }
	public string Statut { get; init; } = "";
	public DateTime DateCalcul { get; init; } = DateTime.Now;

	public string Libelle =>
		$"{Nom} - {Age} ans ({Statut}) "
		+ $"| ne(e) le {DateNaissance:dd/MM/yyyy} "
		+ $"| calcule le {DateCalcul:HH:mm}";
}
