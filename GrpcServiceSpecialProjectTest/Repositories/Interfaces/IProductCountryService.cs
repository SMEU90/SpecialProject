using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Linq;
using GrpcServiceSpecialProjectTest.Models;

namespace GrpcServiceSpecialProjectTest.Repositories.Interfaces
{
    public interface IProductCountryService
    {
        //��������� ���� �����
        public Task<List<Country>> GetCountryListAsync();
        //���������� ������
        public Task<Country> AddCountryAsync(Country country);
        //���������� ������
        public Task<bool> UpdateCountryAsync(Country country);
        //�������� ������
        public Task<bool> DeleteCountryAsync(int id);
        //�������� ������� ������ � ��
        public Task<bool> CheckCountryAsync(string name);
        //�������� ������� ����� ������
        public Task<Country> GetOneCountryAsync(string name);

    }
}