using Microsoft.Maui.Controls;

namespace LOCATEM_DESKTOP.Behaviors
{
    /// <summary>
    /// Behavior que reproduz a animação CSS "gentleShake" (0.4s, translateX -4/+4px)
    /// usada em FormInput.module.css / PasswordInput.module.css / TokenInput.module.css.
    /// Basta fazer bind de IsShaking a um FieldErrorState.Shake — sem lógica de negócio no code-behind.
    /// </summary>
    public class ShakeBehavior : Behavior<VisualElement>
    {
        public static readonly BindableProperty IsShakingProperty =
            BindableProperty.Create(nameof(IsShaking), typeof(bool), typeof(ShakeBehavior), false, propertyChanged: OnIsShakingChanged);

        public bool IsShaking
        {
            get => (bool)GetValue(IsShakingProperty);
            set => SetValue(IsShakingProperty, value);
        }

        private VisualElement? _target;

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            _target = bindable;
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            _target = null;
            base.OnDetachingFrom(bindable);
        }

        private static async void OnIsShakingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not ShakeBehavior behavior || behavior._target is null) return;
            if (newValue is not true) return;

            var view = behavior._target;
            await view.TranslateTo(-4, 0, 40);
            await view.TranslateTo(4, 0, 80);
            await view.TranslateTo(-4, 0, 80);
            await view.TranslateTo(4, 0, 80);
            await view.TranslateTo(0, 0, 60);
        }
    }
}
