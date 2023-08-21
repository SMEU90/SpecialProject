using Grpc.Core;
using System.Threading.Tasks;
using System;
using ProductCountryService = GrpcServiceSpecialProjectTest.ProductCountryService.ProductCountryServiceBase;
using GrpcServiceSpecialProjectTest.Repositories.Interfaces;
using GrpcServiceSpecialProjectTest.Mapper;
using System.Diagnostics;

namespace GrpcServiceSpecialProjectTest
{
    public class CountryService : ProductCountryService.ProductCountryServiceBase
    {
        private readonly IProductCountryService _productCountryService;
        public CountryService(IProductCountryService productCountryIntegration)
        {
            _productCountryService = productCountryIntegration;
        }

        public async override Task<GetAllCountryResponse> GetAllCountry(EmptyMessageCountry request, ServerCallContext context)
        {
            try
            {
                var countryData = await _productCountryService.GetCountryListAsync();
                var response = new GetAllCountryResponse();
                if (countryData.Count > 0)
                    foreach (var el in countryData)
                        response.Items.Add(MyMapperCountry.CountryMapper(el));
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        public async override Task<CountryProtoCountry> CreateCountry(CountryProtoCountry request, ServerCallContext context)
        {
            try
            {
                var country = await _productCountryService.AddCountryAsync(MyMapperCountry.CountryProtoMapper(request));
                var countryProto = MyMapperCountry.CountryMapper(country);
                return countryProto;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        public async override Task<UpdateCountryResponse> UpdateCountry(CountryProtoCountry request, ServerCallContext context)
        {
            try
            {
                var isUpdated = await _productCountryService.UpdateCountryAsync(MyMapperCountry.CountryProtoMapper(request));
                if (isUpdated)
                    return new UpdateCountryResponse { IsUpdate = true };
                else return new UpdateCountryResponse { IsUpdate = false };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        public async override Task<DeleteCountryResponse> DeleteCountry(CountryProtoCountry request, ServerCallContext context)
        {
            try
            {
                var isDeleted = await _productCountryService.DeleteCountryAsync(request.Id);
                if (isDeleted)
                    return new DeleteCountryResponse { IsDelete = true };
                else return new DeleteCountryResponse { IsDelete = false };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        public async override Task<CheckCountryResponse> CheckCountry(CheckCountryRequest request, ServerCallContext context)
        {
            try
            {
                var check = await _productCountryService.CheckCountryAsync(request.Name);
                if(check)
                    return new CheckCountryResponse { IsCheked = true };
                else return new CheckCountryResponse { IsCheked = false };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        public async override Task<CountryProtoCountry> GetCountry(CheckCountryRequest request, ServerCallContext context)
        {
            try
            {
                var response = await _productCountryService.GetOneCountryAsync(request.Name);
                return MyMapperCountry.CountryMapper(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }


    }
}
