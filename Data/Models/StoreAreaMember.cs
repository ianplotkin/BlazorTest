﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace BlazorTest.Data.Models
{
    public partial class StoreAreaMember
    {
        public int GroceryId { get; set; }
        public int StoreAreaId { get; set; }

        public virtual Grocery Grocery { get; set; }
        public virtual StoreArea StoreArea { get; set; }
    }
}