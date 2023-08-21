using GrpcServiceSpecialProjectTest.Models;
using Google.Protobuf.WellKnownTypes;
using System.Linq;

namespace GrpcServiceSpecialProjectTest.Mapper
{
    public class MyMapperCountry
    {
        public static Country CountryProtoMapper(CountryProtoCountry countryProto)
        {
            Country country = new Country();
            country.Id = countryProto.Id;
            country.Name = countryProto.Name;
            if (countryProto.Questionnaires != null)
                if (countryProto.Questionnaires.Count > 0)
                    foreach (var el in countryProto.Questionnaires)
                        if (!countryProto.Questionnaires.Any(p => p.Id == el.Id))
                            country.Questionnaires.Add(QuestionnaireProtoMapper(el));
            return country;
        }
        public static CountryProtoCountry CountryMapper(Country country)
        {
            CountryProtoCountry countryProto = new CountryProtoCountry();
            countryProto.Id = country.Id;
            countryProto.Name = country.Name;
            if (country.Questionnaires != null)
                if (country.Questionnaires.Count > 0)
                    foreach (var el in country.Questionnaires)
                        if (!country.Questionnaires.Any(p => p.Id == el.Id))
                            countryProto.Questionnaires.Add(QuestionnaireMapper(el));
            return countryProto;
        }
        public static QuestionnaireProtoCountry QuestionnaireMapper(Questionnaire questionnaire)
        {
            QuestionnaireProtoCountry questionnaireProto = new QuestionnaireProtoCountry();
            questionnaireProto.Id = questionnaire.Id;
            questionnaireProto.FirstName = questionnaire.FirstName;
            questionnaireProto.MiddleName = questionnaire.MiddleName;
            questionnaireProto.LastName = questionnaire.LastName;
            questionnaireProto.Birthday = Timestamp.FromDateTimeOffset(questionnaire.Birthday);
            questionnaireProto.CountryId = questionnaire.CountryId;
            if (questionnaire.Country != null)
                questionnaireProto.Country = CountryMapper(questionnaire.Country);
            return questionnaireProto;
        }
        public static Questionnaire QuestionnaireProtoMapper(QuestionnaireProtoCountry questionnaireProto)
        {
            Questionnaire questionnaire = new Questionnaire();
            questionnaire.Id = questionnaireProto.Id;
            questionnaire.FirstName = questionnaireProto.FirstName;
            questionnaire.MiddleName = questionnaireProto.MiddleName;
            questionnaire.LastName = questionnaireProto.LastName;
            questionnaire.Birthday = questionnaireProto.Birthday.ToDateTime();
            questionnaire.CountryId = questionnaireProto.CountryId;
            if (questionnaireProto.Country != null)
                questionnaire.Country = CountryProtoMapper(questionnaireProto.Country);
            return questionnaire;
        }
    }
}