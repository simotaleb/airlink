using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AE.Net.Mail;
using System.IO;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using ZXing;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;


namespace airlink_final
{
    class Program
    {
        static Client cl;

        static void Main(string[] args)
        {
            while (true)
            {
                connection();
                
                System.Threading.Thread.Sleep(900000 * 60 * 1); //15min
            }
        }

        static void connection()
        {
            string path = "../../docs";
            string server = "imap.gmail.com";
            string email = "";
            string pw = "";

            try
            {
                using (ImapClient ic = new ImapClient(server, email, pw, AuthMethods.Login, 993, true))
                {
                    ic.SelectMailbox("inbox");
                    Console.WriteLine("running");
                    int x = 0;
                    Lazy<AE.Net.Mail.MailMessage>[] messages = ic.SearchMessages(SearchCondition.Undeleted(), false);

                    foreach (Lazy<AE.Net.Mail.MailMessage> msg in messages)
                    {
                        AE.Net.Mail.MailMessage m = msg.Value;
                        string sender = m.From.Address;
                        string FileName = string.Empty;

                        if (sender == "noreply@airlink.fi")
                        {
                            FileName = "../../docs/boardingpass";
                            Directory.CreateDirectory(path);
                            foreach (AE.Net.Mail.Attachment attachment in m.Attachments)
                            {
                                if (attachment.Filename.Contains("invoice") == false)
                                {
                                    x++;
                                    FileName = FileName + x;
                                    attachment.Save(FileName + Path.GetExtension(attachment.Filename));
                                    Pdf2text(FileName);
                                }
                            }

                            sendemail(cl);
                            Directory.Delete(path, true);
                            ic.DeleteMessage(m);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        static void Pdf2text(string FileName)
        {
            PDDocument doc = null;
            try
            {
                doc = PDDocument.load(FileName + ".pdf");
                PDFTextStripper stripper = new PDFTextStripper();
                File.WriteAllText(FileName + ".txt", stripper.getText(doc));
                getData(FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (doc != null)
                    doc.close();
            }
        }

        static void getData(string FileName)
        {
            string line;
            StreamReader file = new StreamReader(FileName + ".txt");
            Client cll = new Client();

            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("@"))
                {
                    if (line.Contains("airlink") == false)
                    {
                        cll.Email = line;
                    }
                }
                else if (line.Contains("Varaustunnus"))
                {
                    string[] var = line.Split(' ');
                    cll.PRN = var[1];
                }
                else if (line.Contains("Record locator"))
                {
                    string[] var = line.Split(' ');
                    cll.PRN = var[2];
                }
                else if (line.Contains("4011"))
                {
                    string[] var = line.Split(' ');
                    cll.FName = var[1];
                    cll.LName = var[2];
                }
                else if (line.Contains("BPS655") || line.Contains("BPS651"))
                {
                    string[] var = line.Split(' ');
                    string[] x = var[0].Split('/');
                    cll.FN = x[0];
                    cll.FD = x[1];
                    if (var.Length <= 2)
                    {
                        cll.FD = x[1];
                    }
                    else
                    {
                        cll.FT = var[3];
                    }
                }
                else if (line.Contains("Savonlinna SVL"))
                {
                    string[] var = line.Split(' ');
                    Regex checktime = new Regex(@"(([0-1][0-9])|([2][0-3])):([0-5][0-9])");
                    if (checktime.IsMatch(var[2]))
                    {
                        cll.FT = var[2];
                    }
                }
            }

            file.Close();
            cl = cll;          
            getAztec(cl, FileName);
        }

        static void getAztec(Client cl, String FileName)
        {
            var text = "";
            buildpdf(cl, FileName, text);
        }

        static void buildpdf(Client cl, String FileName, string text)
        {

            string var = cl.getdata(ref text);
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.AZTEC
            };
           
            using (var bitmap = writer.Write(var))
            {
                bitmap.Save(FileName + ".png");

            }

            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Verdana", 15, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);
            XImage img = XImage.FromFile(FileName + ".png");
            gfx.DrawImage(img, 200, 50, 200, 200);
            img.Dispose();

            XRect rect = new XRect(220, 280, 200, 100);
            gfx.DrawRectangle(XBrushes.Transparent, rect);
            tf.DrawString(text, font, XBrushes.Black, rect, XStringFormats.TopLeft);
            gfx.DrawString("Have a nice flight", font, XBrushes.Black, 240, 400);

            img = XImage.FromFile("logo.png");
            gfx.DrawImage(img, 200, 420, 180, 82);
            doc.Save(FileName + ".pdf");

            page.Close();
            img.Dispose();
            gfx.Dispose();
            doc.Close();
        }

        static void sendemail(Client cl)
        {
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("asiakaspalvelu@airlink.fi", "j00natan")
            };
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress("asiakaspalvelu@airlink.fi"), new System.Net.Mail.MailAddress((cl.Email)))
            {
                Subject = "luo bordari",
                Body = "//" + cl.LName + "/"+cl.FName + "/" + cl.FD + "/" + cl.FN
            };

            string path = "../../docs";
            DirectoryInfo d = new DirectoryInfo(path);

            foreach (var file in d.GetFiles("*.pdf"))
            {
                try
                {               
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(file.FullName);
                    m.Attachments.Add(attachment);          
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

            }

            smtp.Send(m);
            smtp.Dispose();
            m.Dispose();
            d = null;
        }
    }
}
