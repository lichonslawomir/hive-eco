using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

namespace BeeHive.Domain.Hives.Audio;

public static class AudioExtensions
{
    public static (float durationSec, float frequency, float amplitudePeak, float amplitudeRms, float amplitudeMav)
        GetAdioStreamStats(this IEnumerable<byte> input, int sampleRate, int channels, int bitsPerSample)
    {
        Span<byte> bytes = input.ToArray();
        Span<float> samples;
        if (bitsPerSample == 16)
        {
            int sampleCount = bytes.Length / 2;
            samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var sample = BitConverter.ToInt16(bytes.Slice(i * 2, 2));
                samples[i] = sample / 32768f;
            }
        }
        else if (bitsPerSample == 12)
        {
            int sampleCount = (bytes.Length / 3) * 2;
            samples = new float[sampleCount];
            int outIdx = 0;
            for (int i = 0; i + 2 < bytes.Length; i += 3)
            {
                // First sample: lower 8 bits from first byte, upper 4 bits from second byte
                int sample1 = bytes[i] | ((bytes[i + 1] & 0x0F) << 8);
                // Sign extend if negative
                if ((sample1 & 0x800) != 0) sample1 |= unchecked((int)0xFFFFF000);
                samples[outIdx++] = ((short)sample1) / 2048f;

                // Second sample: lower 4 bits from second byte, upper 8 bits from third byte
                int sample2 = ((bytes[i + 1] >> 4) & 0x0F) | (bytes[i + 2] << 4);
                if ((sample2 & 0x800) != 0) sample2 |= unchecked((int)0xFFFFF000);
                samples[outIdx++] = ((short)sample2) / 2048f;
            }
        }
        else if (bitsPerSample == 8)
        {
            int sampleCount = bytes.Length;
            samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                samples[i] = (bytes[i] - 128) / 128f;
            }
        }
        else
        {
            throw new NotSupportedException($"BitsPerSample: {bitsPerSample}");
        }

        var samplesCount = samples.Length;

        float peak = 0f;
        float sumSq = 0f;
        float sumAbs = 0f;
        Complex[] fftBuffer = new Complex[samplesCount];
        for (int i = 0; i < samplesCount; ++i)
        {
            var s = samples[i];
            float abs = MathF.Abs(s);
            if (abs > peak) peak = abs;
            sumAbs += abs;
            sumSq += s * s;

            fftBuffer[i] = new Complex(samples[i], 0);
        }

        float rms = MathF.Sqrt(sumSq / samples.Length);
        float mav = sumAbs / samples.Length;

        Fourier.Forward(fftBuffer, FourierOptions.Matlab);

        Span<double> magnitudes = new double[samplesCount / 2];
        int maxIndex = -1;
        double maxMagnitude = double.MinValue;
        for (int i = 0; i < magnitudes.Length; i++)
        {
            var m = fftBuffer[i].Magnitude;
            if (m > maxMagnitude)
            {
                maxMagnitude = m;
                maxIndex = i;
            }
            magnitudes[i] = m;
        }
        double sc = samplesCount;
        double sr = sampleRate;
        double freqResolution = sr / sc;
        var dominantFrequency = (float)(maxIndex * freqResolution);
        var durationSec = (float)(sc / (sr * channels));

        return (durationSec, dominantFrequency, peak, rms, mav);
    }
}