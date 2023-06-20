using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卫星轨道计算
{
    internal class cal2gps
    {
        static public void Cal2gps(double[] cal, out double week, out double gps)
        {
            
            cal2mjd.Cal2mjd(cal, out double mjd);
            double e = mjd - 44244;
            week = Math.Floor(e / 7);
            e = e - week * 7;
            gps = Math.Round(e * 86400);
        }
    }
}
