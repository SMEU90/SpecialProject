using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SpecialProjectTest.Infrastructure.ValidationRules
{
    //Проверка правильности ввода
    public class AlphabetTextBoxValidationRule : ValidationRule
    {
        private static readonly Regex regex = new Regex("^[a-zA-Zа-яА-Я ]+$");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool canConvert = regex.IsMatch((string)value);
            if(!canConvert)
                return new ValidationResult(false, "Неккоректное заполнение поля");
            else
                return new ValidationResult(true, "");  
        }
    }
}
