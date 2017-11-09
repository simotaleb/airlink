using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airlink_final
{
    class Ticket
    {
        public string fc = "M";
        public string leg = "1";
        public string passengername { get; set; }
        public string ticketindicator = "E";
        public string pnr { get; set; }
        public string fromcity = "HEL";
        public string tocity = "SVL";
        public string flightnumber { get; set; }
        public string flightdate { get; set; }
        public string compartementcode = "Y";
        public string seatnumber = "001A";
        public string sequencenumber { get; set; }
        public string passengerstatus = "1";
        public string sizefield = "00";

        public Ticket(string passengername,string pnr,string flightnumber,string flightdate,string sequencenumber)
        {
            this.passengername = passengername;
            this.pnr = pnr;
            this.flightnumber = flightnumber;
            this.flightdate = flightdate;
            this.sequencenumber = sequencenumber;
        }

        public void buildticket()
        {
            int passengername = 20 - this.passengername.Length; //20
            int pnr = 7 - this.pnr.Length; //7
            int flightnumber = 8 - this.flightnumber.Length; //8
            int sequencenumber = 5 - this.sequencenumber.Length;//5
            if (passengername < 20)
            {
                for (int x = 0; x < passengername; x++)
                {
                    this.passengername += " ";
                }
            }
            if (pnr < 7)
            {
                for (int x = 0; x < pnr; x++)
                {
                    this.pnr += " ";
                }
            }
            if (flightnumber < 8)
            {
                for (int x = 0; x < flightnumber; x++)
                {
                    this.flightnumber += " ";
                }
            }
            if (sequencenumber < 5)
            {
                for (int x = 0; x < sequencenumber; x++)
                {
                    this.sequencenumber += " ";
                }
            }
        }

        public string getticket()
        {
            string ticket = this.fc + this.leg + this.passengername + this.ticketindicator + this.pnr + this.fromcity + this.tocity + this.flightnumber + this.flightdate + this.compartementcode + this.seatnumber + this.sequencenumber + this.passengerstatus + this.sizefield;
            return ticket;
        }
    }
}
