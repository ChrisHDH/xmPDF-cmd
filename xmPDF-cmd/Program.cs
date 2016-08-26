using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Ionic.Zlib;
using xmPDF;


namespace xmPDF_cmd
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("start");

            byte[] inbytes;

            var mPDF = new PDF();
            mPDF.LoadPdf(@"C:\Working\PDF_Test.pdf");

            // read first line
            inbytes = mPDF.ReadPdfNextline();
            mPDF.Version = Encoding.UTF8.GetString(inbytes);
            mPDF.WritePdfline(inbytes);

            // read 2nd line
            inbytes = mPDF.ReadPdfNextline();
            mPDF.FileType = Encoding.UTF8.GetString(inbytes);
            mPDF.WritePdfline(inbytes);


            // Read Objects
            bool bInObjects = true;
            while ( bInObjects = true )
            {
                // read object
                inbytes = mPDF.ReadPdfNextline();
                Console.WriteLine("Line1: " + Encoding.UTF8.GetString(inbytes)); 
                // check if done
                if (mPDF.IndexOf(inbytes, mPDF.serXref) > -1)
                {
                    bInObjects = false;
                    break;
                }

                //-- Stream -------------------------------------------------------------------------
                if (mPDF.IndexOf(inbytes, mPDF.serStreamStart) > -1 )
                {
                    // New Object
                    byte[] inStream = new byte[0];
                    bool bInStream = true;
                    while (bInStream = true)
                    {
                        inbytes = mPDF.ReadPdfNextline();
                        if (mPDF.IndexOf(inbytes, mPDF.serXref) > -1)
                        {
                            bInStream = false;
                            break;
                        }
                        Console.WriteLine("Line2: " + Encoding.UTF8.GetString(inbytes));
                        mPDF.appendBytesValue(inStream, inbytes);
                    }
                }
                
            }
            
            Console.WriteLine("PDF-Version: " + mPDF.Version);

                        
            Console.WriteLine("Done");
        }
    }
}
