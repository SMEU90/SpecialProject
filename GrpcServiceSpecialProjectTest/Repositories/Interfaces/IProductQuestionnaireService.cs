using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using GrpcServiceSpecialProjectTest.Models;

namespace GrpcServiceSpecialProjectTest.Repositories.Interfaces
{
    public interface IProductQuestionnaireService
    {
        //��������� ���� �����
        public Task<List<Questionnaire>> GetQuestionnaireListAsync();
        //���������� ������
        public Task<Questionnaire> AddQuestionnaireAsync(Questionnaire questionnaire);
        //���������� ������
        public Task<bool> UpdateQuestionnaireAsync(Questionnaire questionnaire);
        //�������� ������
        public Task<bool> DeleteQuestionnaireAsync(int id);
        


    }
}