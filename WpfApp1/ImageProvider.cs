using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace FrequencyAnalysis
{
    public class ImageProvider
    {
        public ImageProvider() { }

        public Task CreateGrayscale8Async(string imagePath, string storage)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || string.IsNullOrWhiteSpace(storage)) return Task.CompletedTask;

            Bitmap originalBitmap = new Bitmap(imagePath);

            //if (File.Exists(storage))
            //{
            //    File.Delete(storage);
            //}

            return Task.Run(() =>
            {
                Bitmap bitmap = ToGrayscale(originalBitmap);

                bitmap.Save(storage);
            });
        }

        public unsafe Bitmap ToGrayscale(Bitmap colorBitmap)
        {
            int Width = colorBitmap.Width;
            int Height = colorBitmap.Height;

            Bitmap grayscaleBitmap = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

            grayscaleBitmap.SetResolution(colorBitmap.HorizontalResolution,
                                          colorBitmap.VerticalResolution);

            // Set grayscale palette
            ColorPalette colorPalette = grayscaleBitmap.Palette;
            for (int i = 0; i < colorPalette.Entries.Length; i++)
            {
                colorPalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            grayscaleBitmap.Palette = colorPalette;
            
            // Set grayscale palette
            BitmapData bitmapData = grayscaleBitmap.LockBits(
                new Rectangle(Point.Empty, grayscaleBitmap.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            Byte* pPixel = (Byte*)bitmapData.Scan0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color clr = colorBitmap.GetPixel(x, y);

                    Byte byPixel = (byte)((30 * clr.R + 59 * clr.G + 11 * clr.B) / 100);

                    pPixel[x] = byPixel;
                }

                pPixel += bitmapData.Stride;
            }

            grayscaleBitmap.UnlockBits(bitmapData);

            return grayscaleBitmap;
        }

        public int[][] GetBitmapPixelsMatrix(string bitmapPath)
        {
            Bitmap bitmap = new Bitmap(bitmapPath);
            
            int height = bitmap.Height;
            int width = bitmap.Width;
            int[][] pixelsMatrix = new int[height][];

            for (int i = 0; i < height; i++)
            {
                pixelsMatrix[i] = new int[width];
                for (int j = 0; j < width; j++)
                {
                    pixelsMatrix[i][j] = bitmap.GetPixel(j, i).R;
                }
            }

            return pixelsMatrix;
        }

        public Task ExportBitmapPixelsMatrixAsync(string bitmapPath, Stream stream)
        {
            return Task.Run(() => 
            {
                ExportBitmapPixelsMatrix(bitmapPath, stream);
            });

        }

        protected void ExportBitmapPixelsMatrix(string bitmapPath, Stream stream)
        {
            Bitmap bitmap = new Bitmap(bitmapPath);

            int height = bitmap.Height;
            int width = bitmap.Width;

            using (StreamWriter outputFile = new StreamWriter(stream))
            {
                outputFile.Flush();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        outputFile.Write(bitmap.GetPixel(j, i).R.ToString());
                        outputFile.Write(" ");
                    }
                    outputFile.Write(Environment.NewLine);
                }
            }
        }

        public Bitmap CreateBitmapFromFile(string filePath)
        {
            int width = 0;
            int height = 0;
            int[][] pixelsMatrix;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] heightStrings = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                height = heightStrings.Length;
                width = heightStrings[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;

                pixelsMatrix = new int[height][];

                for (int i = 0; i < height; ++i)
                {
                    pixelsMatrix[i] = new int[width];
                    var line = heightStrings[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < width; ++j)
                    {
                        pixelsMatrix[i][j] = Int32.Parse(line[j]);
                    }
                }
            }

            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    bitmap.SetPixel(j, i, Color.FromArgb(pixelsMatrix[i][j], pixelsMatrix[i][j], pixelsMatrix[i][j]));
                }
            }

            return bitmap;
        }

        public Bitmap CreateBitmapFromPixelMartix(int[][] pixelsMatrix)
        {
            int width = pixelsMatrix[0].Length;
            int height = pixelsMatrix.Length;
            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    bitmap.SetPixel(j, i, Color.FromArgb(pixelsMatrix[i][j], pixelsMatrix[i][j], pixelsMatrix[i][j]));
                }
            }

            return bitmap;
        }
    }
}
