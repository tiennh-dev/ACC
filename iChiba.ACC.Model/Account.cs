﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace iChiba.ACC.Model
{
    public partial class Account
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public int? Parent { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
    }
}