using Microsoft.AspNetCore.Mvc;
using JJClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static JJClassLibrary.JJValidation;
using System.Text.RegularExpressions;

namespace JJPatients.Models
{

    [ModelMetadataType(typeof(JJPatientMetadata))]
    public partial class Patient : IValidatableObject
    {

        private readonly PatientsContext _context = new PatientsContext();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FirstName == null || FirstName.Trim() == "")
                yield return new ValidationResult("First name cannot be empty or just blanks",
                                                    new[] { nameof(FirstName) });
            else FirstName = JJValidation.JJCapitalize(FirstName.Trim());

            if (LastName == null || LastName.Trim() == "")
                yield return new ValidationResult("Last name cannot be empty or just blanks",
                                                    new[] { nameof(LastName) });
            else LastName = JJValidation.JJCapitalize(LastName.Trim());

            if (Gender == null || Gender.Trim() == "")
                yield return new ValidationResult("Gender cannot be empty or just blanks", new[] { nameof(Gender) });

            else if (Gender != "M" && Gender != "F" && Gender != "X" && Gender != "m" && Gender != "f" && Gender != "x")
                yield return new ValidationResult("Gender must be either 'M', 'F' or 'X'", new[] { "Gender" });

            else Gender = JJValidation.JJCapitalize(Gender);

            Address = JJValidation.JJCapitalize(Address);
            City = JJValidation.JJCapitalize(City);



            string countryCode = "";
            string firstPostalCode = "";
            //var patient = _context.Patient.FirstOrDefault();
            if (ProvinceCode != null)
            {

                var province = _context.Province.Where(p => p.ProvinceCode == ProvinceCode);

                if (!province.Any())
                {
                    yield return new ValidationResult("Province Code is not on file", new[] { "ProvinceCode" });

                }
                else
                {
                    countryCode = province.FirstOrDefault().CountryCode;
                    firstPostalCode = province.FirstOrDefault().FirstPostalLetter;
                }
            }

            if (PostalCode != null && (ProvinceCode == null || countryCode == ""))
            {
                yield return new ValidationResult("Province Code, if provided, is required to validate Postal Code", new[] { "ProvinceCode" });

            }
            else if (PostalCode != null)
            {
                if (countryCode == "CA")
                {
                    if (JJValidation.JJPostalCodeValidation(PostalCode))
                    {
                        if(firstPostalCode.Contains(PostalCode.ToUpper().Substring(0,1)))

                        //if (JJValidation.JJPostalCodeFirstChar(patient.PostalCode, patient.ProvinceCode))
                        {
                            PostalCode = JJValidation.JJPostalCodeFormat(PostalCode);

                        }
                        else
                        {
                            yield return new ValidationResult("First letter of Postal Code is not valid for given province", new[] { "PostalCode" });
                            yield return new ValidationResult("Province code is not proper to first Postal Code", new[] { "ProvinceCode" });

                        }
                    }
                    else
                    {
                        yield return new ValidationResult("Postal Code is not code pattern: A3A 3A3", new[] { "PostalCode" });

                    }
                }
                else
                {
                    string postcode = PostalCode;
                    if (JJValidation.JJZipCodeValidation(ref postcode))
                    {
                        PostalCode = postcode;
                    }
                    else
                    {
                        yield return new ValidationResult("Province Code is is not code patter: 55555 or 12345-6789", new[] { "ProvinceCode" });

                    }
                }
            }


            if (Ohip != null && Ohip.Trim() != "")
            {
                Ohip = Ohip.ToUpper();
                if (!Regex.IsMatch(Ohip, @"^\d{4}-\d{3}-\d{3}-[a-zA-Z]{2}$"))
                {
                    yield return new ValidationResult("OHIP, if provided, must match pattern: 1234 - 123 - 123 - XX", new[] { "Ohip" });
                }

            }

            if (HomePhone != null && HomePhone.Trim() != ""){

                string digitHomePhone = "";
                digitHomePhone = JJValidation.JJExtractDigits(HomePhone);
                if (digitHomePhone.Length != 10)

                    yield return new ValidationResult("Home phone, if provided,  must be 10 digits: 123-123-1234", new[] { nameof(HomePhone) });
                else
                    HomePhone = digitHomePhone.Substring(0, 3) + "-" + digitHomePhone.Substring(3, 3) + "-" + digitHomePhone.Substring(6);
            }

            if(DateOfBirth != null)
            {
                if(DateOfBirth > DateTime.Now)
                {
                    yield return new ValidationResult("Date of Birth cannot be in the future", new[] { nameof(DateOfBirth) });

                }
            }

            if (DateOfDeath != null)
            {
                if (DateOfDeath > DateTime.Now)
                {
                    yield return new ValidationResult("Date of Death cannot be in the future", new[] { nameof(DateOfDeath) });

                }
            }

            if (Deceased == true && DateOfDeath == null)
            {
                yield return new ValidationResult("If Deceased is true, a Date Of Death is required", new[] { nameof(DateOfDeath) });

            } else if(Deceased == false && DateOfDeath != null)
            {
                yield return new ValidationResult("Deceased must be true if Date Of Death is provided", new[] { nameof(Deceased) });

            }

            yield return ValidationResult.Success;
        }
    }

    public class JJPatientMetadata
    {

        public int PatientId { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Street Address")]
        public string Address { get; set; }
        public string City { get; set; }
        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "OHIP")]
        //[RegularExpression(@"^\d{4}-\d{4}-\d{3}-\w\w$", ErrorMessage = "OHIP, if provided, must match pattern: 1234-123-123-XX")]
        public string Ohip { get; set; }

        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateOfBirth { get; set; }
        public bool Deceased { get; set; }

        [Display(Name =  "Date of Death")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateOfDeath { get; set; }
        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }
        [Required]
        public string Gender { get; set; }

    }
}
