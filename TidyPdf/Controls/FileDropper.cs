using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.VoiceCommands;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TidyPdf.Controls
{
    public sealed partial class FileDropper : Control
    {

        public string FileFormats
        {
            get { return (string)GetValue(FileFormatsProperty); }
            set { SetValue(FileFormatsProperty, value); }
        }

        public static readonly DependencyProperty FileFormatsProperty =
            DependencyProperty.Register(nameof(FileFormats), typeof(string), typeof(FileDropper), new PropertyMetadata(string.Empty));



        public bool ShowText
        {
            get { return (bool)GetValue(ShowTextProperty); }
            set { SetValue(ShowTextProperty, value); }
        }

        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register(nameof(ShowText), typeof(bool), typeof(FileDropper), new PropertyMetadata(true));

        public bool ShowDashedLine
        {
            get { return (bool)GetValue(ShowDashedLineProperty); }
            set { SetValue(ShowDashedLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDashedLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDashedLineProperty =
            DependencyProperty.Register(nameof(ShowDashedLine), typeof(bool), typeof(FileDropper), new PropertyMetadata(true));


        private bool isDragging;
        private bool isPointerOver;
        private bool isPressed;


        public FileDropper()
        {
            DefaultStyleKey = typeof(FileDropper);

            this.Unloaded += FileDropper_Unloaded;
            this.IsEnabledChanged += FileDropper_IsEnabledChanged;
        }
        private void FileDropper_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isDragging = false;
            isPointerOver = false;
            isPressed = false;
            UpdateVisualState();
        }


        private void FileDropper_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= FileDropper_Unloaded;
            this.IsEnabledChanged -= FileDropper_IsEnabledChanged;
        }

        private void UpdateVisualState()
        {
            string state = "Normal";

            if (this.IsEnabled)
            {
                if (isPressed)
                {
                    state = "Pressed";
                }
                else if (isDragging)
                {
                    state = "DragOver";
                }
                else if (isPointerOver)
                {
                    state = "PointerOver";
                }
            }
            else
            {
                state = "Disabled";
            }

            VisualStateManager.GoToState(this, state, true);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            isPressed = false;
            isPointerOver = false;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);


            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                isDragging = true;
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }

            UpdateVisualState();
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            isDragging = false;
            UpdateVisualState();
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);

            isDragging = false;
            UpdateVisualState();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);

            isPointerOver = true;
            UpdateVisualState();
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);

            isPointerOver = false;
            isPressed = false;
            UpdateVisualState();
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            isPressed = false;
            UpdateVisualState();
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            isPressed = true;
            UpdateVisualState();
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
        }

    }
}
