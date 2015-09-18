using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSplit;
using System.IO;
using classesForLibraryExcel;
using System.Collections;
using LibraryProject;
using System.Windows.Forms;

namespace LibraryProject
{
    class MainClass
    {
        public static RequiredClasses Obj;

        public static string folder;

        public  static RequiredClasses getObj()
        {
            return Obj;
        }
        public static void setObj(RequiredClasses Object)
        {
            Obj = Object;
        }

        public static void setTotalEU(List<Student> stud)
        {
            for (int m = 0; m < stud.Count; m++)
            {
                double sum = 0.0;
                List<Semester> semList = stud[m].getStudentSemesters();
                for (int i = 0; i < semList.Count; i++)
                {
                    sum += semList[i].getEUnits();
                }
                stud[m].setEnrollUnits(sum);
            }
        }
        public static void modifyEU(List<Student> stud)
        {
            for (int i = 0; i < stud.Count; i++)
            {
                List<Semester> semList = stud[i].getStudentSemesters();
                for (int j = 0; j < semList.Count; j++)
                {
                    for (int k = j+1; k < semList.Count; k++)
                    {
                        if (semList[j].getSemesterName().Equals(semList[k].getSemesterName()) && !semList[j].getSemesterName().Equals("NON-GW"))
                        {
                            int newEU = 0;
                            int first = 0;
                            int second = 0;
                            newEU += semList[j].getCreditHours();
                            first = newEU;
                            newEU += semList[k].getCreditHours();
                            second = semList[k].getCreditHours();
                            semList[j].setHalfInProg(true);
                            semList[j].setProgComp(second, first);
                            semList[j].setCreditHours(newEU);
                            semList[k].setEUnits(0.0);
                        }
                    }
                }
            }
            setTotalEU(stud);
        }
        public static void Start(RequiredClasses Obj, String folder)
        {
            Obj.splitRequiredClasses();
            string[] lines = System.IO.File.ReadAllLines(@Obj.getStudentPath()); //use studentPathway
            ArrayList newDoc = LineSplit.splitLine(lines);
            string[] oneColumn = (string[])newDoc.ToArray(typeof(string));
            List<Student> studentList = ParseData.createStudent(oneColumn);
            modifyEU(studentList); //fixes the in progress semester case
            ParseData.partTime(studentList);
            ParseData.setTotalCredComplete(studentList, Obj);
            foreach (Course x in Obj.blank)
            {
                Console.WriteLine(x.getCourseName());
            }
            foreach (Student x in studentList)
            {
                requiredCourseMethods.checkRequiredCourses(x, Obj);
                requiredCourseMethods.checkSkills(x, Obj);
                requiredCourseMethods.checkWriting(x, Obj);
            }
            ParseData.GenExcel(studentList, Obj, folder);
            //string currentDir = Form1.getFolder();
            createTextDoc.createText(folder, studentList, Obj);
            Application.Exit();
        }

        public static void startFull(RequiredClasses Obj, String folder)
        {
            Obj.splitRequiredClasses();
            string[] lines = System.IO.File.ReadAllLines(@Obj.getStudentPath()); //use studentPathway
            ArrayList newDoc = LineSplit.splitLine(lines);
            string[] oneColumn = (string[])newDoc.ToArray(typeof(string));
            List<Student> studentList = FullClearance.createStudent(oneColumn);
            modifyEU(studentList); //fixes the in progress semester case

            ParseData.partTime(studentList);
            ParseData.setTotalCredComplete(studentList, Obj);
            foreach (Course x in Obj.blank)
            {
                Console.WriteLine(x.getCourseName());
            }
            foreach (Student x in studentList)
            {
                requiredCourseMethods.checkRequiredCourses(x, Obj);
                requiredCourseMethods.checkSkills(x, Obj);
                requiredCourseMethods.checkWriting(x, Obj);
            }
            FullClearance.sortStudents(studentList);
            List<Student> Cl = FullClearance.createTrFiles();
            ParseData.GenExcelFull(folder, FullClearance.getNew(), FullClearance.getCleared(), FullClearance.getPending());
            Application.Exit();
        }
    }
}
