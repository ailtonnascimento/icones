using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using XProtect.ExternalEvents.IconPlugin.Models;

namespace XProtect.ExternalEvents.IconPlugin.Controls
{
    /// <summary>
    /// Code-behind do controle WPF AlarmRingControl.
    /// Expõe DependencyProperties para binding com o ViewModel.
    /// </summary>
    public partial class AlarmRingControl : UserControl
    {
        #region DependencyProperties

        public static readonly DependencyProperty IconStateProperty =
            DependencyProperty.Register(
                nameof(IconState),
                typeof(IconState),
                typeof(AlarmRingControl),
                new PropertyMetadata(IconState.Normal, OnIconStateChanged)
            );

        public static readonly DependencyProperty DeviceImageProperty =
            DependencyProperty.Register(
                nameof(DeviceImage),
                typeof(BitmapImage),
                typeof(AlarmRingControl),
                new PropertyMetadata(null, OnDeviceImageChanged)
            );

        /// <summary>
        /// Estado atual do ícone: Normal, Alarm ou Failure.
        /// Controla qual anel é exibido e qual animação é executada.
        /// </summary>
        public IconState IconState
        {
            get => (IconState)GetValue(IconStateProperty);
            set => SetValue(IconStateProperty, value);
        }

        /// <summary>
        /// Imagem do dispositivo exibida no centro do controle.
        /// </summary>
        public BitmapImage DeviceImage
        {
            get => (BitmapImage)GetValue(DeviceImageProperty);
            set => SetValue(DeviceImageProperty, value);
        }

        #endregion

        public AlarmRingControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Property changed callbacks

        private static void OnIconStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not AlarmRingControl ctrl) return;
            var newState = (IconState)e.NewValue;

            // Resetar opacidade dos anéis antes de aplicar novo estado
            ctrl.AlarmRing.Opacity = 0;
            ctrl.FailureRing.Opacity = 0;

            switch (newState)
            {
                case IconState.Alarm:
                    ctrl.AlarmRing.Opacity = 1;
                    break;
                case IconState.Failure:
                    ctrl.FailureRing.Opacity = 1;
                    break;
                case IconState.Normal:
                default:
                    // Anéis já zerados acima
                    break;
            }
        }

        private static void OnDeviceImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AlarmRingControl ctrl && e.NewValue is BitmapImage img)
                ctrl.DeviceIcon.Source = img;
        }

        #endregion

        /// <summary>
        /// Define o estado diretamente, útil para chamadas sem binding.
        /// </summary>
        public void SetState(IconState state) => IconState = state;
    }
}
