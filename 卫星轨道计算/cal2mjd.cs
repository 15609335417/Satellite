using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卫星轨道计算
{
    internal class cal2mjd
    {
        static public  void Cal2mjd(double[] cal,out double mjd)
        {
            
            double year=cal[0]; 
            double month=cal[1];
            double day = cal[2] + (cal[3] * 3600 + cal[4] * 60 + cal[5])/86400;
            double y = year+4800;
            double m = month;
            if(m<=2)
            {
                m = m + 12;
                y = y - 1;
            }
            double e = Math.Floor(30.6 * (m + 1));
            double a = Math.Floor(y / 100);
            double b;
            if (year < 1582)
            {
                b = -38;
            }
            else if (year == 1582 && month < 10)
                b = -38;
            else if (year == 1582 && month == 10 && day < 15)
                b = -38;
            else
            {
                b = Math.Floor((a / 4) - a);
            }
            double c= Math.Floor(365.25*y);
            double jd = b + c + e + day - 32167.5;
            mjd = jd - 2400000.5;

        }
    }
}
