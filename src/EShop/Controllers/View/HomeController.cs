﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace EShop.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult FAQ()
		{
			return View();
		}

		public IActionResult Contact()
		{

			return View();
		}

		public IActionResult Detail()
		{

			return View();
		}

		public IActionResult Profile()
		{

			return View();
		}

		public IActionResult Order()
		{

			return View();
		}

		public IActionResult Product()
		{

			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
