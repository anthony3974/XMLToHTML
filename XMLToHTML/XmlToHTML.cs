using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace XMLToHTML
{
    internal class XMLToHTML
    {
        enum H { h6s, h6ms, h6sp, h6sf }
        enum T { Method, Property, Field, Invalid }
        static void Main(string[] args)
        {

            string[] files = Directory.GetFiles(".");
            foreach (string file in files) // main loop for each file in current dir
            {
                if (file.EndsWith(".xml")) // if ends with xml
                {
                    StreamReader xml = new StreamReader(file); // make new file reader
                    FileInfo fileInfo = new FileInfo(file);  // get the name for saving file
                    string htmlfileName = fileInfo.FullName.Substring(0, fileInfo.FullName.Length - fileInfo.Extension.Length); // set the name
                    StreamWriter html = new StreamWriter(htmlfileName + ".html"); // make an html file to write                                                                                                                   <link rel=\"icon\" href=\"t.ico\type=\"image/icon\">\r\n
                    string _ = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n<meta charset=\"utf-8\">\r\n<title> Docs for {htmlfileName.Substring(htmlfileName.LastIndexOf("\\") + 1)} </title>\r\n<link rel=\"stylesheet\" href=\"style.css\">\r\n</head>\r\n<body>";
                    Write(_);

                    string line = xml.ReadLine(); // read first line

                    void Read(int amount = 1) // method to make reading lines easyer
                    {
                        for (int i = 0; i < amount; i++)
                        {
                            line = xml.ReadLine();
                        }
                        //Console.WriteLine(line);
                    }
                    void Write(string s) // method to make writing lines easyer
                    {
                        html.WriteLine(s);
                        //Console.WriteLine(s);
                    }
                    Read(2);
                    if (line.Contains("<assembly>")) // add h1
                    {
                        Read();
                        string tittle = Regex.Match(line, "name>(.*)</name").Groups[1].Value;
                        Write($"<section class=\"h1\">{tittle}");
                        Read(3);
                    }
                    if (line.Contains("<member"))
                    {
                        Match rx;
                        string type;
                        string[] layers;
                        bool dothis()
                        {
                            rx = Regex.Match(line, "name=\"T:(.*)\">");
                            type = rx.Groups[1].Value;
                            layers = type.Split('.');
                            return rx.Success;
                        }
                        while (dothis())
                        {
                            Write($"<section class=\"h2\"><strong>Type:</strong> {type}");
                            Read();
                            void writesum(H h6)
                            {
                                if (line.Contains("<summary>"))
                                {
                                    Read();
                                    Write($"<p class=\"sum {h6}\"><strong>Summary:</strong> ");
                                    while (!line.Contains("</summary>"))
                                    {
                                        Write(line.Trim() + "<br>");
                                        Read();
                                    }
                                    Write("</p>");
                                    Read(); // line after </summary>
                                }
                            }
                            writesum(H.h6ms);
                            Read();
                            if (line.Contains(":"))
                            {
                                Write($"<section class=\"h3\"><strong>Data types:</strong> {type}"); // houses the main type
                                while (Regex.Match(line, "[^T]:").Success)
                                {
                                    void settype(T tp)
                                    {
                                        Write($"<section class=\"h4\"><strong>{tp}:</strong> {Regex.Match(line, $"{type}\\.(.*)\">").Groups[1].Value}"); // houses child members
                                    }
                                    if (line.Contains("M:"))
                                    {
                                        settype(T.Method);
                                        Read();
                                        Write("<section class=\"h5m\">");
                                        writesum(H.h6s);
                                        if (line.Contains("<param"))
                                        {

                                            Read();
                                            Write("<section class=\"h6p\">");
                                            while (line.Contains("<param"))
                                            {
                                                string paramn = Regex.Match(line, "name=\"(.*)\">").Groups[1].Value;
                                                string paramd = Regex.Match(line, "\">(.*)<\\/").Groups[1].Value;
                                                Write($"<p class=\"h7\"><strong>Param:</strong> {paramn}: {paramd}</p>");
                                                Read();
                                            }
                                            Write("</section>");
                                        }
                                        if (line.Contains("<returns>"))
                                        {
                                            string returns = Regex.Match(line, ">(.*)<\\/").Groups[1].Value;
                                            Write($"<p class=\"h6r\"><strong>Returns:</strong> {returns}</p>");
                                            Read();
                                        }
                                        Write($"</section>");
                                        Read();
                                    }
                                    else if (line.Contains("P:"))
                                    {
                                        settype(T.Property);
                                        Read();
                                        Write("<section class=\"h5p\">");
                                        writesum(H.h6sp);
                                        Write($"</section>");
                                        Read();
                                    }
                                    else if (line.Contains("F:"))
                                    {
                                        settype(T.Field);
                                        Read();
                                        Write("<section class=\"h5f\">");
                                        writesum(H.h6sf);
                                        Write($"</section>");
                                        Read();
                                    }
                                    else if (line.Contains(":"))
                                    {
                                        settype(T.Invalid);
                                        Console.WriteLine("not a vaild type");
                                        Read();
                                        Write("<p style=\"color:red;\"> <strong>Invalid type</strong></p>");
                                        while (!line.Contains("</member")) { Read(); }
                                        Read();
                                    }
                                    Write($"</section>");
                                    Write($"<section class=\"h3\"></section>");
                                }
                                Write($"</section>");
                            }
                            Write($"</section>");
                            Write($"<section class=\"h1\"></section>");
                        }
                    }
                    html.Write("</section>\r\n</section>\r\n</body>\r\n</html>");
                    html.Close();
                }
            }
        }
    }
}