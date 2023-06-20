using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卫星轨道计算
{
    internal class Klobuchar
    {
        public static double klobuchar(double[] alpha, double[] beta)
        {
            double f1 = 1575.42;//MHz
            double Tg;
            double phi_m = 0;
            double A = 0;
            double P = 0;
            //计算测站点P和交点P'在地心的夹角EA（单位为角度）
            double el = 0;//卫星在测站处的高度角
            double EA = Convert.ToDouble(445 / (el + 20)) - 4;
            //计算交点P'的地心纬度phi和经度lamuda
            double phi_P = 0;//测站的地心纬度
            double lamuda_P = 0;
            double Alpha = 0;
            double phi_p = phi_P + EA * Math.Cos(Alpha);
            double lamuda_p=lamuda_P + EA * Math.Sin(Alpha)/Math.Cos(phi_P);
            //计算地方时t
            double UT=0;
            double t = UT + lamuda_p / 15.0;
            //计算P'的地磁纬度
            double phi = 79.93;
            double lamuda = 288.04;//单位是度

            for (int i = 0; i < alpha.Length; i++)
            {
                A+=alpha[i]*Math.Pow(phi_m,i);
                P+=beta[i]*Math.Pow(phi_m,i);
            }
            Tg = 5 * 10e-9 + A * Math.Cos(2 * Math.PI / P * (t - 14));
            return Tg;
        }

        public static double deg2rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }
       
        public static double rad2deg(double rad)
        {
            return rad * 180 / Math.PI;
        }
    }
}
