﻿using System;
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
//test

namespace LibraryProject
{
    class FullClearance
    {
        static List<Student> Cleared = new List<Student>();
        static List<Student> New = new List<Student>();
        static List<Student> Pending = new List<Student>();
        public static List<double> creditConfigData = new List<double>();

        public static void setCreditConfigArray(RequiredClasses rq)
        {
            creditConfigData = rq.getCreditConfigData();
        }
        public static List<Student> getCleared()
        {
            return Cleared;
        }
        public static List<Student> getNew()
        {
            return New;
        }
        public static List<Student> getPending()
        {
            return Pending;
        }

        class ParseData
        {

            private static int n = 4;
            public static void setTotalCredComplete(List<Student> stud, RequiredClasses rc)
            {
                for (int i = 0; i < stud.Count; i++)
                {
                    double gradedInProg = 0.0;
                    List<Semester> semList = stud[i].getStudentSemesters();
                    int count = 0;//the count for total credits completed
                    double count1 = 0;//the count for credits in progess
                    int count2 = 0;//the count for graded credits
                    for (int j = 0; j < semList.Count; j++)
                    {
                        List<Course> courseList = semList[j].getCourseList();
                        for (int k = 0; k < courseList.Count; k++)
                        {
                            string sub = courseList[k].getGrade(); // HERE
                            if (sub.Equals("A+") || sub.Equals("A") || sub.Equals("A-") ||
                                sub.Equals("B") || sub.Equals("B+") || sub.Equals("B-") ||
                                sub.Equals("C") || sub.Equals("C+") || sub.Equals("C-") ||
                                sub.Equals("D") || sub.Equals("D+") || sub.Equals("D-"))//gets all the graded credits and adds them in
                            //ojdhfalskdjf;alskjd
                            {
                                count2 += (int)courseList[k].getCreds();
                            }
                        }
                        if (!semList[j].getInProg())
                        {
                            count += semList[j].getCreditHours();//indicates in progress credits
                        }
                        else
                        {
                            count1 += semList[j].getCreditHours();//indicates any other type of credit that doesnt have a grade and isnt in progress
                            gradedInProg = count1;

                            for (int n = 0; n < semList[j].getCourseList().Count; n++)
                            {
                                for (int m = 0; m < rc.getnonLetterGraded().Count; m++)
                                {
                                    string studName = semList[j].getCourseList()[n].getCourseNum();
                                    string rcName = rc.getnonLetterGraded()[m];
                                    if (rcName.Contains(studName))
                                    {
                                        gradedInProg = gradedInProg - semList[j].getCourseList()[n].getCreds();
                                    }
                                }
                            }
                        }
                        if (semList[j].getEUnits() == 0)//indicates an in progress writing class
                        {
                            count -= semList[j].getCreditHours();
                        }
                    }
                    stud[i].setGradedCreds(count2);//sets all the respective credits to the student
                    stud[i].setCredsInProgress(count1);
                    stud[i].setTotCred(count);
                    stud[i].setgradedcredsInProgess(gradedInProg);
                }
            }
            public void setGradedCreds(Student stud)//don't know if this is being used but it seems to just set the graded credits, seems to repeat the code above
            //but just for graded credits
            {
                List<Semester> semList = stud.getStudentSemesters();
                for (int i = 0; i < semList.Count; i++)
                {
                    if (semList[i].getInProg())
                    {
                        double sum = 0.0;
                        List<Course> courseList = semList[i].getCourseList();
                        for (int j = 0; j < courseList.Count; j++)
                        {
                            sum += courseList[j].getCreds();
                        }
                        stud.addGradedCreds(sum);
                    }
                }
            }

            
            public static void createSemester(string[] text, Student stud, int i)//method that sets the list of semesters into the students
            {
                List<Semester> semesterList = new List<Semester>();
                int endIndex = text.Length;
                for (int m = i; m < text.Length; m++)
                {
                    if (text[m].Contains("TOTAL INSTITUTION"))//finds the graded credits listed for the semester
                    {
                        string gradedCreds = text[m].Substring(29, 6);
                        gradedCreds = gradedCreds.Trim();
                        double gradeCreds = Double.Parse(gradedCreds);
                        stud.setGradedCreds(gradeCreds);
                    }
                    if (text[m].Contains("END OF DOCUMENT"))//indicates the end of the student file and gets their gpa
                    {
                        if (text[m - 1].Contains("OVERALL"))
                        {
                            string GPA = text[m - 1].Substring(47, text[m - 1].Length - 47);
                            GPA = GPA.Trim();
                            double gpa = Double.Parse(GPA);
                            stud.setGPA(gpa);
                        }
                        else if (text[m - 2].Contains("OVERALL"))
                        {
                            string GPA = text[m - 2].Substring(47, text[m - 2].Length - 47);
                            GPA = GPA.Trim();
                            double gpa = Double.Parse(GPA);
                            stud.setGPA(gpa);
                        }
                        endIndex = m;
                        break;
                    }
                }
                for (int j = i; j < endIndex; j++)
                {
                    if (text[j].Contains("Fall") && !text[j].Contains("Admit")) //checks to see what kind of student stud is, if he is not law, then we don't have 
                    {                                                           //to worry about getting his classes
                        Semester reusable = new Semester();
                        string semesterName = text[j].Substring(0, 9);
                        reusable.setSemesterName(semesterName);//gets and sets the semester name
                        if (stud.getLaw())
                        {
                            for (int k = j; k < endIndex; k++)
                            {

                                if ((text[k].Contains("LAW") || text[k].Contains("EXCH")) && !text[k].Contains("EFV"))//shows that a law course was found and then gets all the info for it
                                //and stores it into the student's respective semester
                                {
                                    Course reuse = new Course();
                                    reuse.setCourseName(text[k].Substring(4, 25));
                                    string Coursecred = text[k].Substring(38, 4);
                                    Coursecred = Coursecred.Trim();
                                    double courseCred = Double.Parse(Coursecred);
                                    reuse.setCreds(courseCred);
                                    string courseNum = text[k].Substring(5, 4).Trim();

                                    reuse.setCourseNum(courseNum);
                                    string grade = text[k].Substring(43, text[k].Length - 43);
                                    grade = grade.Trim();
                                    if (!grade.Contains("--"))
                                    {
                                        reuse.setGrade(grade);
                                    }
                                    if (text[k].Contains("EXCH"))
                                    {
                                        Coursecred = text[k].Substring(37, 5);
                                        Coursecred = Coursecred.Trim();
                                        courseCred = Double.Parse(Coursecred);
                                        reuse.setCreds(courseCred);
                                    }
                                    reusable.addCourse(reuse);
                                }
                                if (text[k].Contains("Total Transfer")) break;
                                if (text[k].Contains("Credits In Progress"))//sets the in progress credits if found in the semester
                                {
                                    int index = text[k].IndexOf(".");
                                    string creditHours = text[k].Substring(37, index - 37);
                                    creditHours = creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    reusable.setCreditHours(cHours);
                                    reusable.setInProg(true);
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                                if (text[k].Contains("Ehrs"))//indicates the total credits for the semester
                                {
                                    int index = text[k].IndexOf(".");
                                    int length = index - 8;
                                    string creditHours = text[k].Substring(8, length);
                                    creditHours = creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    reusable.setCreditHours(cHours);
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                            }
                        }
                    }
                    if (text[j].Contains("Spring") && !text[j].Contains("Admit"))//follows the same rules as above except for the spring semester
                    {
                        Semester reusable = new Semester();
                        string semesterName = text[j].Substring(0, 11);
                        reusable.setSemesterName(semesterName);
                        if (stud.getLaw())
                        {
                            for (int k = j; k < endIndex; k++)
                            {
                                if ((text[k].Contains("LAW") || text[k].Contains("EXCH")) && !text[k].Contains("EFV"))
                                {
                                    Course reuse = new Course();
                                    reuse.setCourseName(text[k].Substring(4, 25));
                                    string Coursecred = text[k].Substring(38, 4);
                                    Coursecred = Coursecred.Trim();
                                    double courseCred = Double.Parse(Coursecred);
                                    reuse.setCreds(courseCred);
                                    string courseNum = text[k].Substring(5, 4);
                                    courseNum = courseNum.Trim();
                                    reuse.setCourseNum(courseNum);                  //add grade, starts at index 43
                                    string grade = text[k].Substring(43, text[k].Length - 43);
                                    grade = grade.Trim();
                                    if (!grade.Contains("--"))
                                    {
                                        reuse.setGrade(grade);
                                    }
                                    if (text[k].Contains("EXCH"))
                                    {
                                        Coursecred = text[k].Substring(37, 5);
                                        Coursecred = Coursecred.Trim();
                                        courseCred = Double.Parse(Coursecred);
                                        reuse.setCreds(courseCred);
                                    }
                                    reusable.addCourse(reuse);
                                }
                                if (text[k].Contains("Total Transfer")) break;
                                if (text[k].Contains("Credits In Progress"))
                                {
                                    int index = text[k].IndexOf(".");
                                    string creditHours = text[k].Substring(37, index - 37);
                                    creditHours = creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    reusable.setCreditHours(cHours);
                                    reusable.setInProg(true);
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                                if (text[k].Contains("Ehrs"))
                                {
                                    int index = text[k].IndexOf(".");
                                    int length = index - 8;
                                    string creditHours = text[k].Substring(8, length);
                                    creditHours = creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    reusable.setCreditHours(cHours);
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                            }
                        }
                    }
                    if (text[j].Contains("Summer") && !text[j].Contains("Admit"))//follows the same rules as the previous semesters
                    {
                        Semester reusable = new Semester();
                        string semesterName = text[j].Substring(0, 11);
                        reusable.setSemesterName(semesterName);
                        if (stud.getLaw())
                        {
                            for (int k = j; k < endIndex; k++)
                            {
                                if ((text[k].Contains("LAW") || text[k].Contains("EXCH")) && !text[k].Contains("EFV"))
                                {
                                    Course reuse = new Course();
                                    reuse.setCourseName(text[k].Substring(4, 25));
                                    string Coursecred = text[k].Substring(38, 4);
                                    Coursecred = Coursecred.Trim();
                                    double courseCred = Double.Parse(Coursecred);
                                    reuse.setCreds(courseCred);
                                    string courseNum = text[k].Substring(5, 4);
                                    courseNum = courseNum.Trim();
                                    reuse.setCourseNum(courseNum);
                                    string grade = text[k].Substring(43, text[k].Length - 43);
                                    grade = grade.Trim();
                                    if (!grade.Contains("--"))
                                    {
                                        reuse.setGrade(grade);
                                    }
                                    if (text[k].Contains("EXCH"))
                                    {
                                        Coursecred = text[k].Substring(37, 5);
                                        Coursecred = Coursecred.Trim();
                                        courseCred = Double.Parse(Coursecred);
                                        reuse.setCreds(courseCred);
                                    }
                                    reusable.addCourse(reuse);
                                }
                                if (text[k].Contains("Total Transfer")) break;
                                if (text[k].Contains("Credits In Progress"))
                                {
                                    int index = text[k].IndexOf(".");
                                    string creditHours = text[k].Substring(37, index - 37);
                                    creditHours = creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    reusable.setCreditHours(cHours);
                                    reusable.setInProg(true);
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                                if (text[k].Contains("Ehrs"))
                                {
                                    int index = text[k].IndexOf(".");
                                    int length = index - 8;
                                    string creditHours = text[k].Substring(8, length);
                                    creditHours = creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    reusable.setCreditHours(cHours);
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                            }
                        }
                    }
                    /***************************non-gw history - need these courses for requirement checking***************************************/
                    if (text[j].Contains("NON-GW HISTORY:"))//shows that this student has had a transfer semester
                    {
                        Semester reusable = new Semester();//creates and stores the semester
                        String semesterName = "NON-GW";
                        reusable.setSemesterName(semesterName);
                        if (text[j + 1].Contains("20"))
                        {
                            semesterName = text[j + 1].Substring(0, 11);
                            semesterName = semesterName.Trim();
                            reusable.setSemesterName("NON-GW " + semesterName);
                        }
                        else if (text[j + 2].Contains("20"))
                        {
                            semesterName = text[j + 2].Substring(0, 11);
                            semesterName = semesterName.Trim();
                            reusable.setSemesterName("NON-GW " + semesterName);
                        }
                        if (stud.getLaw())//only runs if this student is a law student
                        {
                            for (int k = j; k < endIndex; k++)
                            {
                                if (text[k].Contains("LAW") && !text[k].Contains("EFV"))
                                {
                                    Course reuse = new Course();
                                    //reuse.setCourseName("LAW");
                                    reuse.setCourseName(text[k].Substring(4, 25));
                                    string Coursecred = text[k].Substring(37, 5);
                                    Coursecred = Coursecred.Trim();
                                    double courseCred = Double.Parse(Coursecred);
                                    reuse.setCreds(courseCred);
                                    string courseNum = text[k].Substring(5, 4);
                                    courseNum = courseNum.Trim();
                                    reuse.setCourseNum(courseNum);
                                    string grade = text[k].Substring(43, text[k].Length - 43);
                                    grade = grade.Trim();
                                    if (!grade.Contains("--"))
                                    {
                                        reuse.setGrade(grade);
                                    }
                                    reusable.addCourse(reuse);
                                }
                                if (text[k].Contains("Credits In Progress"))
                                {
                                    int index = text[k].IndexOf(".");
                                    string creditHours = text[k].Substring(37, index - 37);
                                    creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    reusable.setCreditHours(cHours);
                                    reusable.setInProg(true);
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                                if (text[k].Contains("Total Transfer"))
                                {
                                    int index = text[k].IndexOf(":");
                                    string creditHours = text[k].Substring(index + 1, 4);
                                    creditHours = creditHours.Trim();
                                    int cHours = Int32.Parse(creditHours);
                                    if (cHours <= 12)
                                    {
                                        reusable.setCreditHours(cHours);
                                    }
                                    else if (cHours < 24)
                                    {
                                        int otherCreds = cHours - 12;
                                        reusable.setCreditHours(cHours);
                                        switch (otherCreds)
                                        {
                                            case 1:
                                                reusable.addEUnits(0.075);
                                                break;
                                            case 2:
                                                reusable.addEUnits(0.15);
                                                break;
                                            case 3:
                                                reusable.addEUnits(0.2);
                                                break;
                                            case 4:
                                                reusable.addEUnits(0.3);
                                                break;
                                            case 5:
                                                reusable.addEUnits(0.35);
                                                break;
                                            case 6:
                                                reusable.addEUnits(0.4);
                                                break;
                                            case 7:
                                                reusable.addEUnits(0.5);
                                                break;
                                            case 8:
                                                reusable.addEUnits(0.6);
                                                break;
                                            case 9:
                                                reusable.addEUnits(0.65);
                                                break;
                                            case 10:
                                                reusable.addEUnits(0.7);
                                                break;
                                            case 11:
                                                reusable.addEUnits(0.8);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        reusable.setCreditHours(cHours);
                                    }
                                    stud.addOneSemester(reusable);
                                    break;
                                }
                            }
                        }
                    }

                    for (int v = 0; v < semesterList.Count; v++)
                    {
                        Double amtCreds = 0;
                        List<Course> courses = semesterList[i].getCourseList();
                        for (int n = 0; n < courses.Count(); n++)
                        {
                            amtCreds += courses[n].getCreds();
                        }
                        if (amtCreds == semesterList[v].getCreditHours())
                        {
                            semesterList[v].setTookNonLawFalse();
                        }
                    }
                }
            }
            public static void partTime(List<Student> studentlist)//the method that determines if the student was part time for the whole time at law school
            //this is the condition that allows the student to pass with 5.9 EUnits
            {
                bool allPart = false;
                foreach (Student stud in studentlist)
                {
                    foreach (Semester x in stud.getStudentSemesters())
                    {
                        if (!x.getParttime())
                        {
                            allPart = false;
                            break;
                        }
                        else
                        {
                            allPart = true;
                            continue;
                        }
                    }
                    stud.setPartTime(allPart);
                }
            }
        }
        public static List<Student> createStudent(string[] text)
        {
            bool newStudent = true;
            List<Student> listStudent = new List<Student>();
            int t1 = -1;
            int t2 = -1;
            Student temp = new Student();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Contains("Record of") && newStudent)//indicates a new student has been found
                {
                    t1 = i - 2;
                    newStudent = false;
                    string Gwid = text[i - 2].Substring(13, 9);
                    string name = text[i].Substring(11, text[i].Length - 11);//obtains the name of the student
                    name = name.Trim();
                    Student stud = new Student(name);
                    stud.setGWid(Gwid);
                    if (text[i + 1].Contains("Law"))//indicates that the student is a law student
                    {
                        stud.setLaw(true);
                        ParseData.createSemester(text, stud, i);
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
                        if (text[x].Contains("SKILLS REQUIREMENT MET"))//this is the portion that needs to be changed
                        {                                              //we need to parse in the skills and writing req classes
                            stud.setSkillSat("SATISFIED");             //from the provided text file and then see if the student has taken
                        }                                              //6 credits of skills/writing courses, if they have, clear them, else don't
                        if (text[x].Contains("WRITING REQUIREMENT MET"))
                        {
                            stud.setWritSat("SATISFIED");
                        }
                    }
                    temp = stud;
                }
                if (text[i].Contains("END OF DOCUMENT"))//indicates the end of the student file
                {
                    newStudent = true;
                    t2 = i;
                    string[] copy = new string[t2 - t1];
                    Array.Copy(text, t1, copy, 0, t2 - t1);
                    temp.setTranscript(copy);
                }
            }
            return listStudent;
        }
        public static void adjusttrack(List<Student> studs, Boolean isFullClearance) {
            foreach (Student student in studs)
            {
                if (isFullClearance) { 
                    if (student.getCredsInProgress() != 0)
                    {
                        student.setTrack();
                    }
                }
                if (!(student.getCredsInProgress() + student.getGradedCreds() >= creditConfigData[1]))//graded credit req

                {
                    student.setTrack();
                }
                if (!(student.getSkillSat().Contains("SATISFIED") && student.getWritSat().Contains("SATISFIED")))
                {
                    student.setTrack();
                }
                if (student.getGPA() < creditConfigData[3])//gpa requirement
                {
                    student.setTrack();
                }
                int count2 = 0;
                if (student.getPartTime() == true)
                {
                    if (student.getEnrollUnits() >= creditConfigData[7])//transfer eu
                    {
                        count2++;
                    }
                }
                else if (student.getEnrollUnits() >= creditConfigData[6])//reg eu
                {
                    count2++;
                }
                if (count2 == 0)
                {
                    student.setTrack();
                }
                if (student.getTotCred() + student.getCredsInProgress() < creditConfigData[0])//indicates total credits
                {
                    student.setTrack();
                }
            }
        }
        public static void sortStudents(List<Student> studs, string folder)
        {
            string path = folder + "/ClearedGWIDS.txt";
            if (!File.Exists(path)) {
                System.IO.File.Create(path).Close();           
                //File.Create(path);
            }
            adjusttrack(studs, true);
            string[] ctext = System.IO.File.ReadAllLines(@path);
            foreach (Student s in studs)
            {
                if (s.getTrack().Contains("OFF"))
                {
                    Pending.Add(s);
                }
                else if (s.getTrack().Contains("ON TRACK"))
                {
                    Cleared.Add(s);
                    bool alreadycleared = false;
                    for (int i = 0; i < ctext.Length; i++)
                    {
                        if (ctext[i].Contains(s.getGWid()))
                        {
                            alreadycleared = true;
                            break;
                        }
                    }
                    if (alreadycleared == false)
                    {
                        New.Add(s);
                        using (StreamWriter text = File.AppendText(path))
                        {
                            text.WriteLine(s.getGWid());
                        }
                    }
                }
            }
        }
        public static List<Student> createTrFiles(string path)
        {
            string cleared = path + "/CLEARED.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(cleared, false))
            {
                foreach (Student s in Cleared)
                {
                    file.WriteLine();
                    foreach (String l in s.getTranscript()) 
                    {
                        file.WriteLine(l);
                    }
                    file.WriteLine("\n");
                    file.WriteLine("\n");
                }
            }
            System.Diagnostics.Process.Start(@cleared);
            string newlycl = path + "/NEW.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(newlycl, false))
            {
                foreach (Student s in New)
                {
                    file.WriteLine();
                    foreach (String l in s.getTranscript())
                    {
                        file.WriteLine(l);
                    }
                    file.WriteLine("\n");
                    file.WriteLine("\n");
                }
            }
            System.Diagnostics.Process.Start(@newlycl);
            string pending = path + "/PENDING.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pending, false))
            {
                foreach (Student s in Pending)
                {
                    file.WriteLine();
                    foreach (String l in s.getTranscript())
                    {
                        file.WriteLine(l);
                    }
                    file.WriteLine("\n");
                    file.WriteLine("\n");
                }
            }
            System.Diagnostics.Process.Start(@pending);
            return Cleared;
        }
        public void GenFullClearance(List<Student> clearedStuds, string folder) { 
            string path = folder + "ClearedStuds.txt";
            if (!File.Exists(path))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
                {
                    foreach (Student stud in Cleared)
                    {
                        file.WriteLine(stud.getGWid() + "\n");
                        //add each student to newly cleared spreadsheetaru
                    }
                }
            }
            else {
                StreamWriter w = File.AppendText(path);
                string[] GWIDS = File.ReadAllLines(path);
                for (int i = 0; i < Cleared.Count(); i++) {
                    string studGWID = Cleared[i].getGWid();
                    foreach (string gwid in GWIDS) {
                        if (studGWID.Equals(gwid)) {
                            i++;
                            break;
                        }
                    }
                    w.WriteLine(studGWID);
                    //add student to newly cleared student worksheet
                }
            }  
        } 
    }
}



