

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ImageLoader.Commands;

namespace ImageLoader.ViewModel;

public class StopViewModel : INotifyPropertyChanged
{
    public ICommand StopImageCommand { get; }
    private CancellationTokenSource[] _cancellationTokens = new CancellationTokenSource[3];
    private bool[] _isLoading = new bool[3];

    public CancellationTokenSource[] CancellationTokens
    {
        get => _cancellationTokens;
        set
        {
            _cancellationTokens = value;
            OnPropertyChanged(nameof(CancellationTokens));
        }
    }
    
    public  bool[] IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public StopViewModel()
    {
        StopImageCommand = new RelayCommand(Stop, CanStop);
    }
    
    
    private Task Stop(object imageConverter)
    {
        if (imageConverter is ImageConverter ic && ic.Index is int index)
        {
            _cancellationTokens[index].Cancel();
        }
        return Task.CompletedTask;
    }

    private bool CanStop(object imageConverter)
    {
        if (imageConverter is ImageConverter ic && ic.Index is int index)
        {
            return _isLoading[index];
        }

        return false;
    }

    
    
    // Реализация интерфейса INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}