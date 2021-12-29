using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DerekWare.Iris
{
    public partial class BrightnessPanel : UserControl
    {
        readonly object TimerLock = new();
        double _Brightness;
        bool InUpdate;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<BrightnessChangedEventArgs> BrightnessChanged;

        public BrightnessPanel()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public double Brightness
        {
            get => _Brightness;
            set
            {
                _Brightness = value;
                InUpdate = true;
                TrackBar.Value = (int)(value * TrackBar.Maximum);
                InUpdate = false;
            }
        }

        #region Event Handlers

        void BrightnessTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            lock(TimerLock)
            {
                Timer.Start();
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            lock(TimerLock)
            {
                Timer.Stop();
            }

            BrightnessChanged?.Invoke(this, new BrightnessChangedEventArgs { Property = (double)TrackBar.Value / TrackBar.Maximum });
        }

        #endregion
    }
}
