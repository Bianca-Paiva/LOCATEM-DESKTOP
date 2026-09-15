using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LOCATEM_DESKTOP.Helpers.Auth
{
    /// <summary>
    /// Equivalente ao "ErrorState { active, shake }" usado em Login.tsx/useCadastroForm.ts
    /// para controlar a borda vermelha (Active) e a animação de "chacoalhar" (Shake) de um campo.
    /// </summary>
    public class FieldErrorState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _active;
        public bool Active
        {
            get => _active;
            set { _active = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Active))); }
        }

        private bool _shake;
        public bool Shake
        {
            get => _shake;
            set { _shake = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Shake))); }
        }

        /// <summary>
        /// Replica triggerShake(field) do React: zera o shake, espera um tick e ativa
        /// Active+Shake, desligando o Shake de novo depois de 400ms (a "borda vermelha"
        /// em Active permanece ligada até o campo ser corrigido).
        /// </summary>
        public async void Trigger()
        {
            Shake = false;
            await Task.Delay(10);
            Active = true;
            Shake = true;
            await Task.Delay(400);
            Shake = false;
        }

        /// <summary>Equivalente a clearShake(field): some com o erro assim que o usuário corrige o campo.</summary>
        public void Clear()
        {
            if (Active)
            {
                Active = false;
                Shake = false;
            }
        }

        public void Reset()
        {
            Active = false;
            Shake = false;
        }
    }
}
