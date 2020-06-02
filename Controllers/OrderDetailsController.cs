using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Informix.Net.Core;
using Microsoft.AspNetCore.Mvc;

namespace ASPCoreWebApp.Controllers
{
    public class OrderDetailsController : Controller
    {
        string connString = "DataBase=webapp;Server=ol_informix1410_9;User ID = informix; Password=Rinvoke1;";
        public IActionResult Index()
        {
            DataTable table = new DataTable();
            using (IfxConnection Con = new IfxConnection(connString))
            {
                Con.Open();
                try
                {
                    IfxDataAdapter ifx = new IfxDataAdapter("SELECT * FROM orderdetails", Con);
                    ifx.Fill(table);
                }
                catch (Exception ex)
                {
                    string createOrderTable = "Create table orderdetails (orderid serial PRIMARY KEY, productid int, productname varchar(50), price decimal(18,2), count int, totalamount decimal(18,2))";
                    IfxCommand cmd = new IfxCommand(createOrderTable, Con);
                    cmd.ExecuteNonQuery();

                    IfxDataAdapter ifx = new IfxDataAdapter("SELECT * FROM orderDetails", Con);
                    IfxDataAdapter ifx1 = new IfxDataAdapter("SELECT productid,price*count as totalamount from purchase", Con);
                    ifx.Fill(table);
                }
            }
            return View(table);
        }
    }
}