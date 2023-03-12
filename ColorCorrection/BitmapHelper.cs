using System.Drawing;

namespace ColorCorrection;

/// <summary>
/// Класс методов для Bitmap
/// </summary>
public static class BitmapHelper
{
    /// <summary>
    /// Изображение источник
    /// </summary>
    public static Bitmap SourceImg { get; set; }

    /// <summary>
    /// Изображение целевое
    /// </summary>
    public static Bitmap DestinationImg { get; set; }

    /// <summary>
    /// Контраст введенной вручную (1.0 по умолчанию)
    /// </summary>
    public static double? CustomContrast { get; set; }

    /// <summary>
    /// Получить нормализованные значения РГБ из изображения
    /// </summary>
    public static double[,] GetNormalizeGrbFromBitmap(Bitmap image)
    {
        var coef = 235.0 / 255.0;
        var imageHeight = image.Height;
        var imageWidth = image.Width;
        var pixelsWithRgb = new double[imageHeight * imageWidth, 3];

        double CheckMinValue(double value) => value < 3.0 / 255.0 ? 3.0 / 255.0 : value;

        var counter = 0;
        for (var y = 0; y < imageHeight; y++)
        {
            for (var x = 0; x < imageWidth; x++)
            {
                var pixel = image.GetPixel(x, y);
                pixelsWithRgb[counter, 0] = CheckMinValue(pixel.R / 255.0) * coef;
                pixelsWithRgb[counter, 1] = CheckMinValue(pixel.G / 255.0) * coef;
                pixelsWithRgb[counter, 2] = CheckMinValue(pixel.B / 255.0) * coef;
                counter++;
            }
        }

        return pixelsWithRgb;
    }

    /// <summary>
    /// Получить из ргб значений Bitmap
    /// </summary>
    public static Bitmap GetBitmapFromNormalizeGrb(double[,] rgbValues, int width, int height)
    {
        var coef = 235.0 / 255.0;
        var bitmap = new Bitmap(width, height);

        int ValidateRgbValue(int value)
        {
            return value switch
            {
                > 255 => 255,
                < 0 => 0,
                _ => value
            };
        }

        var counter = 0;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var color = Color.FromArgb(ValidateRgbValue((int)(rgbValues[counter, 0] * 255.0 / coef)),
                    ValidateRgbValue((int)(rgbValues[counter, 1] * 255.0 / coef)),
                    ValidateRgbValue((int)(rgbValues[counter, 2] * 255.0 / coef)));
                bitmap.SetPixel(x, y, color);
                counter++;
            }
        }

        return bitmap;
    }
}