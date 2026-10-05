using CalculateurAge.Services;
using CalculateurAge.ViewModels;

namespace CalculateurAge.Views;

[QueryProperty(nameof(Nom), "nom")]
[QueryProperty(nameof(Age), "age")]
[QueryProperty(nameof(Statut), "statut")]
[QueryProperty(nameof(Anniversaire), "anniversaire")]
public partial class ResultatPage : ContentPage
{
	private readonly ResultatViewModel _viewModel;

	public ResultatPage() : this(new ShellNavigationService())
	{
	}

	public ResultatPage(INavigationService navigation)
	{
		InitializeComponent();
		_viewModel = new ResultatViewModel(navigation);
		BindingContext = _viewModel;
	}

	// Ces proprietes sont remplies par la navigation,
	// APRES le constructeur.
	public string Nom
	{
		get => _viewModel.Nom;
		set => _viewModel.Nom = value;
	}

	public string Age
	{
		get => _viewModel.Age;
		set => _viewModel.Age = value;
	}

	public string Statut
	{
		get => _viewModel.Statut;
		set => _viewModel.Statut = value;
	}

	public string Anniversaire
	{
		get => _viewModel.Anniversaire;
		set => _viewModel.Anniversaire = value;
	}
}
