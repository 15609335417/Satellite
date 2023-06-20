using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卫星轨道计算
{
    //[Serializable]
    class Function
    {
        ///<summary>
        ///坐标转换（XYZ-BLH）
        /// </summary>
        /// XYZ->BLH
        // 参数应包括地球长半轴a，以及椭球扁率α；
        public static void XYZ_BLH(double a,double alpha, double[] x, double[] y, double[] z, out double[] b, out double[] l,out double[] h)
        {
            double e2 = alpha * 2 - alpha * alpha;
            double B0 = 0;
            double N=0;
            int count = 0;
            b = new double[x.Length];
            l = new double[x.Length];
            h = new double[x.Length];
            for(int i=0;i<x.Length;i++)
            {
                l[i] = Math.Atan(y[i] / x[i]);
                while(count<10)
                {
                    N = a / Math.Sqrt(1 - e2 * Math.Sin(B0) * Math.Sin(B0));
                    b[i] = Math.Atan((z[i] + N * e2 * Math.Sin(B0) * Math.Sin(B0)) / Math.Sqrt(x[i] * x[i] + y[i] * y[i]));
                    B0 = b[i];
                    count++;
                }
                h[i] = Math.Sqrt(x[i] * x[i] + y[i] * y[i]) / Math.Cos(b[i]) - N;
                b[i] = b[i] * 180.0 / Math.PI;
                l[i] = l[i] * 180.0 / Math.PI;
            }
            
        }

        public static void XYZ_BLH(double a,double alpha, double x, double y, double z, out double b, out double l, out double h)
        {
            double e2 = alpha * 2 - alpha * alpha;
            double B0 = 0;
            l = 0;
            b = 0;
            h = 0;
            int count = 0;
            double N = 0;
            l = Math.Atan(y / x);
            while (count < 10)
            {
                N = a / Math.Sqrt(1 - e2 * Math.Sin(B0) * Math.Sin(B0));
                b = Math.Atan((z + N * e2 * Math.Sin(B0) * Math.Sin(B0)) / Math.Sqrt(x * x + y * y));
                B0 = b;
                count++;
            }
            h = Math.Sqrt(x * x + y * y) / Math.Cos(b) - N;
            b = b * 180.0 / Math.PI;
            l = l * 180.0 / Math.PI;
        }

        //无椭球参数输入时，默认为WGS-84椭球
        public static void XYZ_BLH(double[] x, double[] y, double[] z, out double[] b, out double[] l, out double[] h)
        {
            double a = 6378137;
            double alpha = 1 / 298.2572236;
            double e2 = alpha * 2 - alpha * alpha;
            double B0 = 0;
            double N = 0;
            int count = 0;
            b = new double[x.Length];
            l = new double[x.Length];
            h = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                l[i] = Math.Atan(y[i] / x[i]);
                while (count < 10)
                {
                    N = a / Math.Sqrt(1 - e2 * Math.Sin(B0) * Math.Sin(B0));
                    b[i] = Math.Atan((z[i] + N * e2 * Math.Sin(B0) * Math.Sin(B0)) / Math.Sqrt(x[i] * x[i] + y[i] * y[i]));
                    B0 = b[i];
                    count++;
                }
                h[i] = Math.Sqrt(x[i] * x[i] + y[i] * y[i]) / Math.Cos(b[i]) - N;
                b[i] = b[i] * 180.0 / Math.PI;
                l[i] = l[i] * 180.0 / Math.PI;
            }

        }

        public static void XYZ_BLH( double x, double y, double z, out double b, out double l, out double h)
        {
            double a = 6378137;
            double alpha = 1 / 298.2572236;
            double e2 = alpha * 2 - alpha * alpha;
            double B0 = 0;
            l = 0;
            b = 0;
            h = 0;
            int count = 0;
            double N = 0;
            l = Math.Atan(y / x);
            while (count < 10)
            {
                N = a / Math.Sqrt(1 - e2 * Math.Sin(B0) * Math.Sin(B0));
                b = Math.Atan((z + N * e2 * Math.Sin(B0) * Math.Sin(B0)) / Math.Sqrt(x * x + y * y));
                B0 = b;
                count++;
            }
            h = Math.Sqrt(x * x + y * y) / Math.Cos(b) - N;
            b = b * 180.0 / Math.PI;
            l = l * 180.0 / Math.PI;
        }

        ///<summary>
        ///时间格式转换
        /// </summary>
        /// 1. 时分秒-秒
        ///     
        public static void  HMS2S(double hour,double minute,double second,out int s)
        {
            s = Convert.ToInt32(hour * 3600 + minute * 60 + second);
        }
        ///2.秒-时分秒
        ///
        ///
        public static void S2HMS(int s,out double  hour,out double minute,out double second)
        {
            hour = s / 3600;
            minute = s % 3600 / 60;
            second = s  % 60;
        }
    }
}
