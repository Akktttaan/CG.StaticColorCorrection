using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using static System.Double;

namespace ColorCorrection;

/// <summary>
/// Класс для конвертации
/// </summary>
public static class Converter
{
    /// <summary>
    /// Перевод из RGB в LMS
    /// </summary>
    public static double[,] ConvertRgbToLms(double[,] rgbValues)
    {
        Matrix<double> coef = DenseMatrix.OfArray(new[,]
        {
            { 0.3811, 0.5783, 0.0402 },
            { 0.1967, 0.7244, 0.0782 },
            { 0.0241, 0.1288, 0.8444 }
        });

        return MultiplyMatrixOnArrayVector(coef, rgbValues);
    }

    /// <summary>
    /// Перевод из LMS в lab
    /// </summary>
    public static double[,] ConvertLmsToLab(double[,] lmsValues)
    {
        var coef = DenseMatrix.OfArray(new[,]
        {
            { 0.5774, 0, 0 },
            { 0, 0.4082, 0 },
            { 0, 0, 0.7071 }
        }) * DenseMatrix.OfArray(new[,]
        {
            { 1.0, 1.0, 1.0 },
            { 1.0, 1.0, -2.0 },
            { 1.0, -1.0, 0.0 }
        });

        var numRows = lmsValues.GetLength(0);
        var numColumns = lmsValues.GetLength(1);

        var labValues = new double[numRows, numColumns];
        for (var i = 0; i < numRows; i++)
        {
            var lmsValue = DenseVector.OfArray(new[]
                { Math.Log10(lmsValues[i, 0]), Math.Log10(lmsValues[i, 1]), Math.Log10(lmsValues[i, 2]) });
            var result = coef * lmsValue;
            labValues[i, 0] = result[0];
            labValues[i, 1] = result[1];
            labValues[i, 2] = result[2];
        }

        return labValues;
    }

    /// <summary>
    /// Перевод из lab в LMS
    /// </summary>
    public static double[,] ConvertLabToLms(double[,] labValues)
    {
        var coef = DenseMatrix.OfArray(new[,]
        {
            { 1.0, 1.0, 1.0 },
            { 1.0, 1.0, -1.0 },
            { 1.0, -2.0, 0.0 }
        }) * DenseMatrix.OfArray(new[,]
        {
            { 0.5774, 0.0, 0.0 },
            { 0.0, 0.4082, 0.0 },
            { 0.0, 0.0, 0.7071 }
        });

        var result = MultiplyMatrixOnArrayVector(coef, labValues);

        var lmsValues = new double[labValues.GetLength(0), labValues.GetLength(1)];

        for (var i = 0; i < labValues.GetLength(0); i++)
        {
            lmsValues[i, 0] = Math.Pow(10, result[i, 0]);
            lmsValues[i, 1] = Math.Pow(10, result[i, 1]);
            lmsValues[i, 2] = Math.Pow(10, result[i, 2]);
        }

        return lmsValues;
    }

    public static double[,] ConvertLmsToRgb(double[,] rgbValues)
    {
        var coef = DenseMatrix.OfArray(new[,]
        {
            { 4.4679, -3.5873, 0.1193 },
            { -1.2186, 2.3809, -0.1624 },
            { 0.0497, -0.2439, 1.2045 }
        });

        return MultiplyMatrixOnArrayVector(coef, rgbValues);
    }

    /// <summary>
    /// Умножение матрицы коефицентов на массив векторов
    /// </summary>
    private static double[,] MultiplyMatrixOnArrayVector(
        Matrix<double> coef, double[,] array)
    {
        var numRows = array.GetLength(0);
        var numColumns = array.GetLength(1);

        var results = new double[numRows, numColumns];
        for (var i = 0; i < numRows; i++)
        {
            var vector = DenseVector.OfArray(new[]
                { array[i, 0], array[i, 1], array[i, 2] });
            var result = coef * vector;
            results[i, 0] = result[0];
            results[i, 1] = result[1];
            results[i, 2] = result[2];
        }

        return results;
    }

    /// <summary>
    /// Перевод из массива RGB значений в HSL массив
    /// </summary>
    public static double[,] ConvertRgbToHsl(double[,] rgbValues)
    {
        var hslValues = new double[rgbValues.GetLength(0), rgbValues.GetLength(1)];

        for (var i = 0; i < rgbValues.GetLength(0); i++)
        {
            var rgbToHsl = RgbToHsl(rgbValues[i, 0], rgbValues[i, 1], rgbValues[i, 2]);

            hslValues[i, 0] = rgbToHsl.Item1;
            hslValues[i, 1] = rgbToHsl.Item2;
            hslValues[i, 2] = rgbToHsl.Item3;
        }

        return hslValues;
    }

    /// <summary>
    /// Перевод из RGB в HSL
    /// </summary>
    private static Tuple<double, double, double> RgbToHsl(double r, double g, double b)
    {
        var list = new List<double>() { r, g, b };
        var max = list.Max();
        var min = list.Min();

        double h, s, l;
        l = 0.5 * (max + min);

        if (Math.Abs(max - min) < 0.01) h = 0;
        if (Math.Abs(max - r) < 0.01 && g >= b) h = 60 * ((g - b) / (max - min)) + 0;
        else h = 60 * ((g - b) / (max - min)) + 360;
        if (Math.Abs(max - g) < 0.01) h = 60 * ((b - r) / (max - min)) + 120;
        if (Math.Abs(max - b) < 0.01) h = 60 * ((r - g) / (max - min)) + 240;

        // if (l = 0 || max == min) s = 0;
        // if (l is > 0 and <= 0.5) s = (max - min) / 2 * l;
        // if (l is > 0.5 and < 1) s = (max - min) / 2 - 2 * l;

        s = (max - min) / (1 - Math.Abs(1 - (max + min)));
        if (h is NaN) h = 0;

        return new Tuple<double, double, double>(h, s, l);
    }

    /// <summary>
    /// Перевод массива HSL значений в массив RGB
    /// </summary>
    /// <param name="hslValues"></param>
    /// <returns></returns>
    public static double[,] ConvertHslToRgb(double[,] hslValues)
    {
        var rgbValues = new double[hslValues.GetLength(0), hslValues.GetLength(1)];

        for (var i = 0; i < hslValues.GetLength(0); i++)
        {
            var hslToRgb = HslToRgb(hslValues[i, 0],hslValues[i, 1],hslValues[i, 2]);

            rgbValues[i, 0] = hslToRgb.Item1;
            rgbValues[i, 1] = hslToRgb.Item2;
            rgbValues[i, 2] = hslToRgb.Item3;
        }

        return rgbValues;
    }

    /// <summary>
    /// Перевод из HSL в RGB
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="l"></param>
    /// <returns></returns>
    private static Tuple<double, double, double> HslToRgb(double h, double s, double l)
    {
        double q, p, hk;
        if (l < 0.5) q = l * (1.0 + s);
        else q = l + s - l * s;
        p = 2.0 * l - q;
        hk = h / 360;
        double tr, tg, tb;
        tr = hk + 1.0 / 3.0;
        tg = hk;
        tb = hk - 1.0 / 3.0;

        return new Tuple<double, double, double>(FuncForHstToRgb(tr, p, q),
            FuncForHstToRgb(tg, p, q), FuncForHstToRgb(tb, p, q));
    }

    /// <summary>
    /// Вспомогательная функция для перевода из HSL в RGB
    /// </summary>
    private static double FuncForHstToRgb(double t, double p, double q)
    {
        if (t < 0) t = t + 1.0;
        if (t > 1) t = t - 1.0;

        var color = t switch
        {
            < 1.0 / 6.0 => p + (q - p) * 6.0 * t,
            >= 1.0 / 6.0 and < 0.5 => q,
            >= 0.5 and < 2.0 / 3.0 => p + (q - p) * (2.0 / 3.0 - t) * 6.0,
            _ => p
        };
        return color;
    }
}