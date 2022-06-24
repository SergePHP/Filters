using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Effects;
using Color = System.Drawing.Color;

namespace WpfApp3
{
    public struct RGB
    {
        public float R;
        public float G;
        public float B;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        internal BitmapSource orirginalSourceBitmap = null;
        internal BitmapSource displayedSourceBitmap = null;

        //размытие
        public const int matrixSize = 3;
        public static double[,] blur;
        //public static double[,] blur = new double[matrixSize, matrixSize] {{0.111, 0.111, 0.111},
        //                                                                   {0.111, 0.111, 0.111},
        //                                                                   {0.111, 0.111, 0.111}};
        //public static double[,] blur = new double[matrixSize, matrixSize] {{-1, -1, -1},
        //                                                                   {-1, 16, -1},
        //                                                                   {-1, -1, -1}}; //div = 8
        //public static double[,] blur = new double[matrixSize, matrixSize] {{-1, -1, -1},
        //                                                                   {-1, 9, -1},
        //                                                                   {-1, -1, -1}};
        //public static double[,] blur = new double[matrixSize, matrixSize] {{0, -1, 0},
        //                                                                   {-1, 5, -1},
        //                                                                   {0, -1, 0}};
        //public static double[,] blur = new double[matrixSize, matrixSize] {{1, 2, 1},
        //                                                                   {2, 4, 2},
        //                                                                   {1, 2, 1}};




        public MainWindow()
        {
            InitializeComponent();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                orirginalSourceBitmap = new BitmapImage(new Uri(dlg.FileName));
                displayedSourceBitmap = orirginalSourceBitmap;
                image.Effect = null;
                label.Visibility = Visibility.Hidden;
                image.Source = displayedSourceBitmap;
            }
        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Rotate_Click(object sender, RoutedEventArgs e)
        {

            int width = displayedSourceBitmap.PixelWidth;
            int height = displayedSourceBitmap.PixelHeight;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            UInt32[] pixelsArray = new UInt32[width * height];
            displayedSourceBitmap.CopyPixels(pixelsArray, stride, 0);

            UInt32[] transformedPixelsArray = new UInt32[pixelsArray.Length]; ;

            int newWidth = height;
            int newHeight = width;

            var newStride = newWidth * bytesPerPixel;

            Button button = sender as Button;
            if (button.Name.Equals("rotate90"))
            {
                for (int y = 0, i = width - 1; y < newHeight; y++, i--)
                {
                    for (int x = 0; x < newWidth; x++)
                    {
                        transformedPixelsArray[y * newWidth + x] = pixelsArray[width * x + i];
                    }
                }
            }
            else
            {
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++)
                    {
                        transformedPixelsArray[y * newWidth + x] = pixelsArray[(height - x - 1) * width + y];
                    }
                }
            }

            displayedSourceBitmap = BitmapSource.Create(newWidth, newHeight, displayedSourceBitmap.DpiY, 
                displayedSourceBitmap.DpiX, displayedSourceBitmap.Format, null, transformedPixelsArray, newStride);
            image.Source = displayedSourceBitmap;
        }
        private void Mirror_Click(object sender, RoutedEventArgs e)
        {
            int width = displayedSourceBitmap.PixelWidth;
            int height = displayedSourceBitmap.PixelHeight;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            UInt32[] pixelsArray = new UInt32[width * height];
            displayedSourceBitmap.CopyPixels(pixelsArray, stride, 0);

            UInt32[] transformedPixelsArray = new UInt32[pixelsArray.Length];

            Button button = sender as Button;
            if (button.Name.Equals("mirrorv"))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0, i = width; x < width; x++, i--)
                    {
                        transformedPixelsArray[y * width + x] = pixelsArray[(y * width - 1) + i];
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        transformedPixelsArray[y * width + x] = pixelsArray[(height - y - 1) * width + x];
                    }
                }
            }

            displayedSourceBitmap = BitmapSource.Create(width, height, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, transformedPixelsArray, stride);
            image.Source = displayedSourceBitmap;
        }
        private void Inversion_Click(object sender, RoutedEventArgs e)
        {
            int width = displayedSourceBitmap.PixelWidth;
            int height = displayedSourceBitmap.PixelHeight;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            UInt32[] pixelsArray = new UInt32[width * height];
            displayedSourceBitmap.CopyPixels(pixelsArray, stride, 0);

            UInt32[] transformedPixelsArray = new UInt32[pixelsArray.Length];

            for (int i = 0; i < transformedPixelsArray.Length; i++)
            {
                transformedPixelsArray[i] = ~pixelsArray[i];
            }

            displayedSourceBitmap = BitmapSource.Create(width, height, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, transformedPixelsArray, stride);
            image.Source = displayedSourceBitmap;
        }
        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            //BlurEffect myBlur = new BlurEffect();
            //myBlur.Radius += 5;
            //myBlur.KernelType = KernelType.Gaussian;
            //image.Effect = myBlur;

            //GussianBlur();
            //return;

            if (displayedSourceBitmap.Format != PixelFormats.Bgra32)
                displayedSourceBitmap = new FormatConvertedBitmap(displayedSourceBitmap, PixelFormats.Bgra32, null, 0);

            if (blur == null)
            {
                blur = GetMatrix(0.6, 5);
            }
            int matrixSize = blur.GetUpperBound(0);

            int height = displayedSourceBitmap.PixelHeight;
            int width = displayedSourceBitmap.PixelWidth;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            UInt32[] pixelsArray = new UInt32[width * height];
            displayedSourceBitmap.CopyPixels(pixelsArray, stride, 0);

            UInt32[,] pixelsMatrix = new UInt32[height, width];
            UInt32[,] transformedPixelsMatrix = new UInt32[height, width];

            int index = 0;
            for (int x = 0; x < height; x++)
                for (int y = 0; y < width; y++, index++)
                    pixelsMatrix[x, y] = pixelsArray[index];
            // ---------------------------------------------------------------------------------
            int i, j, k, m, gap = matrixSize / 2;
            int tmpH = height + 2 * gap, tmpW = width + 2 * gap;
            UInt32[,] tmppixel = new UInt32[tmpH, tmpW];

            //заполнение временного расширенного изображения
            //углы
            for (i = 0; i < gap; i++)
                for (j = 0; j < gap; j++)
                {
                    tmppixel[i, j] = pixelsMatrix[0, 0];
                    tmppixel[i, tmpW - 1 - j] = pixelsMatrix[0, width - 1];
                    tmppixel[tmpH - 1 - i, j] = pixelsMatrix[height - 1, 0];
                    tmppixel[tmpH - 1 - i, tmpW - 1 - j] = pixelsMatrix[height - 1, width - 1];
                }
            //крайние левая и правая стороны
            for (i = gap; i < tmpH - gap; i++)
                for (j = 0; j < gap; j++)
                {
                    tmppixel[i, j] = pixelsMatrix[i - gap, j];
                    tmppixel[i, tmpW - 1 - j] = pixelsMatrix[i - gap, width - 1 - j];
                }
            //крайние верхняя и нижняя стороны
            for (i = 0; i < gap; i++)
                for (j = gap; j < tmpW - gap; j++)
                {
                    tmppixel[i, j] = pixelsMatrix[i, j - gap];
                    tmppixel[tmpH - 1 - i, j] = pixelsMatrix[height - 1 - i, j - gap];
                }
            //центр
            for (i = 0; i < height; i++)
                for (j = 0; j < width; j++)
                    tmppixel[i + gap, j + gap] = pixelsMatrix[i, j];

            //применение ядра свертки

            for (i = gap; i < tmpH - gap; i++)
                for (j = gap; j < tmpW - gap; j++)
                {
                    double[] sum = new double[4];
                    for (k = 0; k < matrixSize; k++)
                        for (m = 0; m < matrixSize; m++)
                        {
                            Color color = Color.FromArgb((int)tmppixel[i - gap + k, j - gap + m]);
                            sum[0] = sum[0] + color.A * blur[k, m];
                            sum[1] = sum[1] + color.R * blur[k, m];
                            sum[2] = sum[2] + color.G * blur[k, m];
                            sum[3] = sum[3] + color.B * blur[k, m];
                        }
                    //for (int z = 0; z < sum.Length; z++)
                    //    sum[z] /= 16;

                    if (sum[0] < 0) sum[0] = 0;
                    if (sum[0] > 255) sum[0] = 255;
                    if (sum[1] < 0) sum[1] = 0;
                    if (sum[1] > 255) sum[1] = 255;
                    if (sum[2] < 0) sum[2] = 0;
                    if (sum[2] > 255) sum[2] = 255;
                    if (sum[3] < 0) sum[3] = 0;
                    if (sum[3] > 255) sum[3] = 255;

                    Color rc = Color.FromArgb((int)sum[0], (int)sum[1], (int)sum[2], (int)sum[3]);
                    transformedPixelsMatrix[i - gap, j - gap] = (UInt32)rc.ToArgb();
                }

            index = 0;
            for (int x = 0; x < height; x++ )
                for (int y = 0; y < width; y++, index++)
                {
                    pixelsArray[index] = transformedPixelsMatrix[x, y];
                }

            displayedSourceBitmap = BitmapSource.Create(width, height, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, pixelsArray, stride);
            image.Source = displayedSourceBitmap;
        }

        public static double[,] GetMatrix(double sigma, int W)
        {
            double[,] kernel = new double[W, W];
            double mean = W / 2;
            double sum = 0.0; // For accumulating the kernel values

            for (int x = 0; x < W; ++x) 
                for (int y = 0; y < W; ++y) {

                    kernel[x, y] = Math.Exp(-0.5 * (Math.Pow((x - mean) / sigma, 2.0) + Math.Pow((y - mean) / sigma, 2.0)))
                                 / (2 * Math.PI * sigma * sigma);

                    // Accumulate the kernel values
                    sum += kernel[x, y];
            }

            // Normalize the kernel
            for (int x = 0; x < W; ++x) 
                for (int y = 0; y < W; ++y)
                    kernel[x, y] /= sum;

            return kernel;
        }

        private static int sigma = 4;
        private static int radius = 3 * sigma;
        private static double[] kernel = new double[radius + 1];

        public void GussianBlur()
        {
            initKernel();

            if (displayedSourceBitmap.Format != PixelFormats.Bgra32)
                displayedSourceBitmap = new FormatConvertedBitmap(displayedSourceBitmap, PixelFormats.Bgra32, null, 0);

            int h = displayedSourceBitmap.PixelHeight;
            int w = displayedSourceBitmap.PixelWidth;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = w * bytesPerPixel;

            byte[] bytes = new byte[h * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            int[] pix = new int[bytes.Length / bytesPerPixel];
            for (int i = 0, j = 0; i < bytes.Length; i += bytesPerPixel, j++)
            {
                pix[j] = BitConverter.ToInt32(bytes, i);
            }

            int[] tmp = new int[bytes.Length / bytesPerPixel];
            int[] result = new int[bytes.Length / bytesPerPixel];

            for (int i = 0; i < w * h; i++)
            {
                int x = i % w;
                int y = i / w;
                double[] sum = new double[4];
                for (int j = -radius; j <= radius; j++)
                {
                    int currentX = Math.Min(Math.Max(x + j, 0), w - 1);
                    int index = y * w + currentX;
                    Color color = Color.FromArgb(pix[index]);
                    sum[0] = sum[0] + color.A * kernel[Math.Abs(j)];
                    sum[1] = sum[1] + color.R * kernel[Math.Abs(j)];
                    sum[2] = sum[2] + color.G * kernel[Math.Abs(j)];
                    sum[3] = sum[3] + color.B * kernel[Math.Abs(j)];
                }
                Color rc = Color.FromArgb((int)sum[0], (int)sum[1], (int)sum[2], (int)sum[3]);
                tmp[i] = rc.ToArgb();
            }
            for (int i = 0; i < w * h; i++)
            {
                int x = i % w;
                int y = i / w;
                double[] sum = new double[4];
                for (int j = -radius; j <= radius; j++)
                {
                    int currentY = Math.Min(Math.Max(y + j, 0), h - 1);
                    int index = currentY * w + x;
                    Color color = Color.FromArgb(tmp[index]);
                    sum[0] = sum[0] + color.A * kernel[Math.Abs(j)];
                    sum[1] = sum[1] + color.R * kernel[Math.Abs(j)];
                    sum[2] = sum[2] + color.G * kernel[Math.Abs(j)];
                    sum[3] = sum[3] + color.B * kernel[Math.Abs(j)];
                }
                Color rc = Color.FromArgb((int)sum[0], (int)sum[1], (int)sum[2], (int)sum[3]);
                result[i] = rc.ToArgb();
            }
            byte[] transformedBytes = new byte[h * stride];
            for (int i = 0, j = 0; i < result.Length; i++, j += bytesPerPixel)
            {
                transformedBytes[j] = (byte)result[i];
                transformedBytes[j + 1] = (byte)(result[i] >> 8);
                transformedBytes[j + 2] = (byte)(result[i] >> 16);
                transformedBytes[j + 3] = (byte)(result[i] >> 24);
            }
            displayedSourceBitmap = BitmapSource.Create(w, h, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, transformedBytes, stride);
            image.Source = displayedSourceBitmap;
        }
        private void initKernel()
        {
            double sum = 0.0;
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] = 1 / Math.Sqrt(2 * Math.PI) * Math.Exp(-(i * i * 1.0) / (2.0 * sigma * sigma)) / sigma;
                if (i > 0)
                {
                    sum = sum + kernel[i] * 2.0;
                }
                else
                {
                    sum = sum + kernel[i];
                }
            }
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] = kernel[i] / sum;
            }
        }

        private void Sharp_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Median_Click(object sender, RoutedEventArgs e)
        {
            if (displayedSourceBitmap.Format != PixelFormats.Bgra32)
                displayedSourceBitmap = new FormatConvertedBitmap(displayedSourceBitmap, PixelFormats.Bgra32, null, 0);

            int height = displayedSourceBitmap.PixelHeight;
            int width = displayedSourceBitmap.PixelWidth;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            UInt32[] pixelsArray = new UInt32[width * height];
            displayedSourceBitmap.CopyPixels(pixelsArray, stride, 0);

            UInt32[,] pixelsMatrix = new UInt32[height, width];
            UInt32[,] transformedPixelsMatrix = new UInt32[height, width];

            int index = 0;
            for (int x = 0; x < height; x++)
                for (int y = 0; y < width; y++, index++)
                    pixelsMatrix[x, y] = pixelsArray[index];
            // ---------------------------------------------------------------------------------
            int i, j, k, m, gap = (int)(matrixSize / 2);
            int tmpH = height + 2 * gap, tmpW = width + 2 * gap;
            UInt32[,] tmppixel = new UInt32[tmpH, tmpW];

            //заполнение временного расширенного изображения
            //углы
            for (i = 0; i < gap; i++)
                for (j = 0; j < gap; j++)
                {
                    tmppixel[i, j] = pixelsMatrix[0, 0];
                    tmppixel[i, tmpW - 1 - j] = pixelsMatrix[0, width - 1];
                    tmppixel[tmpH - 1 - i, j] = pixelsMatrix[height - 1, 0];
                    tmppixel[tmpH - 1 - i, tmpW - 1 - j] = pixelsMatrix[height - 1, width - 1];
                }
            //крайние левая и правая стороны
            for (i = gap; i < tmpH - gap; i++)
                for (j = 0; j < gap; j++)
                {
                    tmppixel[i, j] = pixelsMatrix[i - gap, j];
                    tmppixel[i, tmpW - 1 - j] = pixelsMatrix[i - gap, width - 1 - j];
                }
            //крайние верхняя и нижняя стороны
            for (i = 0; i < gap; i++)
                for (j = gap; j < tmpW - gap; j++)
                {
                    tmppixel[i, j] = pixelsMatrix[i, j - gap];
                    tmppixel[tmpH - 1 - i, j] = pixelsMatrix[height - 1 - i, j - gap];
                }
            //центр
            for (i = 0; i < height; i++)
                for (j = 0; j < width; j++)
                    tmppixel[i + gap, j + gap] = pixelsMatrix[i, j];
            //применение ядра свертки

            int z;
            for (i = gap; i < tmpH - gap; i++)
                for (j = gap; j < tmpW - gap; j++)
                {
                    UInt32[] array1 = new UInt32[matrixSize * matrixSize];
                    for (k = 0, z = 0; k < matrixSize; k++)
                        for (m = 0; m < matrixSize; m++, z++)
                        {
                            array1[z] = tmppixel[i - gap + k, j - gap + m];
                        }
                    array1 = SortArray(array1);

                    transformedPixelsMatrix[i - gap, j - gap] = array1[array1.Length / 2];
                }
            index = 0;
            for (int x = 0; x < height; x++)
                for (int y = 0; y < width; y++, index++)
                {
                    pixelsArray[index] = transformedPixelsMatrix[x, y];
                }

            displayedSourceBitmap = BitmapSource.Create(width, height, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, pixelsArray, stride);
            image.Source = displayedSourceBitmap;
        }
        private UInt32[] SortArray(UInt32[] arr)
        {
            UInt32 temp;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[i] > arr[j])
                    {
                        temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }
            }
            return arr;
        }
    }
}
