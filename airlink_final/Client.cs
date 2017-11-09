using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace airlink_final
{
    class Client
    {
        static int seq = 0;
        public string Sseq;
        public string Email { get; set; }
        public string PRN { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string FN { get; set; }
        public string FD { get; set; }
        public string FT { get; set; }
        public string FDJ { get; set; }

        public Client()
        {
            seq++;
            DateTime dt = DateTime.Now;
            if (dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0)
            {
                seq = 0;
            }

            this.Sseq = String.Format("{0:000}", seq);
        }
        public void setDate()
        {

            this.FDJ = getdate(this.FD);


        }
        public string getdate(string str)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("JAN", 1);
            dic.Add("FEB", 32);
            dic.Add("MAR", 60);
            dic.Add("APR", 91);
            dic.Add("MAY", 121);
            dic.Add("JUN", 152);
            dic.Add("JUL", 182);
            dic.Add("AUG", 213);
            dic.Add("SEP", 244);
            dic.Add("OCT", 274);
            dic.Add("NOV", 305);
            dic.Add("DEC", 355);
            var match = Regex.Match(str, @"(\d*)(\D*)");
            var day = int.Parse(match.Groups[1].Value);
            var month = match.Groups[2].Value;
            if (dic.ContainsKey(month))
            {
                int x;
                dic.TryGetValue(month, out x);
                string xx = x.ToString();
                int d = x + day;
                if (xx.Length <= 2)
                {
                    string v = "0";
                    v = v + d.ToString();
                    return "0" + d.ToString();

                }
                else
                    return d.ToString();
            }
            return null;

        }
        public string fixname(string s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }
            string var = stringBuilder.ToString();
            if (var.Contains("-"))
            {
                string[] x = var.Split('-');
                var = x[0];
            }
            
            return var.ToUpper();

        }
        public string getdata(ref string v)
        {
            //string var = "M1";
            this.setDate();
            var Fname = this.fixname(this.FName);
            var Lname = this.fixname(this.LName);

            v =
               " " + this.FName + " " + this.LName + "\n" +
               " DAY               " + this.FD + "\n" +
               " FLIGHT          " + this.FN + "\n" +
               " Departure      " + this.FT + "\n";
            //var = var + Fname + "/" + Lname + "        E" + this.PRN + " HELSVL" + this.FN +"  "+ this.FDJ+" " + "001A" + this.Sseq + " 00";
            string var = "";
            Ticket t = new Ticket(Fname+"/"+Lname,this.PRN,this.FN,this.FDJ,this.Sseq);
            t.buildticket();
            var = t.getticket();
            return var;


        }
    }
}
