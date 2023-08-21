using Grpc.Core;
using GrpcServiceSpecialProjectTest.Models;
using System.Threading.Tasks;
using System;
using ProductQuestionnaireService = GrpcServiceSpecialProjectTest.ProductQuestionnaireService.ProductQuestionnaireServiceBase;
using GrpcServiceSpecialProjectTest.Repositories.Interfaces;
using GrpcServiceSpecialProjectTest.Mapper;
using System.Diagnostics;

namespace GrpcServiceSpecialProjectTest
{
    public class QuestionnaireService : ProductQuestionnaireService.ProductQuestionnaireServiceBase
    {
        private readonly IProductQuestionnaireService _productQuestionnaireService;
        public QuestionnaireService(IProductQuestionnaireService productQuestionnaireIntegration)
        {
            _productQuestionnaireService = productQuestionnaireIntegration;
        }

        public async override Task<GetAllQuestionnaireResponse> GetAllQuestionnaire(EmptyMessageQuestionnaire request, ServerCallContext context)
        {
            try
            {
                var questionnaireData = await _productQuestionnaireService.GetQuestionnaireListAsync();
                var response = new GetAllQuestionnaireResponse();
                if (questionnaireData.Count > 0)
                    foreach (var el in questionnaireData)
                        response.Items.Add(MyMapperQuestionnaire.QuestionnaireMapper(el));
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        public async override Task<QuestionnaireProtoQuestionnaire> CreateQuestionnaire(QuestionnaireProtoQuestionnaire request, ServerCallContext context)
        {
            try
            {
                var questionnaire = MyMapperQuestionnaire.QuestionnaireProtoMapper(request);
                questionnaire.Country = new Country();
                questionnaire.Country.Name=request.Country.Name;
                questionnaire.Country.Id = request.Country.Id;
                questionnaire = await _productQuestionnaireService.AddQuestionnaireAsync(questionnaire);
                var questionnaireProto = MyMapperQuestionnaire.QuestionnaireMapper(questionnaire);
                return questionnaireProto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async override Task<UpdateQuestionnaireResponse> UpdateQuestionnaire(QuestionnaireProtoQuestionnaire request, ServerCallContext context)
        {
            try
            {
                var isUpdated = await _productQuestionnaireService.UpdateQuestionnaireAsync(MyMapperQuestionnaire.QuestionnaireProtoMapper(request));
                if (isUpdated)
                {
                    return new UpdateQuestionnaireResponse { IsUpdate = true };
                }
                else return new UpdateQuestionnaireResponse { IsUpdate = false };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        public async override Task<DeleteQuestionnaireResponse> DeleteQuestionnaire(QuestionnaireProtoQuestionnaire request, ServerCallContext context)
        {
            try
            {
                var isDeleted = await _productQuestionnaireService.DeleteQuestionnaireAsync(request.Id);
                if (isDeleted)
                {
                    return new DeleteQuestionnaireResponse { IsDelete = true };
                }
                else return new DeleteQuestionnaireResponse { IsDelete = false };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }
}
