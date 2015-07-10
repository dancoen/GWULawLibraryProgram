using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryProject;
using System.Collections.Generic;

namespace classesForLibraryExcel
{

    public class Student {
        //this Student class holds a GWid as a string, 
        //the name of the student, and an arraylist of 
        //the semesters that the student is taking, based
        //on how we implement the program, we can either add
        //a whole arraylist of semester at a time, or add them one
        //at a time

        /*-----------------------------CONSTRUCTORS BELOW ------------------------------*/

        public Student(string GWid1, string studentName1, ArrayList studentSemester1)
        {
            GWid = GWid1;
            studentName = studentName1;
            studentSemester = (Semester[])studentSemester1.ToArray(typeof(Semester));
        }
        public Student(string studentName1)
        {
            studentName = studentName1;
        }
        public Student() { }


        /*------------------------------------------------------------------------------*/


        private bool transferStud = false;
        public bool getTransferStud() { return transferStud; }
        public void setTransferStud(bool transferStud) { this.transferStud = transferStud; }
        private string studentName = "";
                public string getStudentName() { return studentName; }
                public void setStudentName(string studentName1) { studentName = studentName1; }
        private string GWid = "";
                public string getGWid() { return GWid; }
                public void setGWid(string GWid1) { GWid = GWid1; }
                private string track = "ON TRACK";
                public void setTrack() {
                    track = "*** OFF TRACK ***";
                }
                public string getTrack() {
                    return track;
                }

        private bool allPartTime = false; //true if student is part time all semesters, false otherwise
                public void setPartTime(bool x) { allPartTime = x; }
                public bool getPartTime() { return allPartTime; }

        private bool nonLawCourses = false;
                public bool getnonLawBool() { return nonLawCourses; }
                public void setnonLawBool(bool x) { nonLawCourses = x; }

        private string[] transcript = null;
                public string[] getTranscript() { return transcript; }
                public void setTranscript(string[] t) { transcript = t; }

        private double GPA = 0.0;
                public double getGPA() { return GPA; }
                public void setGPA(double x) { GPA = x; }
        private double gradedCreds = 0.0;
                public double getGradedCreds() { return gradedCreds; }
                public void setGradedCreds(double gradedCreds1) { gradedCreds = gradedCreds1; }
                public void addGradedCreds(double gradedCreds1) { gradedCreds += gradedCreds1; }
        private double credsInProgress = 0;
                public double getCredsInProgress() { return credsInProgress; }
                public void setCredsInProgress(double credsInProgress1) { credsInProgress = credsInProgress1; }
                public void addCredsInProgress(double addCred) { credsInProgress += addCred; }
        private double gradedcredsInProgress = 0.0;
                public double getgradedcredsInProgress() { return gradedcredsInProgress; }
                public void setgradedcredsInProgess(double n) { gradedcredsInProgress = n; }
        private double totCred = 0.0; //total credits
                public double getTotCred() { return totCred; }
                public void setTotCred(double totCred1) { totCred = totCred1; }
        private double enrollUnits = 0.0; //enrollment units
                public void setEnrollUnits(double units) { enrollUnits = units; }
                public double getEnrollUnits() { return enrollUnits; }
        private bool enrollStat = false;
                public void setEnrollStat() { enrollStat = true; }
                public bool getEnrollStat() { return enrollStat; }
        private bool underGrad = false;
                public void setUnderGrad(bool x) { underGrad = x; }
                public bool getUnderGrad() { return underGrad; }
        private bool Grad = false;
                public bool getGrad() { return Grad; }
                public void setGrad(bool x) { Grad = x; } 
        private bool Law = false;
                public bool getLaw() { return Law; }
                public void setLaw(bool x) { Law = x; }
        private bool courseSat = false; //true if student has satisfied all required courses
                public void setCourseSat(bool x) { courseSat = x; }
                public bool getCourseSat() { return courseSat; }
        private Course[] requiredCourses;
                public void setreqcourses(Course[] x) { requiredCourses = x; }
                public Course[] getreqcourses() { return requiredCourses; }
        private string writSat = "OFF TRACK"; //SATISFIED only if legend below their name says so. ON TRACK if class on list is in progress. Otherwise OFF TRACK
                public void setWritSat(string x) { writSat = x; }
                public string getWritSat() { return writSat; }
        private string skillSat = "OFF TRACK"; //SATISFIED only if legend below their name says so. ON TRACK if class on list is in progress. Otherwise OFF TRACK
                public void setSkillSat(string x) { skillSat = x; }
                public string getSkillSat() { return skillSat; }
        private List<Course> writing = new List<Course>(); //list of writing courses taken
                public void addWriting(Course course) { writing.Add(course); }
                public List<Course> getAllWriting() { return writing; }
        private List<Course> skills = new List<Course>(); //list of skills courses taken
                public void addSkills(Course course) { skills.Add(course); }
                public List<Course> getAllSkills() { return skills; }
        private List<Semester> studentSemesters = new List<Semester>();
                public List<Semester> getStudentSemesters() { return studentSemesters; }
                public void setStudentSemesters(List<Semester> studentSemester1) { studentSemesters = studentSemester1; }
                public void addStudentSemester(List<Semester> studentSemester1)
                {
                    int i = 0;
                    foreach (Semester element in studentSemester1)
                    {
                         studentSemesters.Add(studentSemester1[i]);
                         i++;
                    }
                }
        private Semester[] studentSemester;
                public void addOneSemester(Semester studentSemester1)
                {
                    studentSemesters.Add(studentSemester1);
                }
                public void setSingleSemester(Semester singleSemester) { studentSemesters.Add(singleSemester); }

        /*-----------------------METHODS--------------------------*/

        public string getTotCredStat()
        {
            if (totCred >= 84)
            {
                return "ON TRACK";
            }
            return "OFF TRACK";
        }
        public double getTotalEUnits(Semester lawSem)
        {
            double totalEUnits = 0.0;
            foreach (Semester x in studentSemesters)
            {
                totalEUnits = totalEUnits + x.getEUnits(); 
            }
            return totalEUnits;
        }
    }
}

