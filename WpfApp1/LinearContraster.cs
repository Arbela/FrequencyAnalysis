using System;
using System.Linq;

namespace FrequencyAnalysis
{
    public class LinearContraster
    {
        public LinearContraster() { }

        public double CalculateAverage(int[][] matrix)
        {
            int sum = 0;

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (matrix[i][j] < 0)
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine("row = " + i);
                        Console.WriteLine("column = " + j);
                        Console.WriteLine("x = " + matrix[i][j]);
                    }
                    sum += matrix[i][j];
                }
            }

            return sum / (matrix.Length * matrix[0].Length);
        }

        public double CalculateDispersion(int[][] matrix, double average)
        {
            double sum = 0;

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (matrix[i][j] < 0)
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine("row = " + i);
                        Console.WriteLine("column = " + j);
                        Console.WriteLine("x = " + matrix[i][j]);
                    }
                    sum += (matrix[i][j] - average) * (matrix[i][j] - average);
                }
            }

            return sum / (matrix.Length * matrix[0].Length);
        }

        public double CalculateVariationCoefficient(double average, double dispersion)
        {
            return Math.Sqrt(dispersion) / average;
        }

        public T MinVariationCoefficient<T>(T[][] matrix)
        {
            T[] linear = MatrixHelper.GetArrayLinear(matrix);

            return linear.Min();
        }

        public T MaxVariationCoefficient<T>(T[][] matrix)
        {
            T[] linear = MatrixHelper.GetArrayLinear(matrix);

            return linear.Max();
        }

        public (T min, T max) GetMinAndMaxCoefficients<T>(T[][] matrix)
        {
            T[] linear = MatrixHelper.GetArrayLinear(matrix);

            return (linear.Min(), linear.Max());
        }

        public int CalculateLinearContrast(double min, double max, int x)
        {
            return  min == max ? 255 : Convert.ToInt32((x - min) * 255 / (max - min));
        }

        public int[][] BuildLinearContrastMatrix(int[][] source)
        {
            int[][] linearContrastMatrix = new int[source.Length][];

            var variation = GetMinAndMaxCoefficients(source);            

            for (int i = 0; i < source.Length; ++i)
            {
                linearContrastMatrix[i] = new int[source[i].Length];

                for (int j = 0; j < source[i].Length; ++j)
                {                    
                    linearContrastMatrix[i][j] = CalculateLinearContrast(variation.min, variation.max, source[i][j]);
                    if (linearContrastMatrix[i][j] < 0)
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine("min = " + variation.min);
                        Console.WriteLine("max = " + variation.max);
                        Console.WriteLine("x = " + source[i][j]);
                    }
                }
            }

            return linearContrastMatrix;
        }

        public double MeasureBlur(double linearContrast)
        {
            return Math.Pow(linearContrast, -1.086);
        }
    }
}
