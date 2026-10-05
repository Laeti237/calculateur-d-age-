using System.Collections.ObjectModel;
using CalculateurAge.Models;
using CalculateurAge.Services;

namespace CalculateurAge.ViewModels;

// Contient l'ETAT de l'ecran et les ACTIONS possibles.
// Aucun Label, Entry, Button ou DisplayAlert ici :
// ce fichier pourrait etre compile dans une application console.
public class CalculateurViewModel : BaseViewModel
{
	private static readonly DateTime DateParDefaut
		= DateTime.Today.AddYears(-20);

	// Champs prives : la vraie donnee.
	private readonly INavigationService? _navigation;
	private string _nom = "";
	private DateTime _dateNaissance = DateParDefaut;
	private int _age;
	private string _resultat = "";
	private bool _resultatVisible;
	private string _statut = "";
	private string _prochainAnniversaire = "";
	private string _erreur = "";
	private bool _erreurVisible;

	// Proprietes publiques : ce que le XAML voit.
	public string Nom
	{
		get => _nom;
		set
		{
			if (SetField(ref _nom, value))
			{
				CalculerCommand.Rafraichir();
				EffacerCommand.Rafraichir();
			}
		}
	}

	public DateTime DateNaissance
	{
		get => _dateNaissance;
		set
		{
			if (SetField(ref _dateNaissance, value))
			{
				// Une nouvelle date efface l'erreur precedente.
				Erreur = "";
				ErreurVisible = false;
				EffacerCommand.Rafraichir();
			}
		}
	}

	public string Resultat
	{
		get => _resultat;
		set => SetField(ref _resultat, value);
	}

	public int Age
	{
		get => _age;
		set => SetField(ref _age, value);
	}

	public bool ResultatVisible
	{
		get => _resultatVisible;
		set
		{
			if (SetField(ref _resultatVisible, value))
				AfficherResultatCommand?.Rafraichir();
		}
	}

	// "Majeur" ou "Mineur", calcule apres l'age.
	public string Statut
	{
		get => _statut;
		set => SetField(ref _statut, value);
	}

	// Jours restants avant le prochain anniversaire.
	public string ProchainAnniversaire
	{
		get => _prochainAnniversaire;
		set => SetField(ref _prochainAnniversaire, value);
	}

	// Message d'erreur affiche en rouge sous le formulaire.
	public string Erreur
	{
		get => _erreur;
		set => SetField(ref _erreur, value);
	}

	public bool ErreurVisible
	{
		get => _erreurVisible;
		set => SetField(ref _erreurVisible, value);
	}

	// Lie a Button.Command dans le XAML.
	public RelayCommand CalculerCommand { get; }
	public RelayCommand EffacerCommand { get; }
	public RelayCommand EffacerHistoriqueCommand { get; }

	// Ouvre ResultatPage en lui transmettant l'etat courant :
	// le ViewModel decide, le service de navigation sait comment.
	public AsyncRelayCommand AfficherResultatCommand { get; }

	// Historique des calculs : observe par le CollectionView
	// via l'interface INotifyCollectionChanged.
	public ObservableCollection<EntreeHistorique> Historique
		{ get; } = new();

	public string CompteHistorique =>
		Historique.Count == 0
			? "Aucun calcul"
			: $"{Historique.Count} calcul(s)";

	public bool HistoriqueVisible => Historique.Count > 0;

	public CalculateurViewModel() : this(null)
	{
	}

	public CalculateurViewModel(INavigationService? navigation)
	{
		_navigation = navigation;

		CalculerCommand = new RelayCommand(
			Calculer,
			() => !string.IsNullOrWhiteSpace(Nom));

		EffacerCommand = new RelayCommand(
			Effacer,
			() => ResultatVisible
				  || !string.IsNullOrWhiteSpace(Nom));

		EffacerHistoriqueCommand = new RelayCommand(
			EffacerHistorique,
			() => Historique.Count > 0);

		AfficherResultatCommand = new AsyncRelayCommand(
			AfficherResultatAsync,
			() => ResultatVisible
				  && _navigation is not null);

		// Chaque ajout/vidage notifie les proprietes derivees.
		Historique.CollectionChanged += (_, _) =>
		{
			OnPropertyChanged(nameof(CompteHistorique));
			OnPropertyChanged(nameof(HistoriqueVisible));
			EffacerHistoriqueCommand.Rafraichir();
		};
	}

	// La logique metier : aucun controle d'interface ici.
	private void Calculer()
	{
		// Refus d'une date future.
		if (DateNaissance.Date > DateTime.Today)
		{
			AfficherErreur("La date de naissance ne peut pas "
						   + "etre dans le futur.");
			return;
		}

		int age = CalculerAge(DateNaissance);

		Age = age;
		Resultat = $"{Nom}, vous avez {Age} ans";
		Statut = age >= 18 ? "Majeur" : "Mineur";
		ProchainAnniversaire =
			LibelleProchainAnniversaire(DateNaissance);
		ResultatVisible = true;

		// Le plus recent en premiere ligne.
		Historique.Insert(0, new EntreeHistorique
		{
			Nom = Nom.Trim(),
			DateNaissance = DateNaissance.Date,
			Age = age,
			Statut = Statut
		});

		Erreur = "";
		ErreurVisible = false;
		EffacerCommand.Rafraichir();
	}

	// Remet tous les champs a zero.
	private void Effacer()
	{
		Nom = "";
		DateNaissance = DateParDefaut;
		Age = 0;
		Resultat = "";
		ResultatVisible = false;
		Statut = "";
		ProchainAnniversaire = "";
		Erreur = "";
		ErreurVisible = false;
		CalculerCommand.Rafraichir();
		EffacerCommand.Rafraichir();
	}

	// Vide l'historique sans toucher aux champs du formulaire.
	private void EffacerHistorique() => Historique.Clear();

	// Transmet l'etat courant a la page de resultat.
	private Task AfficherResultatAsync()
		=> _navigation is null
			? Task.CompletedTask
			: _navigation.VersResultatAsync(
				string.IsNullOrWhiteSpace(Nom) ? Nom : Nom.Trim(),
				Age,
				Statut,
				ProchainAnniversaire);

	private void AfficherErreur(string message)
	{
		Erreur = message;
		ErreurVisible = true;
		ResultatVisible = false;
	}

	// Age exact : on retire une annee si l'anniversaire
	// n'est pas encore passe cette annee.
	public static int CalculerAge(DateTime dateNaissance)
	{
		int age = DateTime.Today.Year - dateNaissance.Year;
		if (dateNaissance.Date > DateTime.Today.AddYears(-age))
			age--;
		return age;
	}

	// "12 jour(s) avant votre anniversaire (17/10/2026)"
	public static string LibelleProchainAnniversaire(
		DateTime dateNaissance)
	{
		DateTime today = DateTime.Today;
		DateTime prochain =
			dateNaissance.AddYears(today.Year - dateNaissance.Year);
		if (prochain.Date < today)
			prochain = prochain.AddYears(1);

		int jours = (prochain.Date - today).Days;

		return jours == 0
			? "C'est votre anniversaire aujourd'hui !"
			: $"{jours} jour(s) avant votre anniversaire "
			  + $"({prochain:dd/MM/yyyy})";
	}
}
