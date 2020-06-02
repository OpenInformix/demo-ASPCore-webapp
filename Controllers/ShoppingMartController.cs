using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreWebApp.Models;
using Informix.Net.Core;
using Microsoft.AspNetCore.Mvc;

namespace ASPCoreWebApp.Controllers
{
    public class ShoppingMartController : Controller
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
                }
                catch (Exception ex)
                {
                    string createProductTable = "Create table Product (productid serial PRIMARY KEY, productname varchar(50), price decimal(18,2), count int)";
                    IfxCommand cmd = new IfxCommand(createProductTable, Con);
                    cmd.ExecuteNonQuery();
                    string createOrderTable = "Create table orderdetails (orderid serial PRIMARY KEY, productid int, productname varchar(50), price decimal(18,2), count int, totalamount decimal(18,2))";
                    IfxCommand cmd1 = new IfxCommand(createOrderTable, Con);
                    cmd1.ExecuteNonQuery();

                    IfxDataAdapter ifx = new IfxDataAdapter("SELECT * FROM Product", Con);
                    ifx.Fill(table);
                }
            }
            return View(table);
        }

        // GET: /Product/Purchase/5
        public ActionResult Purchase(int id)
        {
            OrderDetailsModel productModel = new OrderDetailsModel();
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
        public ViewResult Purchase(OrderDetailsModel orderDetailsModel)
        {
            using (IfxConnection Con = new IfxConnection(connString))
            {
                Con.Open();
                int availableQuantity = 0;

                string selectProductDetails = "select Count from product where productid = ?";
                IfxCommand cmd = new IfxCommand(selectProductDetails, Con);
                cmd.Parameters.Add("productid", IfxType.Serial).Value = orderDetailsModel.ProductID;
                try
                {
                    IfxDataReader rows = cmd.ExecuteReader();
                    while (rows.Read())
                    {
                        availableQuantity = Convert.ToInt32(rows[0]);
                    }
                    rows.Close();
                }
                catch (IfxException ex)
                {
                    Con.Close();
                    orderDetailsModel.ErrorMessage = "Error : " + ex.Message;
                }

                if (orderDetailsModel.Count > availableQuantity)
                {
                    Con.Close();
                    orderDetailsModel.ErrorMessage = "Cannot purchase " + orderDetailsModel.Count + " quantities, available quantities are : " + availableQuantity;
                }
                else
                {
                    int newProductQuantity = availableQuantity - orderDetailsModel.Count;

                    string updateProductQuantity = "UPDATE Product SET count = ? Where productid = ?";
                    IfxCommand cmd1 = new IfxCommand(updateProductQuantity, Con);
                    cmd1.Parameters.Add("count", IfxType.Int).Value = newProductQuantity;
                    cmd1.Parameters.Add("productid", IfxType.Serial).Value = orderDetailsModel.ProductID;
                    cmd1.ExecuteNonQuery();

                    try
                    {
                        insertNewOrder(Con, orderDetailsModel);
                    }
                    catch (Exception ex)
                    {
                        string createOrderTable = "Create table orderdetails (orderid serial PRIMARY KEY, productid int, productname varchar(50), price decimal(18,2), count int, totalamount decimal(18,2))";
                        IfxCommand cmd2 = new IfxCommand(createOrderTable, Con);
                        cmd2.ExecuteNonQuery();
                        insertNewOrder(Con, orderDetailsModel);
                    }
                    finally
                    {
                        Con.Close();
                        orderDetailsModel.ErrorMessage = "Purchase successful";
                    }
                }
                return View(orderDetailsModel);
                //return RedirectToAction("Index");
            }
        }

        private void insertNewOrder(IfxConnection con, OrderDetailsModel orderDetailsModel)
        {
            decimal totalamount = orderDetailsModel.Price * orderDetailsModel.Count;

            string query = "INSERT INTO orderdetails (productid, productname, price, count, totalamount) VALUES(?, ?, ?, ?, ?)";
            IfxCommand cmd = new IfxCommand(query, con);
            cmd.Parameters.Add("productid", IfxType.Int).Value = orderDetailsModel.ProductID;
            cmd.Parameters.Add("productname", IfxType.VarChar).Value = orderDetailsModel.ProductName;
            cmd.Parameters.Add("price", IfxType.Decimal).Value = orderDetailsModel.Price;
            cmd.Parameters.Add("count", IfxType.Int).Value = orderDetailsModel.Count;
            cmd.Parameters.Add("totalamount", IfxType.Decimal).Value = totalamount;

            cmd.ExecuteNonQuery();
        }
    }
}