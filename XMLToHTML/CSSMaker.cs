using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLToHTML
{
    internal partial class XMLToHTML
    {

        static void MakeInHTMLStyle(ref string htmlPart2)
        {
            htmlPart2 = "html {\r\n    width: auto;\r\n    word-wrap: break-word;\r\n    background-color: mediumpurple;\r\n}\r\n\r\nbody {\r\n    font-family: sans-serif;\r\n    font-style: normal;\r\n}\r\n\r\n.h1 {\r\n    background-color: #ed5564;\r\n    font-size: 40px;\r\n    padding: 3px 15px 5px 15px;\r\n}\r\n\r\n.h2 {\r\n    background-color: #a0d568;\r\n    font-size: 28px;\r\n    padding: 3px 15px 10px 15px;\r\n}\r\n\r\n.h3 {\r\n    background-color: #ffce54;\r\n    font-size: 25px;\r\n    padding: 3px 15px 5px 15px;\r\n}\r\n\r\n.h4 {\r\n    font-size: 22px;\r\n    padding: 3px 15px 10px 15px;\r\n}\r\n\r\n    .h4.Con {\r\n        background-color: #ffeeee;\r\n        font-style: italic;\r\n    }\r\n\r\n    .h4.Method {\r\n        background-color: #ff9876;\r\n    }\r\n\r\n    .h4.Property {\r\n        background-color: #7679ff;\r\n    }\r\n\r\n    .h4.Field {\r\n        background-color: #99ff76;\r\n    }\r\n\r\n    .h4.Invalid {\r\n        background-color: #38ce27;\r\n    }\r\n\r\n.h5 {\r\n    background-color: #97c0ff;\r\n    font-size: 19px;\r\n    padding: 1px 15px 1px 15px;\r\n    font-style: normal;\r\n}\r\n\r\n.sum {\r\n    font-size: 21px;\r\n    padding: 3px 5px 5px 5px;\r\n}\r\n\r\n.h6s {\r\n    font-size: 19px;\r\n    padding: 8px 0px 8px 0px;\r\n}\r\n\r\n.h6m {\r\n    line-height: 1.3em;\r\n    padding: 8px 0px 8px 0px;\r\n}\r\n";
        }





    }
}
