using NakliyeTakip.MAUI.Dto;
using NakliyeTakip.MAUI.Services.Refit;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NakliyeTakip.MAUI.Services
{
    public class LocationService(ILocationRefitService locationRefitService)
    {
        public async Task<object> InsertCurrentLocation(InsertCurrentLocationRequest request)
        {
            var response = await locationRefitService.InsertCurrentLocation(request);

            return ServiceResult.Success();
        }


    }
}
