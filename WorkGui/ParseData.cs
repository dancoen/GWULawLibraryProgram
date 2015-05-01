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


namespace LibraryProject
{
    class ParseData
    {
        private static int n = 4;
        public static void setTotalCredComplete(List<Student> stud)
        {
            for (int i = 0; i < stud.Count; i++)
            {
                //string temp = "Football";                                               //unnecessary
                List<Semester> semList = stud[i].getStudentSemesters();
                int count = 0;//the count for total credits completed
                int count1 = 0;//the count for credits in progess
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
                            sub.Equals("D") || sub.Equals("D+") || sub.Equals("D-") || 
                            sub.Equals("H") || sub.Equals("P") || sub.Equals("LP"))
                        {
                            count2 += (int)courseList[k].getCreds();
                        }
                    }
                        if (!semList[j].getInProg())
                        {
                            count += semList[j].getCreditHours();
                        }
                        else
                        {
                            count1 += semList[j].getCreditHours();
                        }
                    if (semList[j].getEUnits() == 0)
                    {
                        count -= semList[j].getCreditHours();
                    }
                }
                stud[i].setGradedCreds(count2);
                stud[i].setCredsInProgress(count1);
                stud[i].setTotCred(count);
            }
        }
        public void setGradedCreds(Student stud)
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
        public static List<Student> createStudent(string[] text)
        {
            bool newStudent = true;
            List<Student> listStudent = new List<Student>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Contains("Record of") && newStudent)
                {
                    newStudent = false;
                    string Gwid = text[i - 2].Substring(13, 9);
                    string name = text[i].Substring(11, text[i].Length - 11);
                    name = name.Trim();
                    Student stud = new Student(name);
                    stud.setGWid(Gwid);
                    if (text[i + 1].Contains("Law"))
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
                    for (int x = i; x < i + 17; x++)
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
                if (text[i].Contains("END OF DOCUMENT"))
                {
                    newStudent = true;
                }
            }
                return listStudent;
        }
        public static void createSemester(string[] text, Student stud, int i)
        {
            List<Semester> semesterList = new List<Semester>();
            int endIndex = text.Length;
            for (int m = i; m < text.Length; m++)
            {
                if (text[m].Contains("TOTAL INSTITUTION"))
                {
                    string gradedCreds = text[m].Substring(29, 6);
                    gradedCreds = gradedCreds.Trim();
                    double gradeCreds = Double.Parse(gradedCreds);
                    stud.setGradedCreds(gradeCreds);
                }
                if (text[m].Contains("END OF DOCUMENT"))
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
                    reusable.setSemesterName(semesterName);
                    if (stud.getLaw())
                    {
                        for (int k = j; k < endIndex; k++)
                        {

                            if (text[k].Contains("LAW") && !text[k].Contains("EFV"))
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
                if (text[j].Contains("Spring") && !text[j].Contains("Admit"))
                {
                    Semester reusable = new Semester();
                    string semesterName = text[j].Substring(0, 11);
                    reusable.setSemesterName(semesterName);
                    if (stud.getLaw())
                    {
                        for (int k = j; k < endIndex; k++)
                        {
                            if (text[k].Contains("LAW") && !text[k].Contains("EFV"))
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
                if (text[j].Contains("Summer") && !text[j].Contains("Admit"))
                {
                    Semester reusable = new Semester();
                    string semesterName = text[j].Substring(0, 11);
                    reusable.setSemesterName(semesterName);
                    if (stud.getLaw())
                    {
                        for (int k = j; k < endIndex; k++)
                        {
                            if (text[k].Contains("LAW") && !text[k].Contains("EFV"))
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
                    if (text[j].Contains("NON-GW HISTORY:"))
                    {
                        Semester reusable = new Semester();
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
                        if (stud.getLaw())
                        {
                            for (int k = j; k < endIndex; k++)
                            {
                                if (text[k].Contains("LAW") && !text[k].Contains("EFV"))
                                {
                                    Course reuse = new Course();
                                    //reuse.setCourseName("LAW");
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
        public static void partTime(List<Student> studentlist)
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
        public static void GenExcel(List<Student> studs, requiredClasses Object)
        {
            using (var package = new ExcelPackage(new System.IO.FileInfo("LawPrereqs.xlsx")))
            {
                ExcelWorksheet worksheet;
                var testWork = package.Workbook.Worksheets["Students"];
                if (testWork == null)
                {
                    worksheet = package.Workbook.Worksheets.Add("Students");
                }
                else
                {
                    package.Workbook.Worksheets.Delete(package.Workbook.Worksheets["Students"].Index);
                    worksheet = package.Workbook.Worksheets.Add("Students");
                }
                worksheet.Cells[1, 1].Value = "GWID";
                worksheet.Cells[1, 2].Value = "On/Off Track";
                worksheet.Cells[1, 3].Value = "Name";
                List<String> reqClasses = Object.getRequired();
                foreach (String x in reqClasses)
                {
                    worksheet.Cells[1, n].Value = x.Substring(0,4) + " status";
                    worksheet.Cells[1, n + 1].Value = x.Substring(0,4) + " grade";
                    int index = reqClasses.IndexOf(x);
                    if (index+1 == reqClasses.Count())
                    {
                        n++;
                    }
                    else
                    {
                        n += 2;
                    }
                }
                worksheet.Cells[1, n + 1].Value = "Total Credits Status";
                worksheet.Cells[1, n + 2].Value = "Total Credits Completed";
                worksheet.Cells[1, n + 3].Value = "Total Credits in Progress";
                worksheet.Cells[1, n + 4].Value = "Total Credits";
                worksheet.Cells[1, n + 5].Value = "Graded Credit Status";
                worksheet.Cells[1, n + 6].Value = "Graded Credits Completed";
                worksheet.Cells[1, n + 7].Value = "Graded Credits in Progress";
                worksheet.Cells[1, n + 8].Value = "Graded Credits Total";
                worksheet.Cells[1, n + 9].Value = "Skills Requirement Status";
                worksheet.Cells[1, n + 10].Value = "Skills Requirement Legend";
                worksheet.Cells[1, n + 11].Value = "Writing Requirement Status";
                worksheet.Cells[1, n + 12].Value = "Writing Requirement Legend";
                worksheet.Cells[1, n + 13].Value = "Minimum GPA Status";
                worksheet.Cells[1, n + 14].Value = "Current GPA";
                worksheet.Cells[1, n + 15].Value = "Enrollment Units Status";
                worksheet.Cells[1, n + 16].Value = "Enrollment Units";
                worksheet.Cells[1, n + 17].Value = "Enrollment Units Notes";
                worksheet.Cells[1, n + 18].Value = "Sem_1";
                worksheet.Cells[1, n + 19].Value = "Sem_1_RU";
                worksheet.Cells[1, n + 20].Value = "Sem_1_Credits";
                worksheet.Cells[1, n + 21].Value = "Sem_2";
                worksheet.Cells[1, n + 22].Value = "Sem_2_RU";
                worksheet.Cells[1, n + 23].Value = "Sem_2_Credits";
                worksheet.Cells[1, n + 24].Value = "Sem_3";
                worksheet.Cells[1, n + 25].Value = "Sem_3_RU";
                worksheet.Cells[1, n + 26].Value = "Sem_3_Credits";
                worksheet.Cells[1, n + 27].Value = "Sem_4";
                worksheet.Cells[1, n + 28].Value = "Sem_4_RU";
                worksheet.Cells[1, n + 29].Value = "Sem_4_Credits";
                worksheet.Cells[1, n + 30].Value = "Sem_5";
                worksheet.Cells[1, n + 31].Value = "Sem_5_RU";
                worksheet.Cells[1, n + 32].Value = "Sem_5_Credits";
                worksheet.Cells[1, n + 33].Value = "Sem_6";
                worksheet.Cells[1, n + 34].Value = "Sem_6_RU";
                worksheet.Cells[1, n + 35].Value = "Sem_6_Credits";
                worksheet.Cells[1, n + 36].Value = "Sem_7";
                worksheet.Cells[1, n + 37].Value = "Sem_7_RU";
                worksheet.Cells[1, n + 38].Value = "Sem_7_Credits";
                worksheet.Cells[1, n + 39].Value = "Sem_8";
                worksheet.Cells[1, n + 40].Value = "Sem_8_RU";
                worksheet.Cells[1, n + 41].Value = "Sem_8_Credits";
                worksheet.Cells[1, n + 42].Value = "Sem_9";
                worksheet.Cells[1, n + 43].Value = "Sem_9_RU";
                worksheet.Cells[1, n + 44].Value = "Sem_9_Credits";
                worksheet.Cells[1, n + 45].Value = "Sem_10";
                worksheet.Cells[1, n + 46].Value = "Sem_10_RU";
                worksheet.Cells[1, n + 47].Value = "Sem_10_Credits";
                worksheet.Cells[1, n + 48].Value = "Notes";
               
                
                int count = 2;
                foreach (Student student in studs)
                {
                    worksheet.Cells[count, 1].Value = student.getGWid();
                    worksheet.Cells[count, 3].Value = student.getStudentName();
                    string totCredStat;
                    if (student.getTotCred() >= 84)
                    {
                        totCredStat = "SATISFIED";
                    }
                    else if (student.getTotCred() + student.getCredsInProgress() >= 84)
                    {
                        totCredStat = "ON TRACK";
                    }
                    else {
                        totCredStat = "OFF TRACK";
                    }
                    worksheet.Cells[count, n + 1].Value = totCredStat;
                    worksheet.Cells[count, n + 2].Value = student.getTotCred();
                    worksheet.Cells[count, n + 3].Value = student.getCredsInProgress();
                    worksheet.Cells[count, n + 4].Value = student.getTotCred() + student.getCredsInProgress();
                    if (student.getGradedCreds() >= 67)
                    {
                        worksheet.Cells[count, n + 5].Value = "SATISFIED";
                    }
                    else if (student.getCredsInProgress() + student.getGradedCreds() >= 67)
                    {
                        worksheet.Cells[count, n + 5].Value = "ON TRACK";
                    }
                    else
                    {
                        student.setTrack();
                        worksheet.Cells[count, n + 5].Value = "OFF TRACK";
                    }
                    worksheet.Cells[count, n + 6].Value = student.getGradedCreds();
                    worksheet.Cells[count, n + 7].Value = student.getCredsInProgress();
                    worksheet.Cells[count, n + 8].Value = student.getGradedCreds() + student.getCredsInProgress();
                    worksheet.Cells[count, n + 9].Value = student.getSkillSat();
                    if (student.getSkillSat().Contains("OFF TRACK"))
                    {
                        student.setTrack();
                        worksheet.Cells[count, n + 10].Value = "no legend / no course in prog.";
                    }
                    else if (student.getSkillSat().Contains("SATISFIED"))
                    {
                        worksheet.Cells[count, n + 10].Value = "SKILLS REQUIREMENT MET legend";
                    }
                    else
                    {
                        foreach (Course x in student.getAllSkills())
                        {
                            if (x.getStatus().Contains("ON TRACK"))
                            {
                                worksheet.Cells[count, n + 10].Value = "course " + x.getCourseNum() + " in prog.";
                            }
                        }
                    }
                    worksheet.Cells[count, n + 11].Value = student.getWritSat();
                    if(student.getWritSat().Contains("OFF TRACK")) {
                        student.setTrack();
                        worksheet.Cells[count,n+12].Value = "no legend / no course in prog;";
                    }
                    else if (student.getWritSat().Contains("SATISFIED")) {
                        worksheet.Cells[count,n+12].Value = "WRITING REQUIREMENT MET legend";
                    }
                    else {
                        worksheet.Cells[count,n+11].Value = "ON TRACK";
                        foreach (Course x in student.getAllWriting())
                        {
                            if (x.getStatus().Contains("IN PROGRESS"))
                            {
                                worksheet.Cells[count, n + 12].Value = "course " + x.getCourseNum() + " in prog.";
                            }
                        }
                    }
                    if (student.getGPA() >= 1.67)
                    {
                        worksheet.Cells[count, n + 13].Value = "ON TRACK";
                    }
                    else
                    {
                        student.setTrack();
                        worksheet.Cells[count, n + 13].Value = "OFF TRACK";
                    }
                    worksheet.Cells[count, n + 14].Value = student.getGPA();
                    int count2 = 0;
                    string enrolltrack = "OFF TRACK";
                    if (student.getPartTime() == true)
                    {
                        if (student.getEnrollUnits() >= 5.9) { 
                            enrolltrack = "SATISFIED";
                            count2++;
                        }
                    }
                    else if (student.getEnrollUnits() >= 6.0) { 
                        enrolltrack = "SATISFIED";
                        count2++;
                    }
                    if(count2 == 0)
                    {
                        student.setTrack();
                    }
                    worksheet.Cells[count, n + 15].Value = enrolltrack;
                    worksheet.Cells[count, n + 16].Value = student.getEnrollUnits();
                    int semesterNum = 1;
                    
                    foreach (Semester semester in student.getStudentSemesters())
                    {
                        for (int h = 0; h < student.getreqcourses().Length; h++ )
                        {
                            Course course = student.getreqcourses()[h];
                            if (worksheet.Cells[1, 4].ToString().Contains("6202"))
                            {
                                worksheet.Cells[5, 5].Value = course.getCourseNum();
                            }
                            int i = 2;
                            while (!(worksheet.Cells[1, i].Value.ToString().Contains(((course.getCourseNum())))))
                            {
                                i++;
                                if (i > (n + 48))
                                {
                                    break;
                                }
                            }
                            if (i < (n + 48))
                            {
                                worksheet.Cells[count, i].Value = course.getStatus();
                                if (course.getStatus().Equals("OFF TRACK"))
                                {
                                    student.setTrack();
                                }
                                worksheet.Cells[count, i + 1].Value = course.getGrade();
                            }
                        }
                        int g = n + 17;
                        while (!(worksheet.Cells[1, g].Value.ToString().Contains("Sem_" + semesterNum.ToString())))
                        {
                            g++;
                            if (g > n + 48)
                            {
                                break;
                            }
                        }
                        if (g <= n + 48)
                        {
                            if (semester.getEUnits() > 0)
                            {
                                worksheet.Cells[count, g].Value = semester.getSemesterName();
                                worksheet.Cells[count, g + 1].Value = semester.getEUnits();
                                worksheet.Cells[count, g + 2].Value = semester.getCreditHours();
                                semesterNum++;
                            }
                        }
                        if (student.getnonLawBool() == true)
                        {
                            worksheet.Cells[count, n + 48].Value = "*** Note: Non-Law School courses on transcript ***";
                        }
                    }
                    if (student.getPartTime() == true)
                    {
                        worksheet.Cells[count, n + 17].Value = "Part-time";
                    }
                    worksheet.Cells[count, 2].Value = student.getTrack();
                    count++;      
                }
                for (int i = 1; i <= 73; i++)
                {
                    worksheet.Cells[1, i].Style.Font.Bold = true;
                }
                worksheet.Cells.AutoFitColumns();
                package.Save();
                string path = Directory.GetCurrentDirectory();
                string newPath = path + "/" + "LawPrereqs.xlsx";
                Process.Start(@newPath);
            }
        }       
    }
}