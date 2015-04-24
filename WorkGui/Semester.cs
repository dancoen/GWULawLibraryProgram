using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using LibraryProject;
using System.Collections.Generic;
//using System.Windows.Forms;

namespace classesForLibraryExcel
{
  public class Semester
  {
        //This is the semester class that we can use to 
        //easily store an array of semesters into a student class
        //has basic getters and setters for Enrollment Units,
        //credit hours, and the name of the semester
        private bool halfInProg = false;
        public void setHalfInProg(bool x) { halfInProg = x; }
        public bool getHalfInProg() { return halfInProg; }
        private int halfProg = 0;
        private int halfComp = 0;
        public void setProgComp(int prog, int comp) { halfProg = prog; halfComp = comp; }
        public int getHalfProg() { return halfProg; }
        public int getHalfComp() { return halfComp; }
        private bool inProg = false;
        private bool time; //the boolean that determines whether or not the student was part or full time
        private double EUnits = 0.0;
        private int creditHours = 0;
        private string semesterName = "";
        private List<Course> studentCourseList = new List<Course>();
        public Semester() { }
        public Semester(string semesterName1, int creditHours1, List<Course> courseList1) {
            semesterName = semesterName1; //INCLUDES NON-GW HISTORY
            creditHours = creditHours1;
            calcEUnits(creditHours1);
            studentCourseList = courseList1;
        }
        public bool tookNonLaw = true;
        public void setTookNonLawFalse() {
            tookNonLaw = false;
        }
        public void addEUnits(double EU)
        {
            EUnits += EU;
        }
        public bool getInProg() { return inProg; }
        public void setInProg(bool inProg1) { inProg = inProg1; }
        public double getEUnits() { return EUnits; }
        public bool getParttime() { return time; } //returns true if the semester is parttime and false if the semester is not
        public void setEUnits(double EUnits1) { EUnits = EUnits1; }
        public int getCreditHours() { return creditHours; }
        public void setCreditHours(int creditHours1) {
            calcEUnits(creditHours1);
            if (creditHours1 >= 12)
            {
                time = false;
            }
            else
            {
                time = true;
            }
            creditHours = creditHours1; 
        }
        public string getSemesterName() { return semesterName; }
        public void setSemesterName(string semesterName1) { semesterName = semesterName1; }
        public List<Course> getCourseList() { return studentCourseList; }
        public void addCourse(Course course){ studentCourseList.Add(course); }
        public bool progress;
        public double calcEUnits(int credits)
        {
            if (credits >= 12) { EUnits = 1.0; }
            if (credits >= 24) { EUnits = 2.0; }

    
            else
            {
                  switch (credits)
                  {
                        case 1:
                            EUnits = 0.075;
                            break;
                        case 2:
                            EUnits = 0.15;
                            break;
                        case 3:
                            EUnits = 0.2;
                            break;
                        case 4:
                            EUnits = 0.3;
                            break;
                        case 5:
                            EUnits = 0.35;
                            break;
                        case 6:
                            EUnits = 0.4;
                            break;
                        case 7:
                            EUnits = 0.5;
                            break;
                        case 8:
                            EUnits = 0.6;
                            break;
                        case 9:
                            EUnits = 0.65;
                            break;
                        case 10:
                            EUnits = 0.7;
                            break;
                        case 11:
                            EUnits = 0.8;
                            break;
                  }
            }
            return EUnits;
        }
    }
}   
