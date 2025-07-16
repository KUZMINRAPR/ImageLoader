using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageLoader.Commands;
using ImageLoader.Model;

namespace ImageLoader.ViewModel;

public class StartViewModel: INotifyPropertyChanged
{
    private ObservableCollection<BitmapImage> _images = new() {null, null, null};
    private bool[] _isLoading = new bool[3];
    private ProgressState[] _progressStates = {ProgressState.NotStarted, ProgressState.NotStarted, ProgressState.NotStarted};
    
    private readonly StopViewModel _stopViewModel;

    /// <summary>
    ///  Список в котором хранятся изображения, полученные из UrI
    /// </summary>
    public ObservableCollection<BitmapImage> Images
    {
        get => _images;
        set
        {
            _images = value;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// Массив для хранения состояния картинок
    /// </summary>
    public bool[] IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }
    
    public ProgressState[] ProgressStates
    {
        get => _progressStates;
        set
        {
            _progressStates = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LoadingProgress));
        }
    }
    public double LoadingProgress
    {
        get => Progress.GetProgress(ProgressStates);
        set
        {
            LoadingProgress = value;
            OnPropertyChanged();
        }
    }
    


    /// <summary>
    /// Команда для обновления картинки
    /// </summary>
    public ICommand UpdateImageCommand { get; }

    public StartViewModel(StopViewModel stopViewModel)
    {
        _stopViewModel = stopViewModel;
        UpdateImageCommand = new RelayCommand(Start, CanStart);
        _stopViewModel.IsLoading = IsLoading;
    }

    /// <summary>
    /// Метод для получения изображений из Model
    /// </summary>
    /// <param name="imageConverter">Параметры изображения(Uri и Index картинки слева направо)</param>
    public async Task Start(object imageConverter)
    {
        if (imageConverter is ImageConverter ic && ic.Index is int index)
        {
            _stopViewModel.CancellationTokens[index]?.Dispose();
            _stopViewModel.CancellationTokens[index] = new CancellationTokenSource();
            var token = _stopViewModel.CancellationTokens[index].Token;
            ProgressStates[index] = ProgressState.NotStarted;
            try
            {
                _isLoading[index] = true;
                OnPropertyChanged(nameof(IsLoading));
                CommandManager.InvalidateRequerySuggested();

                Images[index] = await StartModel.GetImage(ic.Uri, token);
                
                ProgressStates[index] = ProgressState.InProgress;
            }
            catch (OperationCanceledException)
            {
                // TODO: Потом если что вместе с консолью писать это на ProgressBar
                Console.WriteLine($"Image {index} stopped loading ");
                ProgressStates[index] = ProgressState.NotStarted;
                OnPropertyChanged(nameof(LoadingProgress));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _isLoading[index] = false;
                OnPropertyChanged(nameof(LoadingProgress));
                await Task.Delay(1000);
                if (ProgressStates[index] != ProgressState.NotStarted)
                {
                    ProgressStates[index] = ProgressState.Completed;
                };
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(LoadingProgress));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        else throw new ArgumentException("imageConverter is not ImageConverter");
    }

    private bool CanStart(object parameter)
    {
        if (parameter is ImageConverter ic && ic.Index is int idx)
            return !_isLoading[idx];
        return true;
    }
    
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