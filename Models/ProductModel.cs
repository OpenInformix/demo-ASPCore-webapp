﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCoreWebApp.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [DisplayName("Price Per Unit")]
        public decimal Price { get; set; }
        public int Count { get; set; }

    }
}
