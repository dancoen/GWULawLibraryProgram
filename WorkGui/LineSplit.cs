using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FileSplit
{
    class LineSplit
    {
        
        public static ArrayList splitLine(string[] text)
        {
            int gwidIndex = 0;
            int i = 0;
            ArrayList dividedLines = new ArrayList();
            foreach (string element in text)
            {
                if (text[i].Contains("GWid")) { gwidIndex = i; }
                if (text[i].Length != 0 && text[i] != null)
                {
                    if (text[i].Length < 52){ dividedLines.Add(text[i]); }
                    else
                    {
                        dividedLines.Add(text[i].Substring(0, 52));
                        if (text[i].Contains("CONTINUED ON NEXT")) //indicates that there is another column to add to the arraylist
                        {
                            int j = gwidIndex;
                            while (true){
                                if (text[j].Length >= 52 && !text[j].EndsWith("gwlaw"))
                                {
                                    if (text[j].Length < 64){ dividedLines.Add(text[j]); }
                                    else
                                    {
                                        string temp2 = text[j].Substring(64);
                                        dividedLines.Add(temp2);
                                        if (temp2.Contains("CONTINUED ON PAGE") || temp2.Contains("END OF DOCUMENT")) { break; }
                                    }
                                }
                                j++;
                            }
                        }
                    }
                }
                i++;
            }
            return dividedLines;
        }
    }
}
