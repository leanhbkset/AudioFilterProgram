using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NWaves.Audio;
using NWaves.Features;
using NWaves.Operations;
using NWaves.Signals;
using NWaves.Transforms;
using NWaves.Windows;
using NWaves.Audio.Interfaces;
using NWaves.Audio.Mci;
using LevelScale = NWaves.Utils.Scale;
using NAudio.Wave;
//using Excel = Microsoft.Office.Interop.Excel;
//using System.Runtime.InteropServices;

namespace PCAudioProcessing
{
    public partial class AudioProcessing : Form
    {

        public Ifilter _oWav;   // FFT -> Filter -> IFFT

        private DiscreteSignal _signal;

        private DiscreteSignal _signal_storage;

        private Fft _fft;
        private CepstralTransform _cepstralTransform;
        private Stft _stft;

        private string _waveFileName;
        private readonly MciAudioPlayer _player = new MciAudioPlayer();
        private bool _hasStartedPlaying;
        private bool _isPaused;

        private int _fftSize;
        private int _hopSize;
        private int _cepstrumSize;

        private List<float> _pitchTrack;

        private int _specNo;

        private int no_point;

        private double[] pattern_array_1;
        private double[] pattern_array_2;

        public AudioProcessing()
        {
            InitializeComponent();

            signalPanel.Gain = 100;
            signalPanelAfterFilter.Gain = 100;
            lblx.Visible = false;
            lblNote.Visible = false;
        }

        // Open file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            spectrumPanel.refresh();
            signalPanel.refresh();

            signalPanel.Stride = 64;

            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtFilePath.Text = ofd.FileName;
            _waveFileName = ofd.FileName;

            try
            {
                using (var stream = new FileStream(_waveFileName, FileMode.Open))
                {
                    IAudioContainer waveFile = new WaveFile(stream);
                    _signal = waveFile[Channels.Left];
                    _signal_storage = _signal;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error reading from {0}. Message = {1}", _waveFileName, ex.Message);
            }

            // Get duration
            WaveFileReader wf = new WaveFileReader(_waveFileName);
            var totalTime = wf.TotalTime;
            txtTotalTime.Text = totalTime.TotalMilliseconds.ToString() + " ms";
            signalPanel.max_time_value = (float)totalTime.TotalMilliseconds;

            var max_value = _signal[0];
            var count = 0;
            for (int i = 1; i < _signal.Length; i++)
            {
                if (max_value < _signal[i])
                {
                    max_value = _signal[i];
                }

                if (_signal[i] > 0.2)
                {
                    count++;
                }
            }
            txtMaxValue.Text = max_value.ToString();

            signalPanel.Signal = _signal;

            _fftSize = int.Parse(fftSizeTextBox.Text);
            _cepstrumSize = int.Parse(cepstrumSizeTextBox.Text);
            _hopSize = int.Parse(hopSizeTextBox.Text);

            _fft = new Fft(_fftSize);
            _cepstralTransform = new CepstralTransform(_cepstrumSize, _fftSize);

            var pitchTracker = new Pitch((float)_fftSize / _signal.SamplingRate,
                                         (float)_hopSize / _signal.SamplingRate);

            _pitchTrack = pitchTracker.Track(_signal);

            // Show chart in frequency domain
            UpdateAutoCorrelation();
            UpdateSpectra();
            
            // obtain spectrogram

            _stft = new Stft(_fftSize, _hopSize, WindowTypes.Rectangular);
            var spectrogram = _stft.Spectrogram(_signal);

            specNoComboBox.DataSource = Enumerable.Range(1, _pitchTrack.Count).ToArray();

            _specNo = 0;

            spectrumPanelAfterFilter.refresh();
            signalPanelAfterFilter.refresh();
            lblNote.Visible = false;
            savePWMDataToolStripMenuItem.Enabled = true;
        }

        private void specNoComboBox_TextChanged(object sender, EventArgs e)
        {
            _specNo = int.Parse(specNoComboBox.Text) - 1;
            UpdateAutoCorrelation();
            UpdateSpectra();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            _specNo--;
            specNoComboBox.Text = (_specNo + 1).ToString();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            _specNo++;
            specNoComboBox.Text = (_specNo + 1).ToString();
        }

        // Update chart of frequency domain
        private void UpdateSpectra()
        {
            var fftSize = int.Parse(fftSizeTextBox.Text);
            var cepstrumSize = int.Parse(cepstrumSizeTextBox.Text);
            _hopSize = int.Parse(hopSizeTextBox.Text);

            if (fftSize != _fftSize)
            {
                _fftSize = fftSize;
                _fft = new Fft(fftSize);
                _cepstralTransform = new CepstralTransform(cepstrumSize, _fftSize);
            }

            if (cepstrumSize != _cepstrumSize)
            {
                _cepstrumSize = cepstrumSize;
                _cepstralTransform = new CepstralTransform(_cepstrumSize, _fftSize);
            }
            
            var pos = _hopSize * _specNo;
            var block = _signal[pos, pos + _fftSize];
            block.ApplyWindow(WindowTypes.Hamming);

            var cepstrum = _cepstralTransform.Direct(block);

            var real = new float[_fftSize];
            var imag = new float[_fftSize];

            for (var i = 0; i < 32; i++)
            {
                real[i] = cepstrum[i];
            }

            _fft.Direct(real, imag);

            var spectrum = _fft.PowerSpectrum(block, normalize: false).Samples;
            var avg = spectrum.Average(s => LevelScale.ToDecibel(s));

            var spectrumEstimate = real.Take(_fftSize / 2 + 1)
                                       .Select(s => (float)LevelScale.FromDecibel(s * 40 / _fftSize - avg))
                                       .ToArray();

            spectrumPanel.Line = spectrum;
            spectrumPanel.Markline = spectrumEstimate;
            spectrumPanel.ToDecibel();

            spectrumPanel.max_freq_value = spectrum.Length * _signal.SamplingRate / fftSize;
        }

        private void UpdateAutoCorrelation()
        {
            var pos = _hopSize * _specNo;
            var block = _signal[pos, pos + _fftSize];

            var fftSize = int.Parse(fftSizeTextBox.Text);
            var spectrum = _fft.PowerSpectrum(block, normalize: false).Samples;

            var fft_max_value = spectrum[1];
            var fft_max_index = 1;
            for (int i = 2; i < spectrum.Length; i++)
            {
                if (fft_max_value < spectrum[i])
                {
                    fft_max_value = spectrum[i];
                    fft_max_index = i;
                }
            }

            var pitch = fft_max_index * _signal.SamplingRate / fftSize;
            txtFreq.Text = pitch.ToString();

            spectrumPanel.Mark = _fftSize * pitch / _signal.SamplingRate;    // pitch index
            spectrumPanel.Legend = string.Format("{0:F2} Hz", pitch);

            var autoCorrelation = Operation.CrossCorrelate(block, block).Last(_fftSize);
        }

        // Update spectra after filter
        private void UpdateSpectraAfterFilter()
        {
            var fftSize = int.Parse(fftSizeTextBox.Text);
            var cepstrumSize = int.Parse(cepstrumSizeTextBox.Text);
            _hopSize = int.Parse(hopSizeTextBox.Text);

            if (fftSize != _fftSize)
            {
                _fftSize = fftSize;
                _fft = new Fft(fftSize);
                _cepstralTransform = new CepstralTransform(cepstrumSize, _fftSize);
            }

            if (cepstrumSize != _cepstrumSize)
            {
                _cepstrumSize = cepstrumSize;
                _cepstralTransform = new CepstralTransform(_cepstrumSize, _fftSize);
            }

            var pos = _hopSize * _specNo;
            var block = _signal[pos, pos + _fftSize];
            block.ApplyWindow(WindowTypes.Hamming);

            var cepstrum = _cepstralTransform.Direct(block);

            var real = new float[_fftSize];
            var imag = new float[_fftSize];

            for (var i = 0; i < 32; i++)
            {
                real[i] = cepstrum[i];
            }

            _fft.Direct(real, imag);

            var spectrum = _fft.PowerSpectrum(block, normalize: false).Samples;
            var avg = spectrum.Average(s => LevelScale.ToDecibel(s));

            var spectrumEstimate = real.Take(_fftSize / 2 + 1)
                                       .Select(s => (float)LevelScale.FromDecibel(s * 40 / _fftSize - avg))
                                       .ToArray();

            spectrumPanelAfterFilter.Line = spectrum;
            spectrumPanelAfterFilter.Markline = spectrumEstimate;
            spectrumPanelAfterFilter.ToDecibel();
            spectrumPanelAfterFilter.max_freq_value = spectrum.Length * _signal.SamplingRate / fftSize;
        }

        private void UpdateAutoCorrelationAfterFilter()
        {
            var pos = _hopSize * _specNo;
            var block = _signal[pos, pos + _fftSize];

            // Anh's coding
            var fftSize = int.Parse(fftSizeTextBox.Text);
            var spectrum = _fft.PowerSpectrum(block, normalize: false).Samples;

            var fft_max_value = spectrum[1];
            var fft_max_index = 1;
            for (int i = 2; i < spectrum.Length; i++)
            {
                if (fft_max_value < spectrum[i])
                {
                    fft_max_value = spectrum[i];
                    fft_max_index = i;
                }
            }

            var pitch = fft_max_index * _signal.SamplingRate / fftSize;
            // End of Anh's coding

            spectrumPanelAfterFilter.Mark = _fftSize * pitch / _signal.SamplingRate;    // pitch index
            spectrumPanelAfterFilter.Legend = string.Format("{0:F2} Hz", pitch);

            var autoCorrelation = Operation.CrossCorrelate(block, block).Last(_fftSize);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        private async void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_waveFileName == null || _isPaused)
            {
                return;
            }

            // Play after filter
            string wavePlay = "wavPlay.wav";

            try
            {
                using (var stream = new FileStream(wavePlay, FileMode.Create))
                {
                    var waveFile = new WaveFile(_signal);
                    waveFile.SaveTo(stream);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error reading from {0}. Message = {1}", wavePlay, ex.Message);
            }

            _hasStartedPlaying = true;

            await _player.PlayAsync(wavePlay);

            _hasStartedPlaying = false;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_waveFileName == null || _hasStartedPlaying == false)
            {
                return;
            }

            var menuItem = sender as ToolStripMenuItem;

            if (_isPaused)
            {
                _player.Resume();
                menuItem.Text = "Pause";
            }
            else
            {
                _player.Pause();
                menuItem.Text = "Resume";
            }

            _isPaused = !_isPaused;
        }

        int x = 0, y = 0;
        private void spectrumPanel_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            lblx.Visible = true;

            var fftSize = int.Parse(fftSizeTextBox.Text);

            x = e.X;
            y = e.Y;
            
            lblx.Location = new Point(x - lblx.Width + 80, y + 360);

            var x_value = 0;
            if (_waveFileName != null)
            {
                x_value = (x - 30) * _signal.SamplingRate / fftSize;
            }
            

            lblx.Text = "Frequency: " + x_value.ToString() + " Hz";
        }

        private void AudioProcessing_MouseMove(object sender, MouseEventArgs e)
        {
            lblx.Visible = false;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isPaused)
            {
                pauseToolStripMenuItem_Click(this.menuStrip1.Items[2], null);
            }

            _player.Stop();
            _hasStartedPlaying = false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "wav | *.wav | mp3 | *.mp3",
                AddExtension = true,
                DefaultExt = "wav"
            };

            if (sfd.ShowDialog() != DialogResult.OK) 
            {
                return;
            }

            using (var stream = new FileStream(sfd.FileName, FileMode.Create))
            {
                var waveFile = new WaveFile(_signal*2);
                waveFile.SaveTo(stream);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (_waveFileName != null)
            {
                filterProcessing();
            }
            else
            {
                lblNote.Visible = true;
                lblNote.Text = "Please open file";
            }

            Pattern_Value_1();
            Pattern_Value_2();
        }

        private void btnSaveas_Click(object sender, EventArgs e)
        {
            saveAsToolStripMenuItem_Click(sender, e);
        }

        // Refresh papameters and chart of time domain and frequency domain
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            signalPanel.Stride = 64;
            signalPanel.refresh();
            signalPanelAfterFilter.refresh();
            spectrumPanel.refresh();
            spectrumPanelAfterFilter.refresh();
            _waveFileName = null;
            txtFilePath.Text = "";
            txtFreq.Text = "0";
            lblNote.Visible = false;
            savePWMDataToolStripMenuItem.Enabled = false;
            txtMaxValue.Text = "";
            txtTotalTime.Text = "";
        }

        // Filter processing
        private void filterProcessing()
        {
            _oWav = new Ifilter();

            _oWav._aSamples = new float[_signal.Length];

            _oWav._aSamples = _signal.Samples;
            _oWav._nHz = _signal.SamplingRate;

            var nOriginalLen = _oWav._aSamples.Length;

            _oWav._np2 = (int)Math.Pow(2, Math.Round((Math.Log(nOriginalLen) / Math.Log(2)), 0)); // round array len to nearest power of 2

            if (_oWav._np2 < nOriginalLen)
            {

                _oWav._np2 *= 2;

            }

            Array.Resize<float>(ref _oWav._aSamples, _oWav._np2);

            for (int i = nOriginalLen + 1; i < _oWav._np2; i++)
            {

                _oWav._aSamples[i] = 0;

            }

            _oWav._aImaginary = new float[_oWav._np2];

            // Filter
            _oWav.BandpassFilter(int.Parse(txtFreq.Text), int.Parse(txtWidthFreq.Text));

            // Inverse to time domain
            _oWav.EnsureDomain(Ifilter.Domain.TimeDomain); // Inverse FFT
            var aSignal_temp = _oWav._aSamples;

            // Calculate scale
            long index = long.Parse(txtFreq.Text) * _oWav._np2 / _oWav._nHz;
            var scale = _signal_storage.Samples[index] / aSignal_temp[index];

            // Assign value to signal
            for (int i = 0; i <_signal.Length; i++)
            {
                _signal.Samples[i] = aSignal_temp[i] * scale;
            }

            // Duration
            WaveFileReader wf = new WaveFileReader(_waveFileName);
            var totalTime = wf.TotalTime;
            signalPanelAfterFilter.max_time_value = (float)totalTime.TotalMilliseconds;
            // Update time domain chart
            signalPanelAfterFilter.Stride = 64;
            signalPanelAfterFilter.Signal = _signal;

            // Update frequency domain chart
            UpdateSpectraAfterFilter();
            UpdateAutoCorrelationAfterFilter();
            UpdateSpectraAfterFilter();
        }

        private void Pattern_Value_1()
        {
            // Find max value
            var max_value = _signal[0];
            for (int i = 1; i < _signal.Length; i++)
            {
                if (max_value < _signal[i])
                {
                    max_value = _signal[i];
                }
            }

            // Duration
            WaveFileReader wf = new WaveFileReader(_waveFileName);
            var totalTime = wf.TotalTime.TotalMilliseconds;

            var time_period = 100;  // Period of one play is 100 ms
            no_point = time_period * _signal.Length / int.Parse(totalTime.ToString());

            var no_period = _signal.Length / no_point;

            pattern_array_1 = new double[no_period];

            double threshold = 0.1;

            for (int j = 0; j < no_period; j++)
            {
                double pattern_value = 0;
                for (int i = j * no_point; i < (j + 1) * no_point; i++)
                {
                    if (_signal[i] > threshold)
                    {
                        pattern_value += _signal[i] - threshold;
                    }
                }

                pattern_array_1[j] = pattern_value / (no_point * (max_value - threshold));
            }

            using (StreamWriter file =
            new StreamWriter(@"C:\Users\CKML202-1\Desktop\Test files\File\Pattern_Value_1.txt", true))
            {
                for (int i = 0; i < pattern_array_1.Length; i++)
                {
                    file.WriteLine(pattern_array_1[i].ToString());
                }
            }
        }

        private void savePWMDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Write to a text file
            float[] maserati_data = new float[_signal.Samples.Length / 20];
            using (StreamWriter file =
            new StreamWriter(@"C:\Users\CKML202-1\Desktop\File storage\Maserati_data.txt", false))
            {
                for (var i = 0; i < _signal.Samples.Length; i++)
                {

                    double maserati_store = Math.Round(Math.Abs(23 * _signal.Samples[i]), 0);

                    if ((i % 20) == 0)
                    {
                        file.Write("\n");
                    }
                    file.Write((maserati_store).ToString() + ",");
                } 
            }
        }

        private void Pattern_Value_2()
        {
            // Find max value
            var max_value = _signal[0];
            for (int i = 1; i < _signal.Length; i++)
            {
                if (max_value < _signal[i])
                {
                    max_value = _signal[i];
                }
            }

            // Duration
            WaveFileReader wf = new WaveFileReader(_waveFileName);
            var totalTime = wf.TotalTime.TotalMilliseconds;

            var time_period = 100;  // Period of one play is 100 ms
            no_point = time_period * _signal.Length / int.Parse(totalTime.ToString());

            var no_period = _signal.Length / no_point;

            pattern_array_2 = new double[no_period];

            double threshold = 0.1;

            for (int j = 0; j < no_period; j++)
            {
                double pattern_value = 0;
                for (int i = j * no_point; i < (j + 1) * no_point; i++)
                {
                    if ((_signal[i] > threshold) && (pattern_value < _signal[i] - threshold))
                    {
                        pattern_value = _signal[i] - threshold; // Find max value when consider threshold
                    }
                }

                pattern_array_2[j] = pattern_value / (max_value - threshold);
            }

            using (StreamWriter file =
            new StreamWriter(@"C:\Users\CKML202-1\Desktop\Test files\File\Pattern_Value_2.txt", true))
            {
                for (int i = 0; i < pattern_array_2.Length; i++)
                {
                    file.WriteLine(pattern_array_2[i].ToString());
                }
            }
        }

        private void segmentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Segmentation();
            Segmentation_HighAndLow_Freqency();
        }

        private void Segmentation()
        {

            DiscreteSignal _segment;

            // Duration
            WaveFileReader wf = new WaveFileReader(_waveFileName);
            var totalTime = wf.TotalTime.TotalMilliseconds;

            var time_period = 200;  // Period of one segmentation
            no_point = time_period * _signal.Length / int.Parse(totalTime.ToString());    // No. of points of one segmentation

            var no_segement = _signal.Length / no_point;    // No. of segmentation

            int[] max_freq_segment = new int[no_segement];
            double[] max_value_segment = new double[no_segement];
            int[] min_freq_segment = new int[no_segement];
            double[] min_value_segment = new double[no_segement];

            for (var i = 0; i < no_segement; i++)
            {
                _segment = _signal[i* no_point, i* no_point + no_point];

                var pos = _hopSize * _specNo;
                var block = _segment[pos, pos + _fftSize];

                //var fftSize = int.Parse(fftSizeTextBox.Text);
                var spectrum = _fft.PowerSpectrum(block, normalize: false).Samples;

                // Find max frequency value
                var fft_max_value = spectrum[1];
                var fft_max_index = 1;
                for (int j = 2; j < spectrum.Length; j++)
                {
                    if (fft_max_value < spectrum[j])
                    {
                        fft_max_value = spectrum[j];
                        fft_max_index = j;
                    }
                }

                max_freq_segment[i] = fft_max_index * _signal.SamplingRate / _fftSize;
                max_value_segment[i] =  spectrum[fft_max_index];

                // Find min frequency value
                var fft_min_value = spectrum[1];
                var fft_min_index = 1;
                for (int j = 2; j < spectrum.Length; j++)
                {
                    if (fft_min_value > spectrum[j])
                    {
                        fft_min_value = spectrum[j];
                        fft_min_index = j;
                    }
                }

                min_freq_segment[i] = fft_min_index * _signal.SamplingRate / _fftSize;
                min_value_segment[i] = spectrum[fft_min_index];
            }

            using (StreamWriter file =
            new StreamWriter(@"C:\Users\CKML202-1\Desktop\File storage\segmentation.txt", false))
            {
                file.WriteLine("Total of segmentations: " + no_segement + "\n");
                for (var i = 0; i < no_segement; i++)
                {
                    file.WriteLine("Segment " + (i + 1) + ":");
                    file.WriteLine("    MAX : " + max_freq_segment[i].ToString() + " Hz,"
                        + " Value: " + max_value_segment[i].ToString());
                    file.WriteLine("    min: " + min_freq_segment[i].ToString() + "Hz,"
                        + " Value: " + min_value_segment[i].ToString());
                }
            }
        }

        private void Segmentation_HighAndLow_Freqency()
        {

            DiscreteSignal _segment;

            // Duration
            WaveFileReader wf = new WaveFileReader(_waveFileName);
            var totalTime = wf.TotalTime.TotalMilliseconds;

            var time_period = 200;  // Period of one segmentation
            no_point = time_period * _signal.Length / int.Parse(totalTime.ToString());    // No. of points of one segmentation

            var no_segement = _signal.Length / no_point;    // No. of segmentation

            // Low frequency parameters
            double lowFreq_index = (double)_fftSize / (double)_signal.SamplingRate;
            int low_freq_segment_size = 1;
            int[,] max_low_freq_segment = new int[no_segement, low_freq_segment_size];
            double[,] max_low_freq_value_segment = new double[no_segement, low_freq_segment_size];

            // High frequency parameters
            double highFreq_index = (double)_fftSize / (double)_signal.SamplingRate;
            int high_freq_segment_size = 5;
            int[,] max_high_freq_segment = new int[no_segement, high_freq_segment_size];
            double[,] max_high_freq_value_segment = new double[no_segement, high_freq_segment_size];

            for (var i = 0; i < no_segement; i++)
            {
                // Low frequency
                _segment = _signal[i * no_point, i * no_point + no_point];

                var pos = _hopSize * _specNo;
                var block = _segment[pos, pos + _fftSize];

                var spectrum = _fft.PowerSpectrum(block, normalize: false).Samples;

                // Low frequency
                // 10 - 200 Hz
                var fft_max_low_value = spectrum[(int) (10 * lowFreq_index)];
                var fft_max_low_index = (int) (10 * lowFreq_index) + 1;
                for (int j = (int) (10 * lowFreq_index) + 1; j <= (int)(200 * lowFreq_index); j++)
                {
                    if (fft_max_low_value < spectrum[j])
                    {
                        fft_max_low_value = spectrum[j];
                        fft_max_low_index = j;
                    }
                }

                max_low_freq_segment[i, 0] = fft_max_low_index * _signal.SamplingRate / _fftSize;
                max_low_freq_value_segment[i, 0] = spectrum[fft_max_low_index];


                /* High frequency */
                var fft_max_high_value = spectrum[0];
                var fft_max_high_index = 0;

                // Range 200 - 360 Hz
                fft_max_high_value = spectrum[(int)(200 * highFreq_index)];
                fft_max_high_index = (int) (200 * highFreq_index) + 1;
                for (int j = (int) (200 * highFreq_index) + 1; j <= (int) (360 * highFreq_index); j++)
                {
                    if (fft_max_high_value < spectrum[j])
                    {
                        fft_max_high_value = spectrum[j];
                        fft_max_high_index = j;
                    }
                }
                max_high_freq_segment[i, 0] = fft_max_high_index * _signal.SamplingRate / _fftSize;
                max_high_freq_value_segment[i, 0] = spectrum[fft_max_high_index];

                // Range 360 - 520 Hz
                fft_max_high_value = spectrum[(int)(360 * highFreq_index)];
                fft_max_high_index = (int) (360 * highFreq_index) + 1;
                for (int j = (int) (360 * highFreq_index) + 1; j <= (int) (520 * highFreq_index); j++)
                {
                    if (fft_max_high_value < spectrum[j])
                    {
                        fft_max_high_value = spectrum[j];
                        fft_max_high_index = j;
                    }
                }
                max_high_freq_segment[i, 1] = fft_max_high_index * _signal.SamplingRate / _fftSize;
                max_high_freq_value_segment[i, 1] = spectrum[fft_max_high_index];

                // Range 520 - 680 Hz
                fft_max_high_value = spectrum[(int) (520 * highFreq_index)];
                fft_max_high_index = (int) (520 * highFreq_index) + 1;
                for (int j = (int) (520 * highFreq_index) + 1; j <= (int) (680 * highFreq_index); j++)
                {
                    if (fft_max_high_value < spectrum[j])
                    {
                        fft_max_high_value = spectrum[j];
                        fft_max_high_index = j;
                    }
                }
                max_high_freq_segment[i, 2] = fft_max_high_index * _signal.SamplingRate / _fftSize;
                max_high_freq_value_segment[i, 2] = spectrum[fft_max_high_index];

                // Range 680 - 840 Hz
                fft_max_high_value = spectrum[(int) (680 * highFreq_index)];
                fft_max_high_index = (int) (680 * highFreq_index) + 1;
                for (int j = (int) (680 * highFreq_index) + 1; j <= (int) (840 * highFreq_index); j++)
                {
                    if (fft_max_high_value < spectrum[j])
                    {
                        fft_max_high_value = spectrum[j];
                        fft_max_high_index = j;
                    }
                }
                max_high_freq_segment[i, 3] = fft_max_high_index * _signal.SamplingRate / _fftSize;
                max_high_freq_value_segment[i, 3] = spectrum[fft_max_high_index];

                // Range 840 - 1000 Hz
                fft_max_high_value = spectrum[(int) (840 * highFreq_index)];
                fft_max_high_index = (int) (840 * highFreq_index) + 1;
                for (int j = (int) (840 * highFreq_index) + 1; j <= (int) (1000 * highFreq_index); j++)
                {
                    if (fft_max_high_value < spectrum[j])
                    {
                        fft_max_high_value = spectrum[j];
                        fft_max_high_index = j;
                    }
                }
                max_high_freq_segment[i, 4] = fft_max_high_index * _signal.SamplingRate / _fftSize;
                max_high_freq_value_segment[i, 4] = spectrum[fft_max_high_index];
            }

            using (StreamWriter file =
            new StreamWriter(@"C:\Users\CKML202-1\Desktop\File storage\Segmentation_HighAndLow_Freqency.txt", false))
            {
                file.WriteLine("Total of segmentations: " + no_segement + "\n");
                for (var i = 0; i < no_segement; i++)
                {
                    file.WriteLine("Segment " + (i + 1) + ":");
                    // low frequency: 10 - 200 Hz
                    
                    file.WriteLine("    Low frequency:");
                    file.WriteLine("        10 - 200 Hz,"
                        + "   Frequency: " + max_low_freq_segment[i, 0].ToString() + " Hz,"
                        + " Value: " + Math.Round(max_low_freq_value_segment[i, 0], 4).ToString());


                    // high frequency: 200 - 1000 Hz
                    file.WriteLine("    High frequency:");
                    
                    file.WriteLine("        200 - 360 Hz,"
                        + "  Frequency: " + max_high_freq_segment[i, 0].ToString() + " Hz,"
                        + " Value: " + Math.Round(max_high_freq_value_segment[i, 0], 4).ToString());
                    file.WriteLine("        360 - 520 Hz,"
                        + "  Frequency: " + max_high_freq_segment[i, 1].ToString() + " Hz,"
                        + " Value: " + Math.Round(max_high_freq_value_segment[i, 1], 4).ToString());
                    file.WriteLine("        520 - 680 Hz,"
                        + "  Frequency: " + max_high_freq_segment[i, 2].ToString() + " Hz,"
                        + " Value: " + Math.Round(max_high_freq_value_segment[i, 2], 4).ToString());
                    file.WriteLine("        680 - 840 Hz,"
                        + "  Frequency: " + max_high_freq_segment[i, 3].ToString() + " Hz,"
                        + " Value: " + Math.Round(max_high_freq_value_segment[i, 3], 4).ToString());
                    file.WriteLine("        840 - 1000 Hz,"
                        + " Frequency: " + max_high_freq_segment[i, 4].ToString() + " Hz,"
                        + " Value: " + Math.Round(max_high_freq_value_segment[i, 4], 4).ToString()); 

                }
            }

            // Create excel file
            //Excel.Application xlApp = new Excel.Application();
            //if (xlApp == null)
            //{
            //    MessageBox.Show("Excel is not properly installed !!");
            //    return;
            //}

            //Excel.Workbook xlWorkBook;
            //Excel.Worksheet xlWorkSheet;
            //object misValue = System.Reflection.Missing.Value;

            //xlWorkBook = xlApp.Workbooks.Add(misValue);
            //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //xlWorkSheet.Cells[1, 1] = "Segmentation";

            //xlWorkSheet.Cells[1, 2] = "10-200 Hz";
            //xlWorkSheet.Cells[1, 3] = "200-360 Hz";
            //xlWorkSheet.Cells[1, 4] = "360-520 Hz";
            //xlWorkSheet.Cells[1, 5] = "520-680 Hz";
            //xlWorkSheet.Cells[1, 6] = "680-840 Hz";
            //xlWorkSheet.Cells[1, 7] = "840-1000 Hz";
            //for (var i = 0; i < no_segement; i++)
            //{
            //    xlWorkSheet.Cells[i + 2, 1] = i + 1;
            //    xlWorkSheet.Cells[i + 2, 2] = Math.Round(max_low_freq_value_segment[i, 0], 4).ToString();
            //    xlWorkSheet.Cells[i + 2, 3] = Math.Round(max_high_freq_value_segment[i, 0], 4).ToString();
            //    xlWorkSheet.Cells[i + 2, 4] = Math.Round(max_high_freq_value_segment[i, 1], 4).ToString();
            //    xlWorkSheet.Cells[i + 2, 5] = Math.Round(max_high_freq_value_segment[i, 2], 4).ToString();
            //    xlWorkSheet.Cells[i + 2, 6] = Math.Round(max_high_freq_value_segment[i, 3], 4).ToString();
            //    xlWorkSheet.Cells[i + 2, 7] = Math.Round(max_high_freq_value_segment[i, 4], 4).ToString();
            //}

            //xlWorkBook.SaveAs("C:\\Users\\CKML202-1\\Desktop\\File storage\\segmentation.xlsx", Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            //xlWorkBook.Close(true, misValue, misValue);
            //xlApp.Quit();

            //Marshal.ReleaseComObject(xlWorkSheet);
            //Marshal.ReleaseComObject(xlWorkBook);
            //Marshal.ReleaseComObject(xlApp);

            //MessageBox.Show("Excel file created !");
        }
    }

}


