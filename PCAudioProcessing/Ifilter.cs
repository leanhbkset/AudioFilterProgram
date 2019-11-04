using System;

namespace PCAudioProcessing
{
    public class Ifilter
    {

        public int _nHz;

        public int _nBitsPerSample;

        public int _nBytesPerSec;

        public int _nChannels; // mono = 1, stereo = 2

        public enum Domain

        {

            TimeDomain = 1,

            FreqDomain = 2

        };



        Domain currentDomain = Domain.TimeDomain;

        public int _np2; // nearest power of 2

        public float[] _aSamples;

        public float[] _aImaginary;



        public void FFT(bool fInverse)

        {
            var n = _aSamples.Length;

            var nlg2 = (int)(Math.Log(n) / Math.Log(2));

            {

                var j = n / 2;

                if (fInverse)

                {

                    for (int i = 0; i < n; i++)

                    {

                        _aImaginary[i] = -_aImaginary[i];

                    }

                }

                for (int i = 1; i < n - 2; i++) // Bit Reversal order

                {

                    if (i < j)

                    {
                        swap(ref _aSamples[j], ref _aSamples[i]);

                        swap(ref _aImaginary[j], ref _aImaginary[i]);

                    }

                    var k = n / 2;

                    while (k <= j)

                    {

                        j -= k;

                        k /= 2;

                    }

                    j += k;

                }

            }

            var le2 = 1;

            for (int lp = 0; lp < nlg2; lp++)

            {

                var le = 2 * le2;

                var ur = 1.0f;

                var ui = 0.0f;

                var sr = Math.Cos(Math.PI / le2);

                var si = -Math.Sin(Math.PI / le2);

                float tr;

                float ti;

                for (int j = 0; j < le2; j++) // each sub DFT

                {

                    for (int i = j; i < n; i += le) // butterfly loop: cross multiply and accumulate

                    {

                        var ip = i + le2;

                        tr = _aSamples[ip] * ur - _aImaginary[ip] * ui;

                        ti = _aSamples[ip] * ui + _aImaginary[ip] * ur;

                        _aSamples[ip] = _aSamples[i] - tr;

                        _aImaginary[ip] = _aImaginary[i] - ti;

                        _aSamples[i] = _aSamples[i] + tr;

                        _aImaginary[i] = _aImaginary[i] + ti;

                    }

                    tr = ur;

                    ur = (float)(tr * sr - ui * si);

                    ui = (float)(tr * si + ui * sr);

                }

                le2 *= 2;

            }

            if (fInverse)

            {

                for (int i = 0; i < n; i++)

                {

                    _aSamples[i] = _aSamples[i] / n;

                    _aImaginary[i] = -_aImaginary[i] / n;

                }

                currentDomain = Domain.TimeDomain;

            }

            else

            {

                currentDomain = Domain.FreqDomain;

            }

        }



        public void EnsureDomain(Domain toDomin)

        {

            if (currentDomain != toDomin)

            {

                if (currentDomain == Domain.TimeDomain)

                {

                    FFT(fInverse: false);

                }

                else

                {

                    FFT(fInverse: true);

                }

            }

        }

        public void NotchFilter(double nFreqNotch, int nNotchWidth)

        {

            EnsureDomain(Domain.FreqDomain);

            // the Sine wave freq is 2048 hz, so we want the filter to be centered on that

            int nMid = (int)(nFreqNotch / _nHz * _np2);

            var nRange = nNotchWidth;

            for (int i = nMid - nRange; i < nMid + nRange; i++)

            {  // we want to set all values in the range to 0

                if (i >= 0 && i < _aSamples.Length)

                {

                    _aSamples[i] = 0;

                    _aImaginary[i] = 0;

                    _aSamples[_np2 - i] = 0;

                    _aImaginary[_np2 - i] = 0;

                }

            }

        }

        // Bandpass filter
        public void BandpassFilter(double nFreqNotch, int nNotchWidth)

        {

            EnsureDomain(Domain.FreqDomain);

            // the Sine wave freq is 2048 hz, so we want the filter to be centered on that

            int nMid = (int)(nFreqNotch / _nHz * _np2);


            var nRange = nNotchWidth;

            for (int i = 0; i < nMid - nRange; i++)

            {  // we want to set all values in the range to 0

                if (i >= 0 && i < _aSamples.Length)

                {

                    _aSamples[i] = 0;

                    _aImaginary[i] = 0;

                }

            }

            for (int i = nMid + nRange; i < _aSamples.Length; i++)

            {  // we want to set all values in the range to 0

                if (i >= 0 && i < _aSamples.Length)

                {

                    _aSamples[i] = 0;

                    _aImaginary[i] = 0;

                }

            }

        }

        private void swap<T>(ref T a, ref T b)

        {

            T tmp;

            tmp = a;

            a = b;

            b = tmp;

        }

    }
}
