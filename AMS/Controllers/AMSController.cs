using AMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AMS.Controllers
{
    public class AMSController : Controller
    {
        // GET: AMS
        static List<tblPlane> l1 = null;
        static List<tblHanger> hno = null;
        //static ObjectResult<sp_getPilotId_Result> pilotId = null;
       // static tblHanger h1 = null;
       //Action Method of Home Page
        public ActionResult Index()
        {
            return View();
        }
        //Action Method for login Page
        public ActionResult Login()
        {
            return View();
        }
        //This methods checks all the validations, if the entered values matches the credentials then redirects to adminhome page other wise remains in the same page.
        [HttpPost]
        public ActionResult Login(tblLogin l)
        {
            if (ModelState.IsValid)
            {
                string UserID = Request.Form["UserID"];
                string Password = Request.Form["Password"];
                if (DBOperations.LoginValidate(UserID, Password))
                {
                    Session["User"] = UserID;
                    return RedirectToAction("AdminHome");
                }
                else
                {
                    ViewBag.msg = "Invalid Credentials";
                    return View("Login");
                }
            }
            else
                return View("Login");
            
        }
        //AdminHome Page action method
        public ActionResult AdminHome()
        {

            return View();
        }
        //Pilot Action Methods
        //Action method to create view

        public ActionResult AddPilot()
        {
            return View();
        }
        //checks all the validations and insert pilot details into table 
        [HttpPost]
        public ActionResult AddPilot(Validation V)
        {
            if (ModelState.IsValid)
            {
                if (DBOperations.Check(V.Pilot.EmailAddress, V.Pilot.MobileNo))
                {
                    //generates  pilot_id based on entered socialsecurity number
                    V.Pilot.Pilot_ID = DBOperations.getPilotId(V.Pilot.SocialSecurityNo);
                    //check whether same address details already availble in database
                    string s = DBOperations.checkAddressDetails(V.Address.HouseNo, V.Address.City);
                    if (s != null)
                        V.Pilot.AddressId = s;
                    else
                    {
                        V.Pilot.AddressId = DBOperations.getAddressId(V.Address.City);
                        V.Address.AddressId = V.Pilot.AddressId;
                    }
                    //Insert the pilot details
                    ViewBag.msg = DBOperations.InsertPilot(V);
                    return View("AddPilot");
                }
                else
                {
                    ViewBag.msg = "Email and Mobileno details already Exists";
                    return View("AddPilot");
                }
            }
            else
                return View("AddPilot");
        }
        //Addplane Action Methods
        public ActionResult AddPlane()
        {
            
            ViewBag.L = DBOperations.getPilots();
            Validation v = new Validation();

            return View(v);
        }
        [HttpPost]
        //checks all the validations and insert plane details into the table 
        public ActionResult AddPlane(Validation v)
        {
            if (ModelState.IsValid)
            {
                //gets the generated planeid based on entered registration-number
                v.Plane.PlaneID = DBOperations.getPlaneId(v.Plane.Registration_No);
                if (v.Plane.pilot_ID == null)
                {
                    ViewBag.s = "Please Select PilotId";
                    return View("AddPlane");
                }
                else
                {
                    //checks whether the address already exists and gets the addressid
                    string s = DBOperations.checkAddressDetails(v.Address.HouseNo, v.Address.City);
                    if (s != null)
                        v.Plane.AddressId = s;
                    else
                    {
                        //gets the generated addressid 
                        v.Address.AddressId = DBOperations.getAddressId(v.Address.City);
                        v.Plane.AddressId = v.Address.AddressId;
                    }
                }
                ViewBag.msg = DBOperations.InsertPlane(v);
                // ViewBag.L = DBOperations.getPilots();
                //ViewBag.dd = v.Plane.pilot_ID;
            }  //end of modelstate.is valid
            ViewBag.L = DBOperations.getPilots();
            return View("AddPlane");
        }

        
        //AddHanger Action Methods
        public ActionResult AddHanger()
        {
            BindModel b = new BindModel();
            return View(b);
        }
        [HttpPost]
        //checks all the validations and insert hanger details into the table 
        public ActionResult AddHanger(BindModel bind)
        {
            if (ModelState.IsValid)
            {
                if (DBOperations.CheckManager(bind.Manager.EmailAddress, bind.Manager.MobileNo))
                {
                    DBOperations.getIds(bind);

                    ViewBag.str = DBOperations.insertHanger(bind);
                    return View("AddHanger");
                }
                else
                {
                    ViewBag.str = "Email and Mobileno details already Exists";
                    return View("AddHanger");
                }
            }//end of model state
            else
            {
                return View("AddHanger");
            }
        }
       
       
        //AllotingPlane Controller
        public ActionResult AllotingPlane()
        {
            return View();
        }
        // checks all the validations and get the available hangers between the entered dates
        public ActionResult AvailableHangers(tblPlaneAllocation pa)
        {
            if (ModelState.IsValid)
            {
                Session["sdate"] = DateTime.Parse(Request.Form["StartDate"]);
                Session["edate"] = DateTime.Parse(Request.Form["EndDate"]);
                if (DateTime.Parse(Request.Form["StartDate"]) < DateTime.Parse(Request.Form["EndDate"]))
                { 
                    List<sp_AvailableHangersDetails_Result> l = DBOperations.getAvailabeHangers(DateTime.Parse(Request.Form["StartDate"]), DateTime.Parse(Request.Form["EndDate"]));
                    Session["avail"] = l;
                    return View(l);
                }
                else
                {
                    ViewBag.se = "Please Select proper dates to book";
                    return View("AllotingPlane");
                }
            }
            else
            {
                return View("AllotingPlane");
            }

        }
        // to book the required hanger for the coresponding duration selected 
        public ActionResult BookHanger()
        {
            Session["hid"] = Request.Form["Hanger_Id"];
            if (Session["hid"] == null)
            {
                ViewBag.rs = "Please select a hanger to book";
                return View("AvailableHangers", Session["avail"]);
            }
            else
            {
                tblHanger h = DBOperations.getHanger(Session["hid"].ToString());
                l1 = DBOperations.getPlaneIds();
                ViewBag.L = l1;
                return View(h);
            }
        }
        //[HttpPost]
        public ActionResult BookHanger1()
        {
            
            ViewBag.L = l1;
            string pid = Request.Form["PlaneID"];
            if (pid == "")
            {
                tblHanger h1 = DBOperations.getHanger(Session["hid"].ToString());
                ViewBag.s = "Please select PlaneId to alloacte Hanger";
                return View("BookHanger", h1);
            }
            else
            {
                tblPlaneAllocation PlaneAlloc = new tblPlaneAllocation();
                PlaneAlloc.StartDate = DateTime.Parse(Session["sdate"].ToString());
                PlaneAlloc.EndDate = DateTime.Parse(Session["edate"].ToString());
                PlaneAlloc.HangerID = Session["hid"].ToString();
                PlaneAlloc.PlaneID = pid;

                ViewBag.s = "";
                ViewBag.msg = DBOperations.AllocatePlaneToHanger(PlaneAlloc);
                tblHanger h1 = DBOperations.getHanger(Session["hid"].ToString());
                return View("BookHanger",h1);
            }

        }
        //Action Methods for getHangerDetails
        public ActionResult GetHangerDetails()
        {
            hno = DBOperations.gethangerNos();
            ViewBag.hno = hno;
            return View();
        }
        //checks all the validations and gets the status of the hangers at different location in the corresponding duration
        public ActionResult GetHangerStatus(Dates d)
        {
            if (ModelState.IsValid)
            {
                string hn = Request.Form["ddlhno"];
                DateTime stdt = d.Startdate;
                DateTime endt = d.Enddate;
                if (hn == "" || endt < stdt)
                {
                    ViewBag.hno = hno;
                    if (hn == "")
                        ViewBag.msg = "Select Hanger Number";
                    if (endt < stdt)
                        ViewBag.m = "EndDate should be greater than StartDate";
                    return View("GetHangerDetails");
                }

                List<sp_getStatus_Result> l = DBOperations.gethangerstatus(hn, stdt, endt);
                return View(l);
            }
            else
            {
                ViewBag.hno = hno;
                return View("GetHangerDetails");
            }

        }
        //Action Methosd to logout 
        // clears the session variable value and redirect to login page
        public ActionResult LogOut()
        {
           // FormsAuthentication.SignOut();
          //  Session.Clear();
          //  Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login", "AMS");
        }
    }
}