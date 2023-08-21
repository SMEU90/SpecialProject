using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Linq;
using GrpcServiceSpecialProjectTest.Models;

namespace GrpcServiceSpecialProjectTest.Repositories.Interfaces
{
    public interface IProductCountryService
    {
        //Получение всех стран
        public Task<List<Country>> GetCountryListAsync();
        //Добавление страны
        public Task<Country> AddCountryAsync(Country country);
        //Обновление страны
        public Task<bool> UpdateCountryAsync(Country country);
        //Удаление страны
        public Task<bool> DeleteCountryAsync(int id);
        //Проверка наличия страны в бд
        public Task<bool> CheckCountryAsync(string name);
        //Отправка клиенту одной страны
        public Task<Country> GetOneCountryAsync(string name);

    }
}