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
        public static requiredClasses Obj;

        public  static requiredClasses getObj()//makes the required classes for the GUI to be functional
        {
            return Obj;
        }
        public static void setObj(requiredClasses Object)
        {
            Obj = Object;
        }

        public static void setTotalEU(List<Student> stud)//corrects the case where a writing class taken from a previous semester is still in progress
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
        public static void modifyEU(List<Student> stud)//this sets the Enrollment units to be uniform with the changes made from the function above
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
        public static void Start(requiredClasses Obj)
        {
            Obj.splitRequiredClasses();
            string[] lines = System.IO.File.ReadAllLines(@Obj.getStudentPath()); //use studentPathway
            ArrayList newDoc = LineSplit.splitLine(lines);//splits the whole student file into one column for ease of parsing
            string[] oneColumn = (string[])newDoc.ToArray(typeof(string));
            List<Student> studentList = ParseData.createStudent(oneColumn);//creates the list of students with their semesters and courses
            modifyEU(studentList); //fixes the in progress semester case
            ParseData.partTime(studentList);//sees if each student has been part time the whole time through
            ParseData.setTotalCredComplete(studentList);//sets all of their credits
            foreach (Student x in studentList)//sets all of the information into the student for generating the excel
            {
                requiredCourseMethods.checkRequiredCourses(x, Obj);
                requiredCourseMethods.checkSkills(x, Obj);
                requiredCourseMethods.checkWriting(x, Obj);
            }
            ParseData.GenExcel(studentList, Obj);//generates the excel
            string currentDir = Directory.GetCurrentDirectory();
            createTextDoc.createText(currentDir, studentList, Obj);//creates the text doc
            Application.Exit();
        }
    }
}
