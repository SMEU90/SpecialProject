using SpecialProjectTest.Models;
using GrpcServiceSpecialProjectTest;
using Google.Protobuf.WellKnownTypes;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpecialProjectTest.Mapper
{
    public class MyMapperQuestionnaire
    {
        public static Country CountryProtoMapper(CountryProtoQuestionnaire countryProto)
        {
            Country country = new Country();
            country.Id = countryProto.Id;
            country.Name = countryProto.Name;
            if (countryProto.Questionnaires != null)
                if (countryProto.Questionnaires.Count > 0)
                    foreach (var el in countryProto.Questionnaires)
                        if (!countryProto.Questionnaires.Any(p => p.Id == el.Id))
                            country.Questionnaires.Add(MyMapperQuestionnaire.QuestionnaireProtoMapper(el));
            return country;
        }
        public static CountryProtoQuestionnaire CountryMapper(Country country)
        {
            CountryProtoQuestionnaire countryProto = new CountryProtoQuestionnaire();
            countryProto.Id = country.Id;
            countryProto.Name = country.Name;
            if (country.Questionnaires != null)
                if (country.Questionnaires.Count > 0)
                    foreach (var el in country.Questionnaires)
                        if (!country.Questionnaires.Any(p => p.Id == el.Id))
                            countryProto.Questionnaires.Add(MyMapperQuestionnaire.QuestionnaireMapper(el));
            return countryProto;
        }
        public static QuestionnaireProtoQuestionnaire QuestionnaireMapper(Questionnaire questionnaire)
        {
            QuestionnaireProtoQuestionnaire questionnaireProto = new QuestionnaireProtoQuestionnaire();
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
        public static Questionnaire QuestionnaireProtoMapper(QuestionnaireProtoQuestionnaire questionnaireProto)
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
        public static Questionnaire QuestionnaireProtoMapper(QuestionnaireProtoQuestionnaire questionnaireProto, ObservableCollection<Country> allCountryItemsSource)
        {
            Questionnaire questionnaire = new Questionnaire();
            questionnaire.Id = questionnaireProto.Id;
            questionnaire.FirstName = questionnaireProto.FirstName;
            questionnaire.MiddleName = questionnaireProto.MiddleName;
            questionnaire.LastName = questionnaireProto.LastName;
            questionnaire.Birthday = questionnaireProto.Birthday.ToDateTime();
            questionnaire.Country = allCountryItemsSource.FirstOrDefault(el=>el.Id== questionnaireProto.CountryId);
            questionnaire.CountryId = questionnaireProto.CountryId;
            return questionnaire;
        }
    }
}