using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using classesForLibraryExcel;


namespace LibraryProject
{
    class FullClearance {
            public static List<Student> createStudent(string[] text)
            {
                bool newStudent = true;
                List<Student> listStudent = new List<Student>();
                int t1;
                int t2;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i].Contains("Record of") && newStudent)//indicates a new student has been found
                    {
                        t1 = i-2;
                        newStudent = false;
                        string Gwid = text[i - 2].Substring(13, 9);
                        string name = text[i].Substring(11, text[i].Length - 11);//obtains the name of the student
                        name = name.Trim();
                        Student stud = new Student(name);
                        stud.setGWid(Gwid);
                        if (text[i + 1].Contains("Law"))//indicates that the student is a law student
                        {
                            stud.setLaw(true);
                            createSemester(text, stud, i);
                            listStudent.Add(stud);
                        }
                        else if (text[i + 1].Contains("Graduate"))
                        {
                            stud.setGrad(true);
                        }
                        else
                        {
                            stud.setUnderGrad(true);
                        }
                        for (int x = i; x < i + 17; x++)//sets the skills and writing requirements if they are found in the legend
                        {
                            if (text[x].Contains("SKILLS REQUIREMENT MET"))
                            {
                                stud.setSkillSat("SATISFIED");
                            }
                            if (text[x].Contains("WRITING REQUIREMENT MET"))
                            {
                                stud.setWritSat("SATISFIED");
                            }
                        }
                    }
                    if (text[i].Contains("END OF DOCUMENT"))//indicates the end of the student file
                    {
                        newStudent = true;
                        t2 = i;
                        string[] copy = new string[t2-t1];
                        Array.Copy(text, t1, copy, 0, t2-t1);
                        stud.setTranscript(copy);
                    }
                }
                return listStudent;
            }        
        }
    }   