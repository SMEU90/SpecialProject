using Microsoft.EntityFrameworkCore;
using GrpcServiceSpecialProjectTest.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using GrpcServiceSpecialProjectTest.Models.Base;
using System;
using GrpcServiceSpecialProjectTest.Repositories.Interfaces;
using System.Diagnostics;

namespace GrpcServiceSpecialProjectTest.Repositories
{
    public class ProductCountryService : IProductCountryService
    {
        private readonly ContextDB _dbContext;
        
        public ProductCountryService(ContextDB context) { _dbContext = context; }
        public ProductCountryService() { }
        //Получение всех стран
        public async Task<List<Country>> GetCountryListAsync()
        {
            try
            {
                return await _dbContext.Countrys.ToListAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        //Добавление страны
        public async Task<Country> AddCountryAsync(Country country)
        {
            try
            {
                var result = await _dbContext.Countrys.AddAsync(country);
                await _dbContext.SaveChangesAsync();
                return result.Entity;
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        //Обновление страны
        public async Task<bool> UpdateCountryAsync(Country country)
        {
            try
            {
                var counntryLocal = await _dbContext.Countrys.FirstOrDefaultAsync(el => el.Id == country.Id);

                counntryLocal.Name = country.Name;
                var result = _dbContext.Countrys.Update(counntryLocal);
                await _dbContext.SaveChangesAsync();
                return result != null ? true : false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }

        }
        //Удаление страны
        public async Task<bool> DeleteCountryAsync(int id)
        {
            try
            {
                var filteredData = _dbContext.Countrys.FirstOrDefaultAsync(el => el.Id == id);
                var result = _dbContext.Remove(filteredData.Result);
                await _dbContext.SaveChangesAsync();
                return result != null ? true : false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        //Проверка наличия страны в бд
        public async Task<bool> CheckCountryAsync(string name)
        {
            try
            {
                return await _dbContext.Countrys.AnyAsync(el => el.Name == name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        //Вернуть одну страну
        public async Task<Country> GetOneCountryAsync(string name)
        {
            try
            {
                return await _dbContext.Countrys.FirstOrDefaultAsync(el => el.Name == name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }
}
