using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Services
{

    public class NumberExtendMethod
    {
        #region Doc so thanh chu
        //Đọc số hàng chục
        private static List<string> mangso = new List<string>() { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static string dochangchuc(double so, bool daydu)
        {
            string chuoi = "";
            int chuc = (int)Math.Floor(so / 10);
            int donvi = (int)so % 10;
            if (chuc > 1)
            {
                chuoi = " " + mangso[chuc] + " mươi";
                if (donvi == 1)
                {
                    chuoi += " mốt";
                }
            }
            else if (chuc == 1)
            {
                chuoi = " mười";
                if (donvi == 1)
                {
                    chuoi += " một";
                }
            }
            else if (daydu && donvi > 0)
            {
                chuoi = " lẻ";
            }
            if (donvi == 5 && chuc >= 1)
            {
                chuoi += " lăm";
            }
            else if (donvi > 1 || (donvi == 1 && chuc == 0))
            {
                chuoi += " " + mangso[donvi];
            }
            return chuoi;
        }
        //Đọc block 3 số
        private static string docblock(double so, bool daydu)
        {
            string chuoi = "";
            int tram = (int)Math.Floor(so / 100);
            so = so % 100;
            if (daydu || tram > 0)
            {
                chuoi = " " + mangso[tram] + " trăm";
                chuoi += dochangchuc(so, true);
            }
            else
            {
                chuoi = dochangchuc(so, false);
            }
            return chuoi;
        }
        //Đọc số hàng triệu
        private static string dochangtrieu(double so, bool daydu)
        {
            string chuoi = "";
            int trieu = (int)Math.Floor(so / 1000000);
            so = so % 1000000;
            if (trieu > 0)
            {
                chuoi = docblock(trieu, daydu) + " triệu ";
                daydu = true;
            }
            double nghin = Math.Floor(so / 1000);
            so = so % 1000;
            if (nghin > 0)
            {
                chuoi += docblock(nghin, daydu) + " nghìn ";
                daydu = true;
            }
            if (so > 0)
            {
                chuoi += docblock(so, daydu);
            }
            return chuoi;
        }

        //Đọc số
        public static string docso(double so)
        {
            if (so == 0) return mangso[0];
            string chuoi = "", hauto = "";
            do
            {
                double ty = so % 1000000000;
                so = Math.Floor(so / 1000000000);
                if (so > 0)
                {
                    chuoi = dochangtrieu(ty, true) + hauto + chuoi;
                }
                else
                {
                    chuoi = dochangtrieu(ty, false) + hauto + chuoi;
                }
                hauto = " tỷ ";
            } while (so > 0);
            try
            {
                if (chuoi.Trim().Substring(chuoi.Trim().Length - 1, 1) == ",")
                { chuoi = chuoi.Trim().Substring(0, chuoi.Trim().Length - 1); }
            }
            catch { }
            chuoi = chuoi.Trim();
            string kyTuDau = chuoi.Substring(0, 1); //left(1)
            kyTuDau = kyTuDau.ToUpper();
            chuoi = kyTuDau + chuoi.Substring(1, chuoi.Length - 1); //mid(2, length -1 )
            return chuoi.Trim();
        }
        #endregion

    }

}
