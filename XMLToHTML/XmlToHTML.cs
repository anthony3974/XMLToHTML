using System;
using System.IO;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;

namespace XMLToHTML
{
    internal partial class XMLToHTML
    {
        enum H { h6s, none }
        enum T { Method, Property, Field, Invalid }
        static void Main(string[] args)
        {
            // settings
            bool oneFile = false;
            string icoPath = null;
            bool showXMLReads = false;
            bool showHTMLWrites = false;


            // main code
            string[] files = Directory.GetFiles("."); // gets list of dir files ending with xml
            foreach (string file in files) // main loop for each file in current dir
            {
                if (file.EndsWith(".xml")) // if ends with xml
                {
                    StreamReader xml = new StreamReader(file); // make new file reader
                    FileInfo fileInfo = new FileInfo(file);  // get the name for saving file
                    string htmlfileName = fileInfo.FullName.Substring(0, fileInfo.FullName.Length - fileInfo.Extension.Length); // set the name
                    StreamWriter html = new StreamWriter(htmlfileName + ".html"); // make an html file to write with the file name                                                           
                    string htmlPart1 = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n<meta charset=\"utf-8\">\r\n<title> Docs for {htmlfileName.Substring(htmlfileName.LastIndexOf("\\") + 1)} </title>\r\n"; // initial html part 1
                    string htmlPart2 = "<link rel=\"stylesheet\" href=\"style.css\">\r\n"; // initial html part 2
                    string htmlPart3 = ""; // initial html part 3
                    if (icoPath != null) htmlPart3 = $"<link rel=\"icon\" href=\"{icoPath}\" type=\"image/icon\">"; // initial html part 3
                    string htmlPart4 = "\r\n</head>\r\n<body>"; // initial html part 4
                    if (oneFile) MakeInHTMLStyle(ref htmlPart2);
                    Write(htmlPart1 + htmlPart2 + htmlPart3 + htmlPart4); // writing the basic starting html code

                    string line = xml.ReadLine(); // read first line
                    void Read(int amount = 1) // method to make reading lines easyer
                    {
                        for (int i = 0; i < amount; i++) // loop for param amount which allows for many lines at once
                        {
                            line = xml.ReadLine(); // reading line and saving to gobal varable line
                        }
                        if (showXMLReads) Console.WriteLine(line);
                    }
                    void Write(string s) // method to make writing lines easyer
                    {
                        html.WriteLine(s); // writes the s string to the current html file
                        if (showHTMLWrites) Console.WriteLine(s);
                    }
                    Read(2); // read 2 lines to get the assembly
                    if (line.Contains("<assembly>")) // makes the package name
                    {
                        Read(); // reads the next line
                        string tittle = Regex.Match(line, "name>(.*)</name").Groups[1].Value; // find the name and save it to the varable tittle
                        Write($"<section class=\"h1\">{tittle}"); // make a section with the tittle name
                        Read(3); // reads 3 lines to get the member
                    }
                    if (line.Contains("<member")) // start the scan
                    {
                        string getType() { return Regex.Match(line, "name=\"T:(.*)\">").Groups[1].Value; } // looks for the main type
                        string type = getType(); // sets the main type
                        while (type != "") // repeats for each type
                        {
                            Write($"<section class=\"h2\"><strong>Type:</strong> {type}"); // makes the type window
                            Read(); // gets the next line
                            void writesum(H h6 = H.none) // method to make summary
                            {
                                if (line.Contains("<summary>")) // if it is a summary
                                {
                                    Read(); // gets the next line with the summary
                                    if (h6 != H.none) Write($"<section class=\"sum {h6}\"><strong>Summary:</strong> "); // makes the inital summary and p tag
                                    else Write($"<section class=\"sum\"><strong>Summary:</strong> "); // makes the inital summary and p tag
                                    while (!line.Contains("</summary>")) // add each line until the summary ends
                                    {
                                        Write(line.Trim() + "<br>"); // writes the line in the xml file then adds a break
                                        Read(); // reads next line
                                    }
                                    Write("</section>"); // ends the p tag
                                    Read(); // line after </summary>
                                }
                            }
                            writesum(); // writes the main class summary
                            Read(); // reads the next line
                            if (line.Contains(":")) // if it contains :
                            {
                                Write($"<section class=\"h3\"><strong>Data types:</strong> {type}"); // houses the data types of the type
                                while (Regex.Match(line, "[^T]:").Success) // if current line with colon (:) dose not contain "T"
                                {
                                    void settype(T tp) // method for creating the sub types in the data type
                                    {
                                        if (line.Contains("#")) Write($"<section class=\"h4 Con\"><strong>Constructor:</strong> {type}({Regex.Match(line, "\\((.*)\\)\">").Groups[1].Value})"); // houses child member of data type that is constructor
                                        else Write($"<section class=\"h4 {tp}\"><strong>{tp}:</strong> {Regex.Match(line, $"{type}\\.(.*)\">").Groups[1].Value}"); // houses child member of data type
                                    }
                                    if (line.Contains("M:")) // if line is method type
                                    {
                                        settype(T.Method); // set the section
                                        Read(); // read the next line
                                        Write("<section class=\"h5\">"); // make sub section for the method
                                        writesum(H.h6s);
                                        if (line.Contains("<param"))
                                        {
                                            Write($"</section><section class=\"h4\" style=\"background-color: inherit;\"></section><section class=\"h5\">");
                                            Write("<section class=\"h6m\">");
                                            while (line.Contains("<param"))
                                            {
                                                string paramn = Regex.Match(line, "name=\"(.*)\">").Groups[1].Value;
                                                string paramd = Regex.Match(line, "\">(.*)<\\/").Groups[1].Value;
                                                Write($"<strong>Param:</strong> {paramn}: {paramd}<br>");
                                                Read();
                                            }
                                            Write("</section>");
                                        }
                                        if (line.Contains("<returns>"))
                                        {
                                            Write($"</section><section class=\"h4 {T.Method}\"></section><section class=\"h5\">");
                                            string returns = Regex.Match(line, ">(.*)<\\/").Groups[1].Value;
                                            Write($"<section class=\"h6m\"><strong>Returns:</strong> {returns}</section>");
                                            Read();
                                        }
                                        Write($"</section>");
                                        Read();
                                    }
                                    else if (line.Contains("P:"))
                                    {
                                        settype(T.Property);
                                        Read();
                                        Write("<section class=\"h5\">");
                                        writesum(H.h6s);
                                        Write($"</section>");
                                        Read();
                                    }
                                    else if (line.Contains("F:"))
                                    {
                                        settype(T.Field);
                                        Read();
                                        Write("<section class=\"h5\">");
                                        writesum(H.h6s);
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
                            type = getType();
                        }
                    }
                    html.Write("</section>\r\n</body>\r\n</html>");
                    html.Close();
                }
            }
        }
    }
}