using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using GrpcServiceSpecialProjectTest.Models;

namespace GrpcServiceSpecialProjectTest.Repositories.Interfaces
{
    public interface IProductQuestionnaireService
    {
        //Получение всех анкет
        public Task<List<Questionnaire>> GetQuestionnaireListAsync();
        //Добавление анкеты
        public Task<Questionnaire> AddQuestionnaireAsync(Questionnaire questionnaire);
        //Обновление анкеты
        public Task<bool> UpdateQuestionnaireAsync(Questionnaire questionnaire);
        //Удаление анкеты
        public Task<bool> DeleteQuestionnaireAsync(int id);
        


    }
}