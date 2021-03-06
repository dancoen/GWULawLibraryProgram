﻿using System;
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
            parseCreditConfigData();
            //this.setCreditConfigData(parseCreditConfigData());
            //this.setCreditConfigData(parseCreditConfigData());
        }
        public  string coursepath;
        public  string studentpath; 
        public  string[] courseList;  
        private List<string> required = new List<string>(); //arraylist of required course names
        public  List<string> writing = new List<string>();   //arraylist of required writing course names
        public  List<string> skills = new List<string>();    //arraylist of required skills course names
        public  List<double> creditConfigData = new List<double>();
        public  List<string> nonLetterGraded = new List<string>();
        public  List<string> getnonLetterGraded() { return nonLetterGraded; }
        public  List<Course> blank = new List<Course>();     //likely not needed
        public  Course[] actual;                             //arraylist of required Course objects that have been set with default values,--
                                                             //---so same order every time, only need to change appropriate data within for each student. This will happen later on.
        public  string[] lines;                              //arraylist for student list to parse through
        public  int num;                                     //number of required courses

        public List<double> getCreditConfigData()
        {
            return creditConfigData;
        }

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
        //      /*end of credit stats*/



        public  void splitRequiredClasses()         //splits the text file into required, writing, and 
        {
            int lineNum = 0;
            lineNum = parseCreditConfigData();
            int startLine = 1;
            for (int i = 0; i < courseList.Length; i++)
            {
                if (courseList[i].Contains("TOTAL CREDITS"))
                {
                    startLine = i;
                    break;
                }
            }
            //Console.WriteLine(lineNum);
            int x = splitCourseList(lineNum, required);   //parses course list into the proper array, returns the index where required classes stop
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
        public void setCreditConfigData(List<double> datalist) {
            this.creditConfigData = datalist;
        }

        public void getCreditsFromConfigData(int i)
        { 
            int firstIndex = courseList[i].IndexOf("(");//gets the first index of where the number we want occurs
            int secondIndex = courseList[i].IndexOf(")");//gets second index
            string creditValue = courseList[i].Substring(firstIndex + 1, secondIndex - firstIndex - 1);
           // string creditValue = courseList[i].Substring(firstIndex+1, secondIndex - firstIndex -1);

            if (creditValue.Equals("#.#"))
            {
                creditConfigData.Add(-1);//this indicates the writing field, if its a number other than -1 we know to use a different set of code
            }                            // is essentially in place in case ABA changes writing requirement like they have for skills
            else
            {
                //Console.WriteLine("number is " + Double.Parse(creditValue));
                creditConfigData.Add(Double.Parse(creditValue));//parses the string to the double value and adds it to the global list
            }
        }

        public int parseCreditConfigData()
        {
            int lineNum = 0;
            for (int i = 0; i < courseList.Count(); i++)
            {
                while ((courseList[i].Count() != 0))//equal to zero would indicate line in the course file is empty
                {
                    if (courseList[i].Contains("Total"))
                    {
                        for (int j = i; j < i + 8; j++)// we know there are 8 credit requirements, so once we find the first one
                        {                              //we can just parse through the next 8 lines
                                getCreditsFromConfigData(j);
                        }
                    }else if (courseList[i].Contains("---"))//indicates the first dashed line was found
                    {
                        for (int j = i+1; j < courseList.Count(); j++)//iterates through the comment section 
                        {
                            if (courseList[j].Contains("---"))
                            {
                                for (; j < courseList.Count(); j++)
                                {
                                    if (courseList[j].Contains("REQUIRED"))
                                    {
                                        lineNum = j;
                                        return lineNum + 1;//seems to work, a bit weird tho

                                    }
                                }
                            }
                        }
                    }
                    i++;
                }
            }
            return lineNum;
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

        public static void checkWriting(Student student, RequiredClasses reqClasses) //This method checks and sets each student's writing courses (if any) with the correct information
                                                                                 //always use the same requiredClasses object when calling these methods
        {
            //reqClasses.creditConfigData[5] is writing requirement number, initialized to -1 when #.# is in course config file
            if (reqClasses.creditConfigData[5] >= 0){
                double completedWriting = 0.0;
                double inprogWriting = 0.0;

                List<Semester> semest = student.getStudentSemesters();
                for (int i = 0; i < semest.Count; i++)
                {
                    //if (semest[i].getInProg() && !student.getSkillSat().Contains("SATISFIED")) {  //if the stud's writing req hasn't been satisfied, check for Skill courses in progress
                    //if it has, don't need to go through this process, hence the '!'
                    for (int j = 0; j < semest[i].getCourseList().Count; j++)
                    {
                        List<Course> temp = semest[i].getCourseList();
                        for (int k = 0; k < reqClasses.writing.Count; k++)
                        {
                            if (reqClasses.writing[k].Contains(temp[j].getCourseNum()))  //if each name in the writing arraylist(string) contains this requirement,
                            {
                                if (semest[i].getInProg())
                                {
                                    inprogWriting += temp[j].getCreds();
                                }
                                else { completedWriting += temp[j].getCreds(); }
                                temp[j].setStatus("ON TRACK");
                                student.addWriting(temp[j]);
                            }

                        }
                    }

                }
                //Object.setCreditConfigData(Object.parseCreditConfigData());
                Console.WriteLine(student.getGWid() + " : " + completedWriting + " " + reqClasses.creditConfigData[5]);
                if (completedWriting >= reqClasses.creditConfigData[5]) { student.setWritSat("SATISFIED"); }   //change to implicit credit count
                else if (completedWriting + inprogWriting >= reqClasses.creditConfigData[5]) { student.setWritSat("ON TRACK"); }
                else { student.setWritSat("OFF TRACK"); }
            
        }else {
                List<Semester> semest = student.getStudentSemesters();
                for (int i = 0; i < semest.Count; i++)
                {
                    if (semest[i].getInProg() && !student.getWritSat().Contains("SATISFIED"))   //if the stud's writing req hasn't been satisfied, check for writing courses in progress
                                                                                                //if it has, don't need to go through this process, hence the '!'
                    {
                        for (int j = 0; j < semest[i].getCourseList().Count; j++)
                        {
                            List<Course> temp = semest[i].getCourseList();
                            for (int k = 0; k < reqClasses.writing.Count; k++)
                            {
                                if (reqClasses.writing[k].Contains(temp[j].getCourseNum()))
                                {
                                    temp[j].setStatus("IN PROGRESS");                   //set course status and the student's WritSat
                                    student.setWritSat("ON TRACK");
                                    student.addWriting(temp[j]);                        //add this course to the student's writing course array

                                    if (reqClasses.writing[k].Contains("or more credits")) ///////////////////////////if each name in the writing arraylist(string) contains this requirement,
                                    {
                                        int b = reqClasses.writing[k].Length;
                                        string str = reqClasses.writing[k];
                                        int numCreds = -5;
                                        for (int n = 0; n < b; n++)                     //find how many credits are needed,
                                        {
                                            if (str[n].Equals('1') || str[n].Equals('2') || str[n].Equals('3') || str[n].Equals('3') || str[n].Equals('4') || str[n].Equals('5')
                                            || str[n].Equals('6') || str[n].Equals('7') || str[n].Equals('8') || str[n].Equals('9'))
                                            {
                                                numCreds = n;
                                                break;
                                            }
                                        }
                                        if (temp[j].getCreds() < numCreds)
                                        {        //determine if student is taking enough credits in that course + set appropriately
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
        }

        public static void checkSkills(Student student, RequiredClasses reqClasses) //This method checks and sets each student's Skill courses (if any) with the correct information
                                                                                //always use the same requiredClasses object when calling these methods
        {
            /*List<Semester> semest = student.getStudentSemesters();
            for (int i = 0; i < semest.Count; i++)
            {
                //if (semest[i].getInProg() && !student.getSkillSat().Contains("SATISFIED")) {  //if the stud's writing req hasn't been satisfied, check for Skill courses in progress
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
                //}
            }
             * */
            double completedSkills = 0.0;
            double inprogSkills = 0.0;

            List<Semester> semest = student.getStudentSemesters();
            for (int i = 0; i < semest.Count; i++)
            {
                //if (semest[i].getInProg() && !student.getSkillSat().Contains("SATISFIED")) {  //if the stud's writing req hasn't been satisfied, check for Skill courses in progress
                //if it has, don't need to go through this process, hence the '!'
                for (int j = 0; j < semest[i].getCourseList().Count; j++)
                {
                    List<Course> temp = semest[i].getCourseList();
                    for (int k = 0; k < reqClasses.skills.Count; k++)
                    {
                        if (reqClasses.skills[k].Contains(temp[j].getCourseNum()))  //if each name in the writing arraylist(string) contains this requirement,
                        {
                            if (semest[i].getInProg())
                            {
                                inprogSkills += temp[j].getCreds();
                            }
                            else { completedSkills += temp[j].getCreds(); }
                            temp[j].setStatus("ON TRACK"); 
                            student.addSkills(temp[j]);
                        }

                    }
                }

            }
            //Object.setCreditConfigData(Object.parseCreditConfigData());
            if (completedSkills >= reqClasses.creditConfigData[4]) { student.setSkillSat("SATISFIED"); }   //change to implicit credit count
            else if (completedSkills + inprogSkills >= reqClasses.creditConfigData[4]) { student.setSkillSat("ON TRACK"); }
            else { student.setSkillSat("OFF TRACK"); }
        }
    }
}