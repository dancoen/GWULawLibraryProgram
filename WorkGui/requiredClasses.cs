using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using classesForLibraryExcel;
using WindowsFormsApplication1;

namespace LibraryProject
{
    public class requiredClasses
    {
        public requiredClasses(string cp, string sp) {
            coursepath = cp;
            studentpath = sp;
            this.courseList = System.IO.File.ReadAllLines(@coursepath);
            this.lines = System.IO.File.ReadAllLines(@studentpath);
        }
        
        public  string coursepath; 
        public string studentpath; 
        public  string[] courseList;  
        private  List<string> required = new List<string>();
        public  List<string> writing = new List<string>();
        public  List<string> skills = new List<string>();
        public  List<Course> blank = new List<Course>();
        public  Course[] actual;
        public  string[] lines;
        public  int num;

        public  string getCoursepath()
        {
            return coursepath;
        }

        public  void setCoursePath(string path)
        {
            coursepath = path;
        }
        public  string getStudentPath()
        {
            return studentpath;
        }
        public  void setStudentPath(string path)
        {
            studentpath = path;
        }

        public  void splitRequiredClasses()
        {
            int x = splitCourseList(1, required);
            int y = splitCourseList(x, writing);
            int z = splitCourseList(y, skills);
            num = required.Count;
            setActual();
        }

        public  List<Course> getBlank() {
            return blank;
        }

        public  List<string> getRequired() {
            return required;
        }

        public  Course setAsDefault(string name)
        {
            Course blank = new Course();
            blank.setCourseName(name);
            blank.setCourseNum(name.Substring(0, 4));
            blank.setStatus("OFF TRACK");
            blank.setGrade("not compl. or in prog.");
            return blank;
        }
        public  void setActual()
        {
            Course[] A = new Course[required.Count];
            int i = 0;
            foreach (string x in required)
            {
                A[i] = new Course(x.Substring(0, 4), x, "not compl. or in prog.");
                A[i].setStatus("OFF TRACK");
                i++;
            }
            actual = A;
        }
        public  int splitCourseList(int j, List<string> list)
        {
            for (int i = j; i < courseList.Count(); i++)
            {
               while ((courseList[i].Count() != 0))
                {
                    if (courseList[i].Contains("COURSES:"))
                    {
                        return i + 1;
                    }
                    else if ((courseList[i] != "\n"))
                    {
                        list.Add(courseList[i]);
                    }
                    i++;
                }
            }
            return -1;
        }
    }

    public class requiredCourseMethods
    {
        public static Course[] checkRequiredCourses(Student student, requiredClasses Object)
        {
            Course[] actualcopy = new Course[Object.actual.Length];
            
            for (int i = 0; i < actualcopy.Length; i++)
            {
                actualcopy[i] = Object.actual[i];
            }
            List<Semester> semest = student.getStudentSemesters();

            for (int i = 0; i < semest.Count; i++)
            {
                List<Course> temp = semest[i].getCourseList();
                for (int k = 0; k < actualcopy.Length; k++)
                {
                    for (int j = 0; j < semest[i].getCourseList().Count; j++)
                    {
                        if (actualcopy[k].getCourseNum().Contains(temp[j].getCourseNum()))
                        {
                            actualcopy[k] = temp[j];
                            if (Object.getRequired()[k].Contains(temp[j].getCourseNum()) && 
                                (temp[j].getGrade().Equals("A") || temp[j].getGrade().Equals("A-") ||
                                 temp[j].getGrade().Equals("A+") ||temp[j].getGrade().Equals("B")  || 
                                 temp[j].getGrade().Equals("B-") ||temp[j].getGrade().Equals("B+") ||
                                 temp[j].getGrade().Equals("C") || temp[j].getGrade().Equals("C-") ||
                                 temp[j].getGrade().Equals("C+") ||temp[j].getGrade().Equals("D")  || 
                                 temp[j].getGrade().Equals("D-") ||temp[j].getGrade().Equals("D+") ||
                                 temp[j].getGrade().Equals("TR") || temp[j].Equals("H") || 
                                 temp[j].Equals("P") || temp[j].Equals("LP")))
                            {
                                actualcopy[k].setStatus("SATISFIED");
                            }
                            if ((temp[j].getGrade().Contains("Progress"))) { 
                                actualcopy[k].setStatus("ON TRACK");
                                actualcopy[k].setTrack("IN PROGRESS");
                            }
                            break;
                        }

                    }
                }
            }
            student.setreqcourses(actualcopy);
                return actualcopy;
        }

        public static void checkWriting(Student student, requiredClasses Object) 
        {
            List<Semester> semest = student.getStudentSemesters();
            for (int i = 0; i < semest.Count; i++)
            {
                if (semest[i].getInProg() && !student.getWritSat().Contains("SATISFIED"))
                {
                    for (int j = 0; j < semest[i].getCourseList().Count; j++)
                    {
                        List<Course> temp = semest[i].getCourseList();
                        for (int k = 0; k < Object.writing.Count; k++)
                        {
                            if (Object.writing[k].Contains(temp[j].getCourseNum()))
                            {
                                if (Object.writing[k].Contains("or more credits")) ///////////////////////////
                                {
                                    int b = Object.writing[k].Length;
                                    string str = Object.writing[k];
                                    int numCreds = -5;
                                    for (int n = 0; n < b; n++)
                                    {
                                        if (str[n] > '0' && str[n] <= '9') { 
                                            numCreds = n;
                                            break;
                                        }
                                    }
                                        if (temp[j].getCreds() < numCreds) { 
                                            student.setWritSat("OFF TRACK");
                                            student.addWriting(temp[j]);
                                            temp[j].setStatus("IN PROGRESS");
                                            continue;
                                        }
                                }                                                   ////////////////////////////////
                                temp[j].setStatus("IN PROGRESS");
                                student.setWritSat("ON TRACK");
                                student.addWriting(temp[j]);
                            }
                        }
                    }
                }
            }
        }

        public static void checkSkills(Student student, requiredClasses Object) 
        {
            List<Semester> semest = student.getStudentSemesters();
            for (int i = 0; i < semest.Count; i++)
            {
                if (semest[i].getInProg() && !student.getSkillSat().Contains("SATISFIED")) { 
                    for (int j = 0; j < semest[i].getCourseList().Count; j++)
                    {
                        List<Course> temp = semest[i].getCourseList();
                        for (int k = 0; k < Object.skills.Count; k++)
                        {
                            if (Object.skills[k].Contains(temp[j].getCourseNum()))
                            {
                                 temp[j].setStatus("ON TRACK");
                                 student.setSkillSat("ON TRACK");
                                 student.addSkills(temp[j]);
                            }
                        }
                    }
                }
            }
        }
    }
}