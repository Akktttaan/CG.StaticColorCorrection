using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ColorCorrection;
using Microsoft.Win32;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выбрать изображение источник
        /// </summary>
        private void SetSourceImage(object sender, RoutedEventArgs e)
        {
            // Create an instance of the OpenFileDialog class
            var openFileDialog = new OpenFileDialog
            {
                // Set the file filter to allow only image files
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            // If the user selected a file, load it into a Bitmap object and display it in an Image control
            if (openFileDialog.ShowDialog() != true) return;

            // Load the image file into a Bitmap object
            var bitmap = new Bitmap(openFileDialog.FileName);

            BitmapHelper.SourceImg = bitmap;
            labCorrection.Source = null;
            hslCorrection.Source = null;

            // Set the Source property of an Image control to the bitmap object
            sourceImg.Source = Imaging
                .CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Выбрать целевое изображение
        /// </summary>
        private void SetDestinationImage(object sender, RoutedEventArgs e)
        {
            // Create an instance of the OpenFileDialog class
            var openFileDialog = new OpenFileDialog
            {
                // Set the file filter to allow only image files
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            // If the user selected a file, load it into a Bitmap object and display it in an Image control
            if (openFileDialog.ShowDialog() != true) return;

            // Load the image file into a Bitmap object
            var bitmap = new Bitmap(openFileDialog.FileName);

            BitmapHelper.DestinationImg = bitmap;
            labCorrection.Source = null;
            hslCorrection.Source = null;

            // Set the Source property of an Image control to the bitmap object
            destinationImg.Source = Imaging
                .CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Применить цветовую коррекцию с помощью lab
        /// </summary>
        private void MakeColorCorrectionInLab(object sender, RoutedEventArgs e)
        {
            if (BitmapHelper.SourceImg is null || BitmapHelper.DestinationImg is null) return;
            double customContrast;
            if (double.TryParse(contrastRatio.Text, out customContrast) && contrastRatio.Visibility == Visibility.Visible)
                BitmapHelper.CustomContrast = customContrast;
            else
                BitmapHelper.CustomContrast = null;

            var bitmap = ColorCorrection.ColorCorrection.MakeColorCorrection(CorrectionType.LAB);

            labCorrection.Source = Imaging
                .CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Применить цветовую коррекцию с помощью HSL
        /// </summary>
        private void MakeColorCorrectionInHsl(object sender, RoutedEventArgs e)
        {
            if (BitmapHelper.SourceImg is null || BitmapHelper.DestinationImg is null) return;
            double customContrast;
            if (double.TryParse(contrastRatio.Text, out customContrast) && contrastRatio.Visibility == Visibility.Visible)
                BitmapHelper.CustomContrast = customContrast;
            else
                BitmapHelper.CustomContrast = null;

            var bitmap = ColorCorrection.ColorCorrection.MakeColorCorrection(CorrectionType.HSL);

            hslCorrection.Source = Imaging
                .CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Действие на изменения значения чекбокса о своем контрасте
        /// </summary>
        private void CustomContrastChecked(object sender, RoutedEventArgs e)
        {
            contrastRatio.Visibility = Visibility.Visible;
            labelContrastRatio.Visibility = Visibility.Visible;

            double customContrast;
            if (double.TryParse(contrastRatio.Text, out customContrast))
                BitmapHelper.CustomContrast = customContrast;
        }

        /// <summary>
        /// Действие на изменения значения чекбокса о своем контрасте
        /// </summary>
        private void CustomContrastUnchecked(object sender, RoutedEventArgs e)
        {
            contrastRatio.Visibility = Visibility.Collapsed;
            labelContrastRatio.Visibility = Visibility.Collapsed;

            BitmapHelper.CustomContrast = null;
            contrastRatio.Text = "1,0";
        }
    }
}