using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSplit;
using System.Diagnostics;
using System.Collections;
using LibraryProject;
using System.IO;
using classesForLibraryExcel;

namespace LibraryProject {

    public class createTextDoc
    {
        public static void createText(string path, List<Student> studentList, RequiredClasses Object)
        {
            string doc = path + "/" + "studReports.txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(doc, false))
                {
                    foreach (Student x in studentList)
                    {
                        List<String> tempReqCourses = Object.getRequired();
                        file.WriteLine("GWID " + x.getGWid() + "                " + x.getTrack());   // + stud on or off track
                        file.WriteLine();
                        file.WriteLine(x.getStudentName() + '\n');
                        file.WriteLine();
                        file.WriteLine("REQUIRED COURSES" + '\n');
                        file.WriteLine();
                        Course[] c = new Course[x.getreqcourses().Length]; //creates new array of length [# req courses]
                        Course[] temp = x.getreqcourses();                 //temp array to store student's required courses, whether satisfied or not - contains all course info
                        for (int i = 0; i < c.Length; i++)                 //copies data into empty Course array c
                        {
                            c[i] = temp[i];
                        }
                        for (int i = 0; i < c.Length; i++)                 //below: start writing to text file, loops through each required course
                        {

                            if (c[i].getStatus().Equals(("OFF TRACK")))                            //if that course is not completed
                            {
                                if (c[i].getGrade().Equals("not compl. or in prog."))              //if grade is: set grade (in new array) to "not compl. or in prog."
                                {
                                    string n = "   " + tempReqCourses[i];                          //creates string to concat info
                                    file.Write(n);                                                         //writes string to text file     
                                    for (int o = n.Length; o < 30; o++)                                    //spacing
                                    {
                                        file.Write(" ");
                                    }
                                    file.WriteLine(c[i].getStatus() + "  <= not compl. or in prog.");      //writes status (ON or OFF TRACK) and etc
                                }
                                else                                                  //else, if grade exists/is not previous string, write + its grade
                                {
                                    string n = "   " +  tempReqCourses[i];                                 //creates string to concat info
                                    file.Write(n);                                                         //writes string to text file
                                    for (int o = n.Length; o < 30; o++)                                    //spacing
                                    {
                                        file.Write(" ");
                                    }
                                    file.WriteLine(c[i].getStatus() + "  <= grade is " + c[i].getGrade());
                                }
                            }
                            else if (c[i].getTrack().Equals("IN PROGRESS"))                 //if that course is in progress, do the same thing but..
                            {
                                string n = "   " + tempReqCourses[i];
                                file.Write(n);
                                for (int o = n.Length; o < 30; o++)
                                {
                                    file.Write(" ");
                                }
                                file.WriteLine(c[i].getStatus() + "   <= in progress");    //...write this instead, to note 'in progress'
                            }
                            else                                                           //any other case
                            {
                                string n = "   " + tempReqCourses[i];
                                file.Write(n);
                                for (int o = n.Length; o < 30; o++)
                                {
                                    file.Write(" ");
                                }
                                file.WriteLine(c[i].getStatus() + "  <= grade is " + c[i].getGrade());
                            }
                        }
                        file.WriteLine("");
                        string totcredtrack;                                            //total credits
                        if (x.getTotCred() >= 84) { totcredtrack = "SATISFIED"; }       //check total credits, set student's total credits SATISFIED if >= 84
                        else if (x.getTotCred() + x.getCredsInProgress() >= 84) { 
                            totcredtrack = "ON TRACK"; }                                //if total credits + in-progress credits are <= 84, set ON TRACK (not satisfied)
                        else { totcredtrack = "OFF TRACK"; }                            //else, off track
                        string gradcredtrack;                                           //graded credits
                        if (!x.getTransferStud()) { 
                            if (x.getGradedCreds() >= 67) { gradcredtrack = "SATISFIED"; }
                            else if (x.getGradedCreds() + x.getCredsInProgress() >= 67) { gradcredtrack = "ON TRACK"; }
                            else { gradcredtrack = "OFF TRACK"; }
                        }
                        else
                        {
                            if (x.getGradedCreds() >= 48) { gradcredtrack = "SATISFIED"; }
                            else if (x.getGradedCreds() + x.getCredsInProgress() >= 48) { gradcredtrack = "ON TRACK"; }
                            else { gradcredtrack = "OFF TRACK"; }
                        }
                        file.WriteLine("");
                        file.WriteLine('\n' + "TOTAL CREDITS (84)" + '\t' + "{0} <= {1} compl. + {2} pend. = {3} total", 
                            totcredtrack, x.getTotCred(), x.getCredsInProgress(), x.getTotCred() + x.getCredsInProgress());
                        file.WriteLine("");
                        file.WriteLine('\n' + "GRADED CREDITS (67)" + '\t' + "{0} <= {1} graded + {2} pend. = {3} total",
                            gradcredtrack, x.getGradedCreds(), x.getgradedcredsInProgress(), x.getGradedCreds() + x.getCredsInProgress());
                        file.WriteLine('\n' + "(48 for transfer students)");
                        file.WriteLine("");

                        if (x.getSkillSat().Contains("SATISFIED")) //prev "REQUIREMENT MET"
                        {
                            file.WriteLine('\n' + "SKILLS REQUIREMENT" + '\t' + "SATISFIED  <= SKILLS REQUIREMENT MET legend");
                        }
                        else if (x.getSkillSat().Contains("OFF TRACK"))
                        {
                            file.WriteLine('\n' + "SKILLS REQUIREMENT" + '\t' + x.getSkillSat() + " <= no legend / no course in prog.");
                        }
                        else
                        {
                            List<Course> skills = x.getAllSkills();
                            foreach (Course skill in skills)
                            {
                                if (skill.getGrade().Equals("In Progress"))
                                {
                                    file.WriteLine('\n' + "SKILLS REQUIREMENT" + '\t' + "ON TRACK" + " <= " + "course " + skill.getCourseNum() + " in prog.");
                                    break;
                                }
                            }
                        }
                        file.WriteLine("");

                        // writing

                        if (x.getWritSat().Contains("SATISFIED"))
                        { //prev "REQUIREMENT MET"
                            file.WriteLine('\n' + "WRITING REQUIREMENT" + '\t' + "SATISFIED  <= WRITING REQUIREMENT MET legend");
                        }
                        else if (x.getWritSat().Contains("OFF TRACK"))
                        {
                            file.WriteLine('\n' + "WRITING REQUIREMENT" + '\t' + x.getWritSat() + " <= no legend / no course in prog.");
                        }
                        else
                        {
                            List<Course> skills = x.getAllWriting();
                            foreach (Course skill in skills)
                            {
                                if (skill.getGrade().Equals("In Progress"))
                                {
                                    file.WriteLine('\n' + "WRITING REQUIREMENT" + '\t' + "ON TRACK" + " <= " + "course " + skill.getCourseNum() + " in prog.");
                                    break;
                                }
                            }
                        }

                        file.WriteLine("");
                        string gpatrack = "OFF TRACK";
                        if (x.getGPA() >= 1.67) { gpatrack = "ON TRACK"; }
                        file.WriteLine('\n' + "MINIMUM GPA (1.67)" + '\t' + gpatrack + " <= " + x.getGPA());
                        file.WriteLine("");
                        string enrolltrack = "OFF TRACK";
                        if (x.getPartTime() == true)
                        {
                            if (x.getEnrollUnits() >= 5.9) { enrolltrack = "SATISFIED"; }
                        }
                        else if (x.getEnrollUnits() >= 6.0) { enrolltrack = "SATISFIED"; }
                        file.WriteLine('\n' + "ENROLLMENT UNITS" + '\t' + enrolltrack + " <= " + x.getEnrollUnits() + '\n');   //needs to check if eu is below minimum
                        file.WriteLine();


                        int ctr = 0; //checks to see whether 0 == semester count - 1
                        List<Semester> semList = x.getStudentSemesters();
                        for (int i = 0; i < semList.Count(); i++)
                        {
                            String EUnits = "" + semList[i].getEUnits();
                            if (EUnits.Length == 1) //formats the enrollment units so that they will all be length 5
                            {
                                EUnits += ".000";
                            }
                            else if (EUnits.Length == 3)
                            {
                                EUnits += "00";
                            }
                            else if (EUnits.Length == 4)
                            {
                                EUnits += "0";
                            }
                            if (semList[i].getEUnits() == 0.0) { continue; }
                            if (ctr == x.getStudentSemesters().Count() - 1)
                            {
                                file.WriteLine('\t' + semList[i].getSemesterName() + '\t' + EUnits + " " + "(" + semList[i].getCreditHours() + " in progress credits)");
                            }
                            string n = semList[i].getSemesterName();
                            if (semList[i].getSemesterName().Contains("NON-GW"))
                            {
                                n = n.Replace("NON-GW ", "");
                            }
                            file.Write(n);
                            for (int o = n.Length; o < 24; o++)
                            {
                                file.Write(" ");
                            }
                            if (semList[i].getSemesterName().Contains("NON-GW"))
                            {
                                file.WriteLine(EUnits + " (" + semList[i].getCreditHours() + " transfer credits)");
                            }
                            else if (!semList[i].getSemesterName().Contains("Fall") 
                                && !semList[i].getSemesterName().Contains("Spring") 
                                && !semList[i].getSemesterName().Contains("Summer"))
                            {
                                file.WriteLine(EUnits + " (" + semList[i].getCreditHours() + " transfer credits)");
                            }
                            else if(!semList[i].getInProg() && !semList[i].getHalfInProg())
                            {
                                file.WriteLine(EUnits + " (" + semList[i].getCreditHours() + " compl. credits)");
                            }
                            else if (semList[i].getInProg() && !semList[i].getHalfInProg())
                            {

                                file.WriteLine(EUnits + " (" + semList[i].getCreditHours() + " in prog. credits)");
                            }
                            else
                            {
                                file.WriteLine(EUnits + " (" + semList[i].getHalfComp() + " compl. + " + semList[i].getHalfProg() + " in prog. credits)");
                            }

                        }
                        file.WriteLine("");
                        file.WriteLine("");
                        file.WriteLine();
                        if (x.getnonLawBool() == true)
                        {
                            file.WriteLine("***Non-Law Courses on Transcript***");
                        }
                        file.WriteLine('\f');
                    }
                }
                Process.Start(doc);
            }
        }
}
