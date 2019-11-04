using System;
using System.Drawing;
using System.Windows.Forms;
using NWaves.Signals;

namespace PCAudioProcessing.UserControls
{
    public partial class SignalPlot : UserControl
    {
        /// <summary>
        /// Signal to plot
        /// </summary>
        private DiscreteSignal _signal;
        public DiscreteSignal Signal
        {
            get { return _signal; }
            set
            {
                _signal = value;
                if (_signal == null) return;
                AutoScrollMinSize = new Size(_signal.Length / _stride + 10, 0);
                MakeBitmap();
                Invalidate();
            }
        }

        private int _stride = 64;
        public int Stride
        {
            get { return _stride; }
            set
            {
                _stride = value > 1 ? value : 1;
                if (_signal == null) return;
                AutoScrollMinSize = new Size(_signal.Length / _stride + 10, 0);
                MakeBitmap();
                Invalidate();
            }
        }

        public float Gain { get; set; } = 1;
        public float max_time_value { get; set; } = 20000;
        public int PaddingX { get; set; } = 24;
        public int PaddingY { get; set; } = 24;


        public SignalPlot()
        {
            InitializeComponent();
            ForeColor = Color.Blue;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_bmp == null) MakeBitmap();

            e.Graphics.DrawImage(_bmp, 0, 0, 
                new Rectangle(-AutoScrollPosition.X, 0, Width, Height),
                GraphicsUnit.Pixel);
        }

        private void buttonZoomIn_Click(object sender, System.EventArgs e)
        {
            if (_stride < 4)
            {
                Stride++;
            }
            else
            {
                Stride = (int)(_stride * 1.25);
            }
        }

        private void buttonZoomOut_Click(object sender, System.EventArgs e)
        {
            Stride = (int)(_stride / 1.25);
        }


        private Bitmap _bmp;

        private void MakeBitmap()
        {
            var width = Math.Max(AutoScrollMinSize.Width, Width);

            _bmp = new Bitmap(width, Height);

            var g = Graphics.FromImage(_bmp);
            g.Clear(Color.White);

            var offset = Height / 2;

            var gray = new Pen(Color.LightGray) { DashPattern = new[] { 2f, 2f } };

            for (var k = 0; k < offset; k += 10)
            {
                g.DrawLine(gray, 0, offset + k, width, offset + k);
                g.DrawLine(gray, 0, offset - k, width, offset - k);
            }

            gray.Dispose();

            if (_signal != null)
            {
                DrawAxes(g, -(Height - 2 * PaddingY) / (2 * Gain), 
                             (Height - 2 * PaddingY) / (2 * Gain));

                var pen = new Pen(ForeColor);

                var i = 0;
                var x = PaddingX;

                while (i < _signal.Length - _stride)
                {
                    var j = 0;
                    var min = 0.0;
                    var max = 0.0;
                    while (j < _stride)
                    {
                        if (_signal[i + j] > max) max = _signal[i + j];
                        if (_signal[i + j] < min) min = _signal[i + j];
                        j++;
                    }
                    g.DrawLine(pen, x, (float) (-min*Gain) + offset, x, (float) (-max*Gain) + offset);
                    x++;
                    i += _stride;
                }

                pen.Dispose();
            }

            g.Dispose();
        }

        public void refresh()
        {
            var width = Math.Max(AutoScrollMinSize.Width, Width);

            _bmp = new Bitmap(width, Height);

            var g = Graphics.FromImage(_bmp);
            g.Clear(Color.White);
            Invalidate();

            var offset = Height / 2;

            var gray = new Pen(Color.LightGray) { DashPattern = new[] { 2f, 2f } };

            for (var k = 0; k < offset; k += 10)
            {
                g.DrawLine(gray, 0, offset + k, width, offset + k);
                g.DrawLine(gray, 0, offset - k, width, offset - k);
            }

            gray.Dispose();
            g.Dispose();
        }
        private void DrawAxes(Graphics g, float min, float max)
        {
            var black = new Pen(Color.Black);

            g.DrawLine(black, PaddingX, Height/2, _bmp.Width, Height/2);
            g.DrawLine(black, PaddingX, 10, PaddingX, Height - PaddingY);

            var font = new Font("arial", 5);
            var brush = new SolidBrush(Color.Black);

            const int stride = 10;
            var pos = Height + 2;
            var n = (Height - 2 * PaddingY) / stride;
            for (var i = 0; i <= n; i++)
            {
                g.DrawString(string.Format("{0:F2}", min + i * (max - min) / n), font, brush, 1, pos -= stride);
            }

            var font_line = new Font("arial", 10);
            var brush_line = new SolidBrush(Color.Black);

            var width = Math.Max(AutoScrollMinSize.Width, Width);
            var posX = 0;
            
            var step = 100;
            var nX = max_time_value / step;
            var strideX = (float) width / nX;

            var posX_line = width;
            float posX_temp = posX - 50;
            float posX_line_temp = posX - 48;

            for (var i = 0; i <= nX; i++)
            {
                g.DrawString(string.Format("{0}", i * step) + " ms", font, brush, posX_temp += strideX, 1);
                g.DrawString("|", font_line, brush_line, posX_line_temp += strideX, 10);
            }

            font.Dispose();
            brush.Dispose();

            black.Dispose();
        }
    }
}
