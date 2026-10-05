using CalculateurAge.Services;

namespace CalculateurAge.ViewModels;

public class ResultatViewModel : BaseViewModel
{
	private readonly INavigationService _navigation;

	private string _nom = "";
	private string _age = "";
	private string _statut = "";
	private string _anniversaire = "";
	private string _message = "";
	private string _detail = "";

	public string Nom
	{
		get => _nom;
		set { if (SetField(ref _nom, value)) Recomposer(); }
	}

	public string Age
	{
		get => _age;
		set { if (SetField(ref _age, value)) Recomposer(); }
	}

	public string Statut
	{
		get => _statut;
		set { if (SetField(ref _statut, value)) Recomposer(); }
	}

	public string Anniversaire
	{
		get => _anniversaire;
		set { if (SetField(ref _anniversaire, value)) Recomposer(); }
	}

	public string Message
	{
		get => _message;
		set => SetField(ref _message, value);
	}

	public string Detail
	{
		get => _detail;
		set => SetField(ref _detail, value);
	}

	public AsyncRelayCommand RetourCommand { get; }

	public ResultatViewModel(INavigationService navigation)
	{
		_navigation = navigation;
		RetourCommand = new AsyncRelayCommand(_navigation.RetourAsync);
	}

	private void Recomposer()
	{
		Message = string.IsNullOrWhiteSpace(Nom)
			? ""
			: $"{Nom}, vous avez {Age} ans";

		Detail = string.IsNullOrWhiteSpace(Statut)
			? ""
			: $"{Statut} - {Anniversaire}";
	}
}
