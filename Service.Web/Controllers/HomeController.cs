using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Common;
using Service.Weather.Interfaces;

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
            Guid guid = Guid.NewGuid();
            var weather = ServiceProxyUtil.InvokeService<IWeatherService, string>(ServiceProxyUtil.GetServiceUri("Weather"), (weatherService) =>
            {
                return weatherService.GetCurrentWeather("Changchun");
            }, new CustomContextDataDto { ID = guid.ToString(), Name = DateTime.Now.ToString() }).Result;

            ViewData["Message"] = weather;
            return View();
        }

        public IActionResult Contact()
        {
            Guid guid = Guid.NewGuid();
            var weather = ServiceProxyUtil.InvokeService<IWeatherService, string>(ServiceProxyUtil.GetServiceUri("Weather"), (weatherService) =>
            {
                return weatherService.GetCurrentWeather("Changchun");
            }, new CustomContextDataDto { ID = guid.ToString(), Name = DateTime.Now.ToString() }).Result;
            ViewData["Message"] = weather;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
