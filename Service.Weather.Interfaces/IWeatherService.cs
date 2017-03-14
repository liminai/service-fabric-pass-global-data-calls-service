using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Service.Weather.Interfaces
{
    public interface IWeatherService: IService
    {
        Task<string> GetCurrentWeather(string cityName);

        Task<string> GetSupportCity(string provinceName);
    }
}
