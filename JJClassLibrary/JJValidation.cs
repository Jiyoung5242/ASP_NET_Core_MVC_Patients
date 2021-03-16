using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace JJClassLibrary
{
    public static class JJValidation
    {
        //Int32 MaxWords = 10;

        public static string JJCapitalize(string input)
        {
            string rtnStr = "";
            if (input == null)
            {
                return rtnStr;
            }
            else
            {
                rtnStr = input.Trim();
                rtnStr = Regex.Replace(rtnStr, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
                return rtnStr;

            }
        }

        public static string JJExtractDigits(string input)
        {
            string rtnStr = "";

            if (input != null)
            {
                foreach (char c in input)
                {
                    if (Char.IsDigit(c))
                    {
                        rtnStr += c.ToString();
                    }
                }
            }
            else return input;


            return rtnStr;
        }

        public static Boolean JJPostalCodeValidation(string input)
        {
            Boolean rtnBool = true;

            if (input == null || input == "")
            {
                return rtnBool;
            }
            else
            {
                input = input.Trim();
                rtnBool = Regex.IsMatch(input, "^[ABCEGHJ-NPRSTVXYabceghj-nprstvxy]{1}[0-9]{1}[ABCEGHJ-NPRSTV-Zabceghj-nprstv-z]{1}[ ]?[0-9]{1}[ABCEGHJ-NPRSTV-Zabceghj-nprstv-z]{1}[0-9]{1}$");

                return rtnBool;
            }

        }

        public static string JJPostalCodeFormat(string input)
        {
            string rtnStr = "";

            if (input != null)
            {
                if (!input.Contains(" "))
                {
                    rtnStr = input.Substring(0, 3) + " " + input.Substring(3);
                }

                rtnStr = rtnStr.ToUpper();
                return rtnStr;
            }
            else
            {
                return input;
            }
        }

        public static Boolean JJZipCodeValidation(ref string input)
        {

            if (input == null)
            {
                input = "";
                return true;
            }
            else
            {
                string digitStr = JJExtractDigits(input);
                if (digitStr.Length == 5)
                {
                    input = digitStr;
                    return true;
                }
                else if (digitStr.Length == 9)
                {
                    input = digitStr.Substring(0, 5) + "-" + digitStr.Substring(5);
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }


        public class JJPostalCodeValidationAttribute : ValidationAttribute
        {
            Int32 MaxWords = 10;

            public JJPostalCodeValidationAttribute(Int32 maxWords, Int32 minWords)
            {
                ErrorMessage = "{0} cannot be less than {2} or longer then {1} words";
                MaxWords = maxWords;

            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null && (value.ToString().Split(' ').Length > MaxWords))
                {
                    return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName, MaxWords));
                }
                else
                    return ValidationResult.Success;
            }

        }
       
    }
}

