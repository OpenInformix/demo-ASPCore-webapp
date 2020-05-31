using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Informix.Net.Core;
using System.Data;
using ASPCoreWebApp.Models;

namespace ASPCoreWebApp.Controllers
{
    public class ProductController : Controller
    {
        string connString = "DataBase=webapp;Server=ol_informix1410_9;User ID = informix; Password=Rinvoke1;";
        [HttpGet]
        public ActionResult Index()
        {
            DataTable table = new DataTable();
            using (IfxConnection Con = new IfxConnection(connString))
            {
                Con.Open();
                try
                {
                    IfxDataAdapter ifx = new IfxDataAdapter("SELECT * FROM Product", Con);
                    ifx.Fill(table);
                } catch (Exception ex)
                {
                    string createTable = "Create table Product (productid serial PRIMARY KEY, productname varchar(50), price decimal(18,2), count int)";
                    IfxCommand cmd = new IfxCommand(createTable, Con);
                    cmd.ExecuteNonQuery();
                    IfxDataAdapter ifx = new IfxDataAdapter("SELECT * FROM Product", Con);
                    ifx.Fill(table);
                }
            }
            return View(table);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new ProductModel());
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductModel productModel)
        {
            using (IfxConnection Con = new IfxConnection(connString))
            {
                Con.Open();
                string query = "INSERT INTO Product (productname, price, count) VALUES(?, ?, ?)";
                IfxCommand cmd = new IfxCommand(query, Con);
                cmd.Parameters.Add("productname", IfxType.VarChar).Value = productModel.ProductName;
                cmd.Parameters.Add("price", IfxType.Decimal).Value = productModel.Price;
                cmd.Parameters.Add("count", IfxType.Int).Value = productModel.Count;

                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // GET: /Product/Edit/5
        public ActionResult Edit(int id)
        {
            ProductModel productModel = new ProductModel();
            DataTable tblProd = new DataTable();
            using (IfxConnection Con = new IfxConnection(connString))
            {
                Con.Open();
                // Prone to SQL enjection
                string query = "SELECT * FROM Product Where productid = ?";
                IfxDataAdapter ifx = new IfxDataAdapter(query, Con);
                ifx.SelectCommand.Parameters.Add("productid", IfxType.Serial).Value = id;
                ifx.Fill(tblProd);
            }
            if (tblProd.Rows.Count == 1)
            {
                productModel.ProductID = Convert.ToInt32(tblProd.Rows[0][0].ToString());
                productModel.ProductName = tblProd.Rows[0][1].ToString();
                productModel.Price = Convert.ToDecimal(tblProd.Rows[0][2].ToString());
                productModel.Count = Convert.ToInt32(tblProd.Rows[0][3].ToString());
                return View(productModel);
            }
            else
                return RedirectToAction("Index");
        }

        // POST: /Product/Edit/5
        [HttpPost]
        public ActionResult Edit(ProductModel productModel)
        {
            using (IfxConnection Con = new IfxConnection(connString))
            {
                Con.Open();
                string query = "UPDATE Product SET productname = ? , price= ? , count = ? Where productid = ?";
                IfxCommand cmd = new IfxCommand(query, Con);
                cmd.Parameters.Add("productname", IfxType.VarChar).Value = productModel.ProductName;
                cmd.Parameters.Add("price", IfxType.Decimal).Value = productModel.Price;
                cmd.Parameters.Add("count", IfxType.Int).Value = productModel.Count;
                cmd.Parameters.Add("productid", IfxType.Serial).Value = productModel.ProductID;
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        //
        // GET: Product/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (IfxConnection Con = new IfxConnection(connString))
            {
                Con.Open();
                string query = "DELETE FROM Product WHere productid = ?";
                IfxCommand cmd = new IfxCommand(query, Con);
                cmd.Parameters.Add("productid", IfxType.Serial).Value = id;
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}