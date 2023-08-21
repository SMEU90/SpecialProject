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
    public class ProductQuestionnaireService : IProductQuestionnaireService
    {
        private readonly ContextDB _dbContext;
        
        public ProductQuestionnaireService(ContextDB context) { _dbContext = context; }
        public ProductQuestionnaireService() { }
        //Получение всех анкет
        public async Task<List<Questionnaire>> GetQuestionnaireListAsync()
        {
            try
            {
                return await _dbContext.Questionnaires.ToListAsync();
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        //Добавление анкеты
        public async Task<Questionnaire> AddQuestionnaireAsync(Questionnaire questionnaire)
        {
            try
            {
                _dbContext.Countrys.Attach(questionnaire.Country);
                var result = await _dbContext.Questionnaires.AddAsync(questionnaire);
                await _dbContext.SaveChangesAsync();
                return result.Entity;
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        //Обновление анкеты
        public async Task<bool> UpdateQuestionnaireAsync(Questionnaire questionnaire)
        {
            try
            {
                var questionnaireLocal = await _dbContext.Questionnaires.FirstOrDefaultAsync(el => el.Id == questionnaire.Id);///////////////////
                questionnaireLocal.FirstName = questionnaire.FirstName;
                questionnaireLocal.MiddleName = questionnaire.MiddleName;
                questionnaireLocal.LastName = questionnaire.LastName;
                questionnaireLocal.Birthday = questionnaire.Birthday;
                questionnaireLocal.Country = questionnaire.Country;
                questionnaireLocal.CountryId= questionnaire.CountryId;
                var result = _dbContext.Questionnaires.Update(questionnaireLocal);
                await _dbContext.SaveChangesAsync();
                return result != null ? true : false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }

        }
        //Удаление анкеты
        public async Task<bool> DeleteQuestionnaireAsync(int id)
        {
            try
            {
                var filteredData = _dbContext.Questionnaires.FirstOrDefaultAsync(el => el.Id == id);
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
    }
}
