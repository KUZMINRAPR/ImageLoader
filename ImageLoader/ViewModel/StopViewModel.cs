

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageLoader.Commands;

namespace ImageLoader.ViewModel;

public class StopViewModel : INotifyPropertyChanged
{
    public ICommand StopImageCommand { get; }
    public ICommand StopAllImageCommand { get;  }
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
        
        StopAllImageCommand = new RelayCommand(StopAll, CanStopAll);
    }
    
    
    private Task Stop(object imageConverter)
    {
        if (imageConverter is ImageConverter ic && ic.Index is int index)
        {
            _cancellationTokens[index].Cancel();
            _isLoading[index] = false;
            OnPropertyChanged(nameof(IsLoading));
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

    private Task StopAll(object images)
    {
        for (int i = 0; i < _cancellationTokens.Length; i++)
        {
            if (_cancellationTokens[i] != null && !_cancellationTokens[i].IsCancellationRequested)
            {
                _cancellationTokens[i].Cancel();
                _isLoading[i] = false;
            }
        }
        OnPropertyChanged(nameof(IsLoading));
        Console.WriteLine("Loading of all images has been stopped");
        return Task.CompletedTask;
    }

    private bool CanStopAll(object images)
    {
        return _isLoading.Any(il => il);
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