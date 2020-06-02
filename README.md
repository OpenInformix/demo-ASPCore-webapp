# Inventory Management ASP.NET Core
This repository contains the inventory management application, built with ASP.NET Core 3.1 to illustrate performing CRUD operations in HCL Informix Database. 

## Prerequisites

* Visual Studio 2019 
* ASP.NET Core 3.1 
* .Net Core 3.1
* HCL Informix


## How to run the project

* Git clone this project to a location in your disk.
* Open the solution file(ASPCoreWebApp.sln) using the Visual Studio 2019.
* Restore the NuGet packages by rebuilding the solution.
* Change the connection string in the ProductController.cs file.
* Run the project(it will open a web browser with application running on it).


## In the UI we will get three menu (navigation) items
* Product Store : It is the Inventory Management Portal for Shop Admin.
* Shopping Mart : It is the shopping portal for buyers/customers. Which will display all the products available in store, and will give a purchase option for each product.
* Order Details : It will display the summary of all placed orders.