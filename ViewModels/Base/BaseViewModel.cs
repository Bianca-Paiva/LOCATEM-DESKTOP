using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LOCATEM_DESKTOP.ViewModels.Base
{
    /// <summary>
    /// Base MVVM simples para os ViewModels do app (equivalente ao estado dos hooks React).
    /// Não depende de nenhum pacote externo de MVVM — apenas INotifyPropertyChanged puro.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isBusy;
        /// <summary>Estado de loading — equivalente aos "submitting"/"isLoading" das telas React.</summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                    OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
