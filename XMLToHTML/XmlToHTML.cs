using System;
using System.IO;
using System.Text.RegularExpressions;

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
            bool manyScan = true;
            bool makeCss = true;

            // main code
            string[] files; // gobal varable files
            string pathToStartScanning = ".."; // scan files here
            if (!manyScan) files = Directory.GetFiles(pathToStartScanning); // gets list of dir files in the current dir
            else { files = FFScaner.ScanFiles(pathToStartScanning).ToArray(); } // gets list of dir files in the current dir

            foreach (string file in files) // main loop for each file in current dir
            {
                if (file.EndsWith(".xml")) // if file ends with xml
                {
                    StreamReader xml = new StreamReader(file); // make new file reader from the xml file
                    FileInfo fileInfo = new FileInfo(file);  // get the name for saving file
                    string htmlfileName = pathToStartScanning + "\\" + fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length) + ".html"; // set the name of the html file
                    StreamWriter html = new StreamWriter(htmlfileName); // make an html file to write with the file name                                                           
                    string htmlPart1 = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n<meta charset=\"utf-8\">\r\n<title> Docs for {htmlfileName.Substring(htmlfileName.LastIndexOf("\\") + 1)} </title>\r\n"; // initial html part 1
                    string htmlPart2 = "<link rel=\"stylesheet\" href=\"style.css\">\r\n"; // initial html part 2
                    string htmlPart3 = ""; // initial html part 3
                    if (icoPath != null) htmlPart3 = $"<link rel=\"icon\" href=\"{icoPath}\" type=\"image/icon\">"; // initial html part 3
                    string htmlPart4 = "\r\n</head>\r\n<body>"; // initial html part 4
                    if (oneFile && makeCss) MakeInHTMLStyle(ref htmlPart2);
                    else if (!oneFile && makeCss) MakeFileHTMLStyle(pathToStartScanning);
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
                    else // if it is not a c# api xml file then
                    {
                        html.Close(); // close
                        File.Delete(htmlfileName); // delete
                        continue; // do to next file
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
                                    else Write("<section class=\"sum\"><strong>Summary:</strong> "); // makes the inital summary and p tag
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
                                        writesum(H.h6s); // makes the summary for the method
                                        if (line.Contains("<param")) // if the method has params
                                        {
                                            Write("</section><section class=\"h4\" style=\"background-color: inherit;\"></section><section class=\"h5\">"); // makes a divider from summary to params and opens section to put params
                                            Write("<section class=\"h6m\">"); // color the background for the set of params
                                            while (line.Contains("<param")) // for each params, loop
                                            {
                                                string paramn = Regex.Match(line, "name=\"(.*)\">").Groups[1].Value; // get the name of the param
                                                string paramd = Regex.Match(line, "\">(.*)<\\/").Groups[1].Value; // get the message of the param
                                                Write($"<strong>Param:</strong> {paramn}: {paramd}<br>"); // writes the two values to the html file
                                                Read(); // reads the next line
                                            }
                                            Write("</section>"); // ends the params section
                                        }
                                        if (line.Contains("<returns>")) // checks if there are any returns
                                        {
                                            Write($"</section><section class=\"h4 {T.Method}\"></section><section class=\"h5\">"); // makes a divider from params/summary to return and opens section to put return in
                                            string returns = Regex.Match(line, ">(.*)<\\/").Groups[1].Value; // gets the description of what is returned
                                            Write($"<section class=\"h6m\"><strong>Returns:</strong> {returns}</section>"); // writes the description to the html file
                                            Read(); // reads the next line
                                        }
                                        Write("</section>"); // ends the section for the method
                                        Read(); // reads the next line
                                    }
                                    else if (line.Contains("P:")) // if line is property type
                                    {
                                        settype(T.Property); // set the section
                                        Read(); // reads the next line(s)
                                        Write("<section class=\"h5\">"); // start of section
                                        writesum(H.h6s); // makes the summary for the property
                                        Write("</section>"); // end of section
                                        Read(); // reads the next line(s)
                                    }
                                    else if (line.Contains("F:")) // if line is felid type
                                    {
                                        settype(T.Field); // set the section
                                        Read(); // reads the next line(s)
                                        Write("<section class=\"h5\">"); // start of section
                                        writesum(H.h6s); // makes the summary for the felid
                                        Write("</section>"); // end of section
                                        Read(); // reads the next line(s)
                                    }
                                    else if (line.Contains(":")) // other types are not suported
                                    {
                                        settype(T.Invalid); // set the section 
                                        Console.WriteLine("not a vaild type"); // writes not a valid type
                                        Read(); // reads the next line(s)
                                        Write("<section style=\"color:red;\"> <strong>Invalid type</strong></section>"); // writes a section saying not valid type
                                        while (!line.Contains("</member")) { Read(); } // reads the next line(s)
                                        Read(); // reads the next line(s)
                                    }
                                    Write("</section>"); // ends the section for the data type
                                    Write("<section class=\"h3\"></section>"); // makes a divider section
                                }
                                Write("</section>"); // ends section for sub data type
                            }
                            Write("</section>"); // ends section for data type
                            Write("<section class=\"h1\"></section>"); // makes a divider for data types
                            type = getType(); // method to set type to see if it should run again
                        }
                    }
                    html.Write("</section>\r\n</body>\r\n</html>"); // ends section for data type
                    html.Close(); // closes the html file
                    xml.Close(); // closes the xml file
                }
            }
        }
    }
}