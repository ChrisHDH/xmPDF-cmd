using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Ionic.Zlib;


namespace xmPDF_cmd
{
    class Program
    {
        /// <summary>
        /// Converts a string to a MemoryStream.
        /// </summary>
        static System.IO.MemoryStream StringToMemoryStream(string s)
        {
            byte[] a = System.Text.Encoding.ASCII.GetBytes(s);
            return new System.IO.MemoryStream(a);
        }

        /// <summary>
        /// Converts a MemoryStream to a string. Makes some assumptions about the content of the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static String MemoryStreamToString(System.IO.MemoryStream ms)
        {
            byte[] ByteArray = ms.ToArray();
            return System.Text.Encoding.ASCII.GetString(ByteArray);
        }


        static void CopyStream(System.IO.Stream src, System.IO.Stream dest)
        {
            byte[] buffer = new byte[1024];
            int len;
            while ((len = src.Read(buffer, 0, buffer.Length)) > 0)
                dest.Write(buffer, 0, len);
            dest.Flush();
        }

        static void Main(string[] args)
        {

            string[] sObjects = new string[10000];

            string path = @"C:\Working\xm\0000-PDF_Generation\PDF_Test.pdf";
            string PdfVersion = "";


            // Open the stream and read it back. 
            using (StreamReader sr = File.OpenText(path))
            {
                //pdfVersion - first line 
                PdfVersion = sr.ReadLine();
                PdfVersion = PdfVersion + " --- " + sr.ReadLine();
                PdfVersion = PdfVersion.Replace("\r\n", "");
                PdfVersion = PdfVersion.Replace("%", "");
                PdfVersion = PdfVersion.Trim();

                Console.WriteLine(PdfVersion);


                // read objects
                string s = "";
                int iCtn = 0;
                string sStream = "";
                string strType = "";
                string sOutput = "";
                string strType2 = "";


                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains(" obj"))
                    {
                        if (s.Equals("34 0 obj"))
                        {
                            strType = "/BearingPoint";
                        }

                            // Add to object ============================================
                            sObjects[iCtn] = s + "\r\n"; ;
                        while ((s = sr.ReadLine()) != null)
                        {
                            // Get Type
                            if ((s.Length >= 5 && s.Substring(0,5) == "/Type") )
                            {
                                strType2 = s.Substring(6,s.Length-6);
                            }

                            // Skip stream
                            if (s.Equals("stream"))
                            {
                                sObjects[iCtn] = sObjects[iCtn] + s + ".......";

                                while ((s = sr.ReadLine()) != null)
                                {
                                    //Save stream if /BearingPoint
                                    if (strType == "/BearingPoint" || !s.Equals("endstream"))
                                    {
                                        sStream = sStream + s;
                                    }
                                    if (s.Equals("endstream"))
                                    {
                                        sObjects[iCtn] = sObjects[iCtn] + s + "\r\n";
                                        break;
                                    }
                                }
                                if (strType == "/BearingPoint")
                                {



                                    // zLib & print out
                                    System.IO.MemoryStream msSinkDecompressed;
                                    ZlibStream zOut;
                                    
                                    // now, decompress:
                                    msSinkDecompressed = new System.IO.MemoryStream();
                                    zOut = new ZlibStream(msSinkDecompressed, CompressionMode.Decompress, true);
                                    CopyStream(StringToMemoryStream(sStream), zOut);

                                    // at this point, msSinkDecompressed contains the decompressed bytes
                                    sOutput = MemoryStreamToString(msSinkDecompressed);
                                    //System.Console.Out.WriteLine("decompressed: {0}", decompressed);
                                    //System.Console.WriteLine();
                                    
                                }

                            }
                            else
                            {
                                sObjects[iCtn] = sObjects[iCtn] + s + "\r\n"; ;
                                if (s.Equals("endobj"))
                                    break;
                            }
                        }
                        //============================================================
                        Console.WriteLine(sObjects[iCtn]);
                        Console.WriteLine("=== Type: " + strType);
                        Console.WriteLine("decompressed: {0}", sOutput);
                    }
                    if (s.Contains("trailer"))
                    {
                        // Add to object ============================================
                        sObjects[iCtn] = s + "\r\n"; ;
                        while ((s = sr.ReadLine()) != null)
                          {
                                sObjects[iCtn] = sObjects[iCtn] + s + "\r\n"; ;
                                if (s.Equals(">>"))
                                     break;
                          }
                        //============================================================
                        Console.WriteLine(sObjects[iCtn]);
                    }
                    iCtn++;

                }

                
                Console.ReadKey();

         
                //string s = "";
                //while ((s = sr.ReadLine()) != null)
                //{
                //Console.WriteLine(s);
                //}
            }
            
        }
       

    }
}
