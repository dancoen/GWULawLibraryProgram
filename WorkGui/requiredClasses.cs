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
    public class RequiredClasses //This class takes inputs of a text document with required classes, skills, and writing. 
    {
        public RequiredClasses(string cp, string sp) { //Was made an object in order to be compatible with the GUI; takes the text file paths of the Courselist and the Studentlist docs
            coursepath = cp;
            studentpath = sp;
            this.courseList = System.IO.File.ReadAllLines(@coursepath);
            this.lines = System.IO.File.ReadAllLines(@studentpath);
        }
        public  string coursepath;
        public  string studentpath; 
        public  string[] courseList;  
        private List<string> required = new List<string>(); //arraylist of required course names
        public  List<string> writing = new List<string>();   //arraylist of required writing course names
        public  List<string> skills = new List<string>();    //arraylist of required skills course names
        public  List<string> nonLetterGraded = new List<string>();
        public List<string> getnonLetterGraded() { return nonLetterGraded; }
        public  List<Course> blank = new List<Course>();     //likely not needed
        public  Course[] actual;                             //arraylist of required Course objects that have been set with default values,--
                                                             //---so same order every time, only need to change appropriate data within for each student. This will happen later on.
        public  string[] lines;                              //arraylist for student list to parse through
        public  int num;                                     //number of required courses

        public  string getCoursepath()          //getter for course path
        {
            return coursepath;
        }

        public  void setCoursePath(string path) //setter for course path
        {
            coursepath = path;
        }
        public  string getStudentPath()         //getter for student path
        {
            return studentpath;
        }
        public  void setStudentPath(string path)//setter for student path
        {
            studentpath = path;
        }

        public  void splitRequiredClasses()         //splits the text file into required, writing, and skills
        {
            int x = splitCourseList(1, required);   //parses course list into the proper array, returns the index where required classes stop
            int y = splitCourseList(x, writing);    //parses course list into the proper array, returns idx where writing stops
            int z = splitCourseList(y, skills);     //parses course list into the proper array, returns idx where skills stop
            int last = splitCourseList(z, nonLetterGraded); //parses course list into the proper array, returns idx where nonlettergraded stops
            for (int i = 0; i < nonLetterGraded.Count; i++) {
                nonLetterGraded[i] = nonLetterGraded[i].Substring(0, 4);
            }
                num = required.Count;
            setActual();                            //sets default required course array
        }

        public  List<Course> getBlank() {           //not sure if necessary
            return blank;
        }

        public  List<string> getRequired() {        //returns arraylist of required classes (String)
            return required;
        }

        public  void setActual()                        
        {
            Course[] A = new Course[required.Count];    //initialize Course array with same size as #required courses
            int i = 0;
            foreach (string x in required)              //course name
            {
                A[i] = new Course(x.Substring(0, 4), x, "not compl. or in prog.");  //Initializes Course object with the course number, name, and default grade
                A[i].setStatus("OFF TRACK");                                        //Sets course status as Off Track
                i++;
            }
            actual = A; //after loop, sets global actual with the modified local Course array
        }
        public  int splitCourseList(int j, List<string> list) //goes through text file of Courses to separate the different categories - works as long as they are in the same order as original
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

    public class requiredCourseMethods //new class: methods for checking satisfaction: required, writing, skills
    {
        public static Course[] checkRequiredCourses(Student student, RequiredClasses Object) //This method sets each student's required courses with the correct information
                                                                                             //always use the same requiredClasses object when calling these methods
        {
            Course[] actualcopy = new Course[Object.actual.Length]; //copy of Actual
            
            for (int i = 0; i < actualcopy.Length; i++)             //make sure each Course object in actual is copied into actualcopy
            {
                actualcopy[i] = Object.actual[i];
            }
            List<Semester> semest = student.getStudentSemesters();  //temporary semester List, easier to understand

            for (int i = 0; i < semest.Count; i++)
            {
                List<Course> temp = semest[i].getCourseList(); //temporary Course List, easier to understand
                for (int k = 0; k < actualcopy.Length; k++)
                {
                    for (int j = 0; j < semest[i].getCourseList().Count; j++)
                    {
                        if (actualcopy[k].getCourseNum().Contains(temp[j].getCourseNum())) //if the student's course on the transcript matches one of the required courses in the list
                        {
                            actualcopy[k] = temp[j]; //copy the course + info
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
                                actualcopy[k].setStatus("SATISFIED");           //if these grade conditions are met, then the student has satisfied this course, set info          
                            }
                            if ((temp[j].getGrade().Contains("Progress"))) {    //modify Status and Track accordingly: means this course is in progress
                                actualcopy[k].setStatus("ON TRACK");
                                actualcopy[k].setTrack("IN PROGRESS");
                            }
                            break;
                        }

                    }
                }
            }
            student.setreqcourses(actualcopy); //stores copy of required courses Course array in the Student object with modified Status and Track +etc
                return actualcopy;
        }

        public static void checkWriting(Student student, RequiredClasses Object) //This method checks and sets each student's writing courses (if any) with the correct information
                                                                                 //always use the same requiredClasses object when calling these methods
        {
            List<Semester> semest = student.getStudentSemesters();
            for (int i = 0; i < semest.Count; i++)
            {
                if (semest[i].getInProg() && !student.getWritSat().Contains("SATISFIED"))   //if the stud's writing req hasn't been satisfied, check for writing courses in progress
                                                                                            //if it has, don't need to go through this process, hence the '!'
                {
                    for (int j = 0; j < semest[i].getCourseList().Count; j++)
                    {
                        List<Course> temp = semest[i].getCourseList();
                        for (int k = 0; k < Object.writing.Count; k++)
                        {
                            if (Object.writing[k].Contains(temp[j].getCourseNum()))
                            {
                                temp[j].setStatus("IN PROGRESS");                   //set course status and the student's WritSat
                                student.setWritSat("ON TRACK");
                                student.addWriting(temp[j]);                        //add this course to the student's writing course array

                                if (Object.writing[k].Contains("or more credits")) ///////////////////////////if each name in the writing arraylist(string) contains this requirement,
                                {
                                    int b = Object.writing[k].Length;
                                    string str = Object.writing[k];
                                    int numCreds = -5;
                                    for (int n = 0; n < b; n++)                     //find how many credits are needed,
                                    {
                                        if (str[n].Equals('1') || str[n].Equals('2') || str[n].Equals('3')|| str[n].Equals('3')|| str[n].Equals('4')|| str[n].Equals('5')
                                        || str[n].Equals('6')|| str[n].Equals('7')|| str[n].Equals('8')|| str[n].Equals('9')) { 
                                            numCreds = n;
                                            break;
                                        }
                                    }
                                        if (temp[j].getCreds() < numCreds) {        //determine if student is taking enough credits in that course + set appropriately
                                            student.setWritSat("OFF TRACK");
                                            student.addWriting(temp[j]);
                                            temp[j].setStatus("IN PROGRESS");
                                            continue;
                                        }
                                }                                                   ////////////////////////////////
 
                            }
                        }
                    }
                }
            }
        }

        public static void checkSkills(Student student, RequiredClasses Object) //This method checks and sets each student's Skill courses (if any) with the correct information
                                                                                //always use the same requiredClasses object when calling these methods
        {
            List<Semester> semest = student.getStudentSemesters();
            for (int i = 0; i < semest.Count; i++)
            {
                if (semest[i].getInProg() && !student.getSkillSat().Contains("SATISFIED")) {  //if the stud's writing req hasn't been satisfied, check for Skill courses in progress
                                                                                              //if it has, don't need to go through this process, hence the '!'
                    for (int j = 0; j < semest[i].getCourseList().Count; j++)
                    {
                        List<Course> temp = semest[i].getCourseList();
                        for (int k = 0; k < Object.skills.Count; k++)
                        {
                            if (Object.skills[k].Contains(temp[j].getCourseNum()))  //if each name in the writing arraylist(string) contains this requirement,
                            {
                                 temp[j].setStatus("ON TRACK");                     //set necessary info
                                 student.setSkillSat("ON TRACK");
                                 student.addSkills(temp[j]);                        //add course to student's Skill course array
                            }
                        }
                    }
                }
            }
        }
    }
}