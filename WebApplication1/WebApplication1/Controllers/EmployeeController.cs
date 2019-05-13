﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Filters;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class Customer
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return this.CustomerName + "|" + this.Address;
        }
    }

    
    public class EmployeeController : Controller
    {
        public Customer GetCustomer()
        {
            Customer c = new Customer();
            c.CustomerName = "Customer 1";
            c.Address = "Address1";
            return c;
        }

        [NonAction]
        public string SimpleMethod()
        {
            return "Hi, I am not action method";
        }

        [Authorize]
        public ActionResult Index()
        {
            EmployeeListViewModel employeeListViewModel = new EmployeeListViewModel();
            employeeListViewModel.UserName = User.Identity.Name;

            EmployeeBusinessLayer empBal = new EmployeeBusinessLayer();
            List<Employee> employees = empBal.GetEmployees();

            List<EmployeeViewModel> empViewModels = new List<EmployeeViewModel>();

            foreach( Employee emp in employees)
            {
                EmployeeViewModel empViewModel = new EmployeeViewModel();
                empViewModel.EmployeeName = emp.FirstName + " " + emp.LastName;
               
                empViewModel.Salary = emp.Salary.Value.ToString("C");
                if (emp.Salary > 15000)
                {
                    empViewModel.SalaryColor = "yellow";
                }
                else
                {
                    empViewModel.SalaryColor = "green";
                }
                empViewModels.Add(empViewModel);
            }
            employeeListViewModel.Employees = empViewModels;

            employeeListViewModel.FooterData = new FooterViewModel();
            employeeListViewModel.FooterData.CompanyName = "StepBystepSchools";
            employeeListViewModel.FooterData.Year = DateTime.Now.Year.ToString();

            return View("Index",employeeListViewModel);
        }

        [AdminFilter]
        public ActionResult AddNew()
        {
            CreateEmployeeViewModel employeeViewModel = new CreateEmployeeViewModel();
            employeeViewModel.FooterData = new FooterViewModel();
            employeeViewModel.FooterData.CompanyName = "StepByStepSchools";
            employeeViewModel.FooterData.Year = DateTime.Now.Year.ToString();
            employeeViewModel.UserName = User.Identity.Name;

            return View("CreateEmployee", employeeViewModel);
        }

        [AdminFilter]
        public ActionResult SaveEmployee(Employee e, string BtnSubmit)
        {
            switch (BtnSubmit)
            {
                case "Save Employee":
                    if(ModelState.IsValid)
                    {
                        EmployeeBusinessLayer empBal = new EmployeeBusinessLayer();
                        empBal.SaveEmployee(e);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        CreateEmployeeViewModel vm = new CreateEmployeeViewModel();
                        vm.FirstName = e.FirstName;
                        vm.LastName = e.LastName;
                        if (e.Salary.HasValue)
                        {
                            vm.Salary = e.Salary.ToString();
                        }
                        else
                        {
                            vm.Salary = ModelState["Salary"].Value.AttemptedValue;
                        }
                        vm.FooterData = new FooterViewModel();
                        vm.FooterData.CompanyName = "StepByStepSchools";
                        vm.FooterData.Year = DateTime.Now.Year.ToString();
                        vm.UserName = User.Identity.Name;

                        return View("CreateEmployee", vm);
                        
                    }
                    
                case "Cancel":
                    return RedirectToAction("Index", "Employee");
            }
            return new EmptyResult();
        }

        public ActionResult GetAddNewLink()
        {
            if (Convert.ToBoolean(Session["IsAdmin"]))
            {
                return PartialView("AddNewLink");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}