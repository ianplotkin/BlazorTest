﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace BlazorTest.Data.Models
{
    public partial class Need
    {
        public int GroceryId { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }

        public virtual Grocery Grocery { get; set; }
    }
}