using System.Windows.Input;

namespace CalculateurAge.ViewModels;

public class AsyncRelayCommand : ICommand
{
	private readonly Func<Task> _executer;
	private readonly Func<bool>? _peutExecuter;

	public AsyncRelayCommand(Func<Task> executer,
							 Func<bool>? peutExecuter = null)
	{
		_executer = executer;
		_peutExecuter = peutExecuter;
	}

	public bool CanExecute(object? p)
		=> _peutExecuter?.Invoke() ?? true;

	public async void Execute(object? p) => await _executer();

	public event EventHandler? CanExecuteChanged;

	public void Rafraichir()
		=> CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
