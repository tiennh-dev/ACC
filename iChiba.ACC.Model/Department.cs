﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace iChiba.ACC.Model
{
    public partial class Department
    {
        public Department()
        {
            Employee = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string DepartId { get; set; }
        public string DepartName { get; set; }
        public int? Parent { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}