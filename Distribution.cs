using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_LW1
{
    class Distribution
    {
        public static Random random = new Random();

        //Реализация Экспоненциальной СВ с параметром Лямбда
        public static double Exponential(double lambda)
        {
            return -(1 / lambda) * Math.Log(1 - random.NextDouble());
        }
        //Реализация Эрланговской СВ с параметрами l и Лямбда
        public static double Gamma(int length, double lambda)
        {
            double sum = 0D;

            for (int i = 0; i < length; i++)
            {
                sum += Exponential(lambda);
            }

            return sum;
        }
        //Реализация Нормальной СВ с параметрами μ и σ : Метод полярных координат
        public static double Normal(double mx, double sigma)
        {
            while (true)
            {
                double w1 = 2.0 * random.NextDouble() - 1.0;
                double w2 = 2.0 * random.NextDouble() - 1.0;
                double w = Math.Pow(w1, 2) + Math.Pow(w2, 2);

                if (w <= 1)
                {
                    double c = Math.Sqrt(-2.0 * Math.Log(w) / w);

                    return w1 * c * sigma + mx;
                }
            }
        }
        //Реализация Пуассоновской СВ с параметром Лямбда
        public static double Poisson(double lambda)
        {
            return Exponential(lambda);
        }
    }
}
