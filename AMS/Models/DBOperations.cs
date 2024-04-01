using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class DBOperations
    {
        static AMSEntities a = new AMSEntities();
        //Check the entered values with credentials
        public static bool LoginValidate(string UserID, string Password)
        {

            var check = from l in a.tblLogins
                        where l.UserID == UserID && l.Password == Password
                        select l;
            if (check.ToList().Count == 1)
                return true;
            else
                return false;
        }

        //Pilot Operations 
        public static bool Check(string emailid, string mobileno)
        {
            var m = from i in a.tblPilots
                    where i.EmailAddress == emailid && i.MobileNo == mobileno
                    select i;
            if (m.Count() != 0)
                return false;
            else
                return true;
        }
        public static string InsertPilot(Validation V)
        {
            
            var email = from e in a.tblPilots
                        where e.EmailAddress == V.Pilot.EmailAddress
                        select e;
            var mobile = from e in a.tblPilots
                        where e.MobileNo == V.Pilot.MobileNo
                        select e;
        
            if (email.Count() != 0)
            {
                return "EmailId already exists";
            }
            else if (mobile.Count() != 0)
            {
                return "Mobile Number already exists";
            }
            else
            {
                tblPilot P1 = V.Pilot;
                tblAddress A1 = V.Address;
                try
                {
                    if (A1.AddressId == null)
                        a.tblPilots.Add(P1);
                    else
                    {
                        a.tblPilots.Add(P1);
                        a.tblAddresses.Add(A1);
                    }


                    a.SaveChanges();
                }
                catch (DbUpdateException E)
                {
                    SqlException S = E.GetBaseException() as SqlException;

                    return "Something went wrong, Plz try again.";
                }
                catch (DbEntityValidationException M)
                {
                    //SqlException ex = E.GetBaseException() as SqlException;
                    return M.Message;
                }
                return "Pilot added successfully " + V.Pilot.Pilot_ID;
            }
        }
        public static string checkAddressDetails(string HouseNo, string City)
        {
            var l = from add in a.tblAddresses
                    where add.HouseNo == HouseNo && add.City == City
                    select add;
            if (l.Count() != 0)
                return l.ToList().First().AddressId;
            else
                return null;
            
        }
        //generates AddressId based on entered City 
        public static string getAddressId(string city)
        {
            string s = null;
            ObjectParameter aid = new ObjectParameter("aid", typeof(string));
            ObjectResult<string> ob = a.sp_Address(city);
            List<string> L = ob.ToList();
            if (L.Count != 0)
            {
                int ai = int.Parse(L[0].Substring(3)) + 1;
                s = city.Substring(0, 3) + string.Format("{0:000}", ai);
            }
            else
                s = city.Substring(0, 3) + "101";
            return s;
        }
        //Generates the PilotId based on input social security number
        public static string getPilotId(string SSN)
        {
            string s = null;
            ObjectResult<string> ob = a.sp_Pilot(SSN);
            List<string> L = ob.ToList();
            if (L.Count != 0)
            {
                int pi = int.Parse(L[0].Substring(4)) + 1;
                s = SSN.Substring(SSN.Length - 4, 4) + string.Format("{0:00}", pi);
            }
            else
                s = SSN.Substring(SSN.Length - 4, 4) + "31";
            return s;
        }

        //Plane dboperations

        public static string InsertPlane(Validation v)
        {
            var email = from e in a.tblPlanes
                        where e.Email == v.Plane.Email
                        select e;
           
            // tblPilot P1 = (tblPilot)email;
            if (email.Count()!= 0)
            {
                return "EmailId already exists";
            }
            else
            {
                try
                {
                    if (v.Address.AddressId == null)
                    {
                        a.tblPlanes.Add(v.Plane);
                        a.SaveChanges();
                    }
                    else
                    {
                        a.tblAddresses.Add(v.Address);
                        a.SaveChanges();
                        a.tblPlanes.Add(v.Plane);
                        a.SaveChanges();

                    }
                }
                catch (DbUpdateException E)
                {
                    SqlException S = E.GetBaseException() as SqlException;

                    return "Something went wrong, Plz try again.";
                }
                catch (DbEntityValidationException M)
                {
                    //SqlException ex = E.GetBaseException() as SqlException;
                    return M.Message;
                }
                return "Plane added successfully " + v.Plane.PlaneID;
            }

        }//------------------------- 
public static List<tblPlane> getPlaneIds()
        {
            var planeId = from pid in a.tblPlanes
                          select pid;
            return planeId.ToList();
        }
        public static string getPlaneId(string Registration_No)
        {
            string s = null;
            ObjectResult<string> ob = a.sp_Plane(Registration_No);
            List<string> L = ob.ToList();
            if (L.Count != 0)
            {
                int pi = int.Parse(L[0].Substring(4)) + 1;
                s = Registration_No.Substring(Registration_No.Length - 4, 4) + string.Format("{0:00}", pi);
            }
            else
                s = Registration_No.Substring(Registration_No.Length - 4, 4) + "31";
            return s;
        }
        //method gets all the available pilots to assign to a plane
        public static ObjectResult<sp_getPilotId_Result> getPilots()
        {
            ObjectResult<sp_getPilotId_Result> l = a.sp_getPilotId();

            return l;
        }
        //AddHanger DBOperations
        //checks the manager emaild and mobileno already exsists are not
        public static bool CheckManager(string emailid, string mobileno)
        {
            var m = from i in a.tblManagers
                    where i.EmailAddress == emailid || i.MobileNo == mobileno
                    select i;

            if (m.Count() != 0)
                return false;
            else
                return true;

        }
        //generates addressid based on entered city and houseno,managerid based on socialscurity umber,hangerid based on location
        public static void getIds(BindModel bind)
        {
            var ad = from i in a.tblAddresses
                     where i.HouseNo == bind.Address.HouseNo && i.City == bind.Address.City
                     select i;
            if (ad.Count() == 1)
            {
                bind.Manager.AddressId = ad.First().AddressId;
            }
            else
            {
                ObjectResult<string> ob = a.sp_Address(bind.Address.City);
                List<string> L2 = ob.ToList();
                if (L2.Count != 0)
                {
                    int ai = int.Parse(L2[0].Substring(3)) + 1;
                    bind.Address.AddressId = bind.Address.City.Substring(0, 3) + string.Format("{0:000}", ai);
                    bind.Manager.AddressId = bind.Address.AddressId;
                }
                else
                {
                    bind.Address.AddressId = bind.Address.City.Substring(0, 3) + "101";
                    bind.Manager.AddressId = bind.Address.AddressId;

                }
            }

            ObjectResult<string> ob1 = a.sp_Hanger(bind.Hanger.HangerLocation);
            List<string> L = ob1.ToList();
            if (L.Count != 0)
            {
                int ai = int.Parse(L[0].Substring(3)) + 1;
                bind.Hanger.Hanger_ID = bind.Hanger.HangerLocation.Substring(0, 3) + string.Format("{0:000}", ai);
            }
            else
                bind.Hanger.Hanger_ID = bind.Hanger.HangerLocation.Substring(0, 3) + "101";
            ObjectResult<string> ob2 = a.sp_Manager(bind.Manager.SocialSecurityNo);
            List<string> L1 = ob2.ToList();
            if (L1.Count != 0)
            {
                int pi = int.Parse(L1[0].Substring(4)) + 1;
                bind.Manager.ManagerId = bind.Manager.SocialSecurityNo.Substring(bind.Manager.SocialSecurityNo.Length - 4, 4) + string.Format("{0:00}", pi);
                bind.Hanger.ManagerId = bind.Manager.ManagerId;
            }
            else
            {
                bind.Manager.ManagerId = bind.Manager.SocialSecurityNo.Substring(bind.Manager.SocialSecurityNo.Length - 4, 4) + "31";
                bind.Hanger.ManagerId = bind.Manager.ManagerId;
            }
        }
        //inserts hanger details into database ,if the addressid is already present in 
        //address table it adds the details only in hanger table,manager tables 
        //else it adds in address table also
        public static string insertHanger(BindModel bind)
        {
              try
            {
                if (bind.Address.AddressId == null)
                {
                    a.tblManagers.Add(bind.Manager);
                    a.SaveChanges();
                    a.tblHangers.Add(bind.Hanger);
                    a.SaveChanges();

                }
                else
                {
                    a.tblAddresses.Add(bind.Address);
                    a.SaveChanges();
                    a.tblManagers.Add(bind.Manager);
                    a.SaveChanges();
                    a.tblHangers.Add(bind.Hanger);
                    a.SaveChanges();
                }
                 }
                 catch (DbUpdateException E)
                 {
                     SqlException S = E.GetBaseException() as SqlException;

                     return "Something went wrong, Plz try again.";
                 }
                 catch (DbEntityValidationException M)
                 {
                     //SqlException ex = E.GetBaseException() as SqlException;
                     return M.Message;
                 }
                return "Hanger Added Successfully " + bind.Hanger.Hanger_ID;
            
        }

        //Plane Allocation Operations
        public static List<sp_AvailableHangersDetails_Result> getAvailabeHangers(DateTime sDate, DateTime eDate)
        {
            var Avail_Hangers = a.sp_AvailableHangersDetails(sDate, eDate);
            return Avail_Hangers.ToList();
        }
        //get all the hangers available at different locations
        public static tblHanger getHanger(string hid)
        {
            var Avail_Hangers = from ah in a.tblHangers
                                where ah.Hanger_ID == hid
                                select ah;
            return Avail_Hangers.FirstOrDefault();
        }
        //allocate plane to purticular hanger for some duration
        public static string AllocatePlaneToHanger(tblPlaneAllocation PlaneAlloc)
        {
            try
            {
                a.tblPlaneAllocations.Add(PlaneAlloc);
                a.SaveChanges();
                return "Hanger allocated for the plane " + PlaneAlloc.PlaneID + " from date " + PlaneAlloc.StartDate.ToString("dd-MM-yyyy") + " to date" + PlaneAlloc.EndDate.ToString("dd-MM-yyyy");
            }
            catch (DbUpdateException e)
            {
                SqlException ex = e.GetBaseException() as SqlException;
                if (ex.Message.Contains("PK_tblPlaneAllocation"))
                {
                    return "This Plane has already booked for hanger in this duration";
                }
                else
                    return "Something went wrong, Plz try again.";
            }

        }
        //gets the hanger available numbers 
        public static List<tblHanger> gethangerNos()
        {
            var hno = from i in a.tblHangers
                      select i;
            return hno.ToList();
        }
        //gets the status of the hanger
        public static List<sp_getStatus_Result> gethangerstatus(string hno, DateTime stdt, DateTime endt)
        {
            var h = a.sp_getStatus(hno, stdt, endt);
            return h.ToList();
        }
    }

}
    
