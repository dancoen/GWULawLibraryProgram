using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject
{
    public class Course
    {
        private double creds = 0.0;
        private String courseNum = "0000";
        private String courseName = "";
        private String status = "OFF TRACK";
        private String grade = "In Progress";
        private String requirement = ""; //SKILL OR WRITING or REQUIRED
        private String onofftrack = "OFF TRACK"; //set as "OFFTRACK" or as "IN PROGRESS" if it's currently being taken
        public bool nonLetterGraded = false;
        public Course() { }
        public Course(String courseNum1, String courseName1, String grade1)
        {
            courseNum = courseNum1;
            courseName = courseName1;
            grade = grade1;
        }
        public double getCreds() { return creds; }
        public void setCreds(double creds1) { creds = creds1; }
        public String getCourseNum() { return courseNum; }
        public void setCourseNum(String courseNum1) { courseNum = courseNum1; }
        public String getCourseName() { return courseName; }
        public void setCourseName(String courseName1) { courseName = courseName1; }
        public String getStatus() { return status; }
        public void setStatus(String status1) { status = status1; }
        public String getGrade() { return grade; }
        public void setGrade(String grade1) { grade = grade1; }
        public void setReq(String req) { requirement = req; }
        public String getReq() { return requirement; }
        public void setTrack(String pf) { onofftrack = pf; }
        public String getTrack() { return onofftrack; }
        public void setnonLetterGraded(bool status) { nonLetterGraded = status; }
        public bool getnonLetterGraded() { return nonLetterGraded; }
    }
}
