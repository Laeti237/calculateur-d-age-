using CalculateurAge.Services;
using CalculateurAge.ViewModels;

namespace CalculateurAge;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		// Objet dans lequel tous les {Binding} de la page
		// vont chercher leurs valeurs. Le service de navigation
		// est fourni au ViewModel : la vue ignore ou l'on va.
		BindingContext = new CalculateurViewModel(
			new ShellNavigationService());
	}
}
