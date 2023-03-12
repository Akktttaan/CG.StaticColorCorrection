using System.Drawing;

namespace ColorCorrection;

/// <summary>
/// Класс для цветовой коррекции
/// </summary>
public static class ColorCorrection
{
    /// <summary>
    /// Применить цветовую коррекцию с помощью lab
    /// </summary>
    public static Bitmap MakeColorCorrection(CorrectionType type)
    {
        var sourceValues = new double[BitmapHelper.SourceImg.Height * BitmapHelper.SourceImg.Width, 3];
        var targetValues = new double[BitmapHelper.DestinationImg.Height * BitmapHelper.DestinationImg.Width, 3];
        if (type == CorrectionType.LAB)
        {
            sourceValues = Converter.ConvertLmsToLab(
                Converter.ConvertRgbToLms(BitmapHelper.GetNormalizeGrbFromBitmap(BitmapHelper.SourceImg)));
            targetValues = Converter.ConvertLmsToLab(
                Converter.ConvertRgbToLms(BitmapHelper.GetNormalizeGrbFromBitmap(BitmapHelper.DestinationImg)));
        }

        else
        {
            sourceValues = Converter.ConvertRgbToHsl(BitmapHelper.GetNormalizeGrbFromBitmap(BitmapHelper.SourceImg));
            targetValues =
                Converter.ConvertRgbToHsl(BitmapHelper.GetNormalizeGrbFromBitmap(BitmapHelper.DestinationImg));
        }

        var sourceMeans = CalcMean(sourceValues);
        var sourceVariance = CalcVariance(sourceValues, sourceMeans);

        var targetMeans = CalcMean(targetValues);
        var targetVariance = CalcVariance(targetValues, targetMeans);

        var contrastChanel1 = sourceVariance.Item1 / targetVariance.Item1;
        var contrastChanel2 = sourceVariance.Item2 / targetVariance.Item2;
        var contrastChanel3 = sourceVariance.Item3 / targetVariance.Item3;

        for (var i = 0; i < targetValues.GetLength(0); i++)
        {
            targetValues[i, 0] = sourceMeans.Item1 +
                                 (targetValues[i, 0] - targetMeans.Item1) * (BitmapHelper.CustomContrast == null
                                     ? contrastChanel1
                                     : BitmapHelper.CustomContrast.Value);
            targetValues[i, 1] = sourceMeans.Item2 +
                                 (targetValues[i, 1] - targetMeans.Item2) * (BitmapHelper.CustomContrast == null
                                     ? contrastChanel2
                                     : BitmapHelper.CustomContrast.Value);
            targetValues[i, 2] = sourceMeans.Item3 +
                                 (targetValues[i, 2] - targetMeans.Item3) * (BitmapHelper.CustomContrast == null
                                     ? contrastChanel3
                                     : BitmapHelper.CustomContrast.Value);
        }

        if (type == CorrectionType.LAB)
            return BitmapHelper
                .GetBitmapFromNormalizeGrb(
                    Converter.ConvertLmsToRgb(
                        Converter.ConvertLabToLms(targetValues)), BitmapHelper.DestinationImg.Width,
                    BitmapHelper.DestinationImg.Height);
        return BitmapHelper
            .GetBitmapFromNormalizeGrb(
                Converter.ConvertHslToRgb(targetValues), BitmapHelper.DestinationImg.Width,
                BitmapHelper.DestinationImg.Height);
    }

    /// <summary>
    /// Посчитать мат ожидание
    /// </summary>
    private static Tuple<double, double, double> CalcMean(double[,] values)
    {
        double chanel1 = 0;
        double chanel2 = 0;
        double chanel3 = 0;

        for (var i = 0; i < values.GetLength(0); i++)
        {
            chanel1 += values[i, 0];
            chanel2 += values[i, 1];
            chanel3 += values[i, 2];
        }

        chanel1 /= values.GetLength(0);
        chanel2 /= values.GetLength(0);
        chanel3 /= values.GetLength(0);

        return new Tuple<double, double, double>(chanel1, chanel2, chanel3);
    }

    /// <summary>
    /// Посчитать дисперсию
    /// </summary>
    private static Tuple<double, double, double> CalcVariance(double[,] values, Tuple<double, double, double> means)
    {
        double chanel1 = 0;
        double chanel2 = 0;
        double chanel3 = 0;

        for (var i = 0; i < values.GetLength(0); i++)
        {
            chanel1 += Math.Pow(values[i, 0] - means.Item1, 2);
            chanel2 += Math.Pow(values[i, 1] - means.Item2, 2);
            chanel3 += Math.Pow(values[i, 2] - means.Item3, 2);
        }

        chanel1 = Math.Sqrt(chanel1 / values.GetLength(0));
        chanel2 = Math.Sqrt(chanel2 / values.GetLength(0));
        chanel3 = Math.Sqrt(chanel3 / values.GetLength(0));

        return new Tuple<double, double, double>(chanel1, chanel2, chanel3);
    }
}