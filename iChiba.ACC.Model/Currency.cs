﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace iChiba.ACC.Model
{
    public partial class Currency
    {
        public int Id { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int? Exchange { get; set; }
        public string AccoountCash { get; set; }
        public string AccoountDeposits { get; set; }
        public bool Active { get; set; }
    }
}