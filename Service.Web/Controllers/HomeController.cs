using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Common;
using Service.Weather.Interfaces;
using System.Runtime.Remoting.Messaging;

namespace SFWorld.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            string userAgent = Request.Headers["User-Agent"].ToString();
            var weather = ServiceProxyUtil.InvokeService<IWeatherService, string>(ServiceProxyUtil.GetServiceUri("Weather"), (weatherService) =>
            {
                return weatherService.GetCurrentWeather("北京");
            }, new CustomContextDataDto { ID = userAgent, Name = DateTime.Now.ToString() }).Result;

            ViewData["Message"] = weather;
            return View();
        }

        public IActionResult Contact()
        {
            string userAgent = Request.Headers["User-Agent"].ToString();
            var cities = ServiceProxyUtil.InvokeService<IWeatherService, string>(ServiceProxyUtil.GetServiceUri("Weather"), (weatherService) =>
            {
                return weatherService.GetSupportCity("辽宁");
            }, new CustomContextDataDto { ID = userAgent, Name = DateTime.Now.ToString() }).Result;
            ViewData["Message"] = cities;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
