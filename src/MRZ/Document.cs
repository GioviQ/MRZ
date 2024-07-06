using System;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace MRZ
{
    public enum MrzFormat
    {
        Unknown,
        TD1,
        TD2,
        TD3,
        MRVA,
        MRVB,
        /// <summary>
        /// French ID card (1988 - 2021)
        /// </summary>
        IDFRA,
        /// <summary>
        /// Swiss Driving License
        /// </summary>
        SDL,
    }

    public sealed class Document
    {
        private const string Filler = "<"; // const string enables compile-time interpolation of line patterns (18 - 25% less execution time and allocated memory compared to const char)

        private static readonly int[] Weights = { 7, 3, 1 };

        public string Mrz { get; }
        public MrzFormat DocumentFormat { get; }
        public string DocumentType { get; }
        public string IssuingState { get; }
        public string DocumentNumber { get; }
        private char DocumentNumberCheckDigit { get; }
        public string OptionalData1 { get; }
        private char OptionalData1CheckDigit { get; }
        public DateTime BirthDate { get; }
        private char BirthDateCheckDigit { get; }
        /// <summary>
        /// Can be M, F, X or &lt;
        /// </summary>
        public char Gender { get; }
        public DateTime? ExpirationDate { get; }
        private char ExpirationDateCheckDigit { get; }
        public string Nationality { get; }
        /// <summary>
        /// May only be populated in case of a <see cref="MrzFormat.TD1"/> document
        /// </summary>
        public string OptionalData2 { get; }
        private char OverallCheckDigit { get; }
        /// <summary>
        /// This is the primary identifier as described in the official ICAO specification
        /// </summary>
        public string Surname { get; }
        /// <summary>
        /// This is the secondary identifier as described in the official ICAO specification
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parses the given MRZ according to the parsing rules of the detected <see cref="MrzFormat"/>
        /// </summary>
        /// <param name="mrz"></param>
        /// <exception cref="FormatException">The given MRZ doesn't match the pattern of the detected <see cref="MrzFormat"/> or a checksum check failed</exception>
        public Document(string mrz)
        {
            Mrz = mrz.ToUpperInvariant();

            // detect MRZ format

            int characterCount = Mrz.Count(c => !char.IsControl(c));

            switch (characterCount)
            {
                case 30 * 3:
                    DocumentFormat = MrzFormat.TD1;
                    break;
                case 36 * 2:
                    DocumentFormat = Mrz[0] == 'V'
                        ? MrzFormat.MRVB
                        : Mrz.StartsWith("IDFRA")
                            ? MrzFormat.IDFRA
                            : MrzFormat.TD2;
                    break;
                case 44 * 2:
                    DocumentFormat = Mrz[0] == 'V'
                        ? MrzFormat.MRVA
                        : Mrz[0] == 'P'
                            ? MrzFormat.TD3
                            : MrzFormat.Unknown;
                    break;
                case 9 + 30 + 30:
                    DocumentFormat = MrzFormat.SDL;
                    break;
                default:
                    DocumentFormat = MrzFormat.Unknown;
                    break;
            }

            // build and match regex pattern for detected MRZ format

            string pattern = null;

            switch (DocumentFormat)
            {
                case MrzFormat.TD1:
                    string td1Line1 = $"(?'{nameof(DocumentType)}'[ACI]{{1}}[A-Z0-9{Filler}]{{1}})(?'{nameof(IssuingState)}'[A-Z{Filler}]{{3}})(?'{nameof(DocumentNumber)}'[A-Z0-9{Filler}]{{9}})(?'{nameof(DocumentNumberCheckDigit)}'[0-9{Filler}]{{1}})(?'{nameof(OptionalData1)}'[A-Z0-9{Filler}]{{15}})";
                    string td1Line2 = $"(?'{nameof(BirthDate)}'[0-9]{{6}})(?'{nameof(BirthDateCheckDigit)}'[0-9]{{1}})(?'{nameof(Gender)}'[MFX{Filler}]{{1}})(?'{nameof(ExpirationDate)}'[0-9{Filler}]{{6}})(?'{nameof(ExpirationDateCheckDigit)}'[0-9]{{1}})(?'{nameof(Nationality)}'[A-Z{Filler}]{{3}})(?'{nameof(OptionalData2)}'[A-Z0-9{Filler}]{{11}})(?'{nameof(OverallCheckDigit)}'[0-9]{{1}})";
                    string td1Line3 = $"(?'{nameof(Surname)}'[A-Z0-9{Filler}]*?){Filler}{{2}}(?'{nameof(Name)}'[A-Z0-9{Filler}]*)";

                    pattern = $"^{td1Line1}[\r\n]*{td1Line2}[\r\n]*{td1Line3}[\r\n]*$";
                    break;
                case MrzFormat.TD2:
                    string td2Line1 = $"(?'{nameof(DocumentType)}'[ACI]{{1}}[A-Z0-9{Filler}]{{1}})(?'{nameof(IssuingState)}'[A-Z{Filler}]{{3}})(?'{nameof(Surname)}'[A-Z0-9{Filler}]*?){Filler}{{2}}(?'{nameof(Name)}'[A-Z0-9{Filler}]*)";
                    string td2Line2 = $"(?'{nameof(DocumentNumber)}'[A-Z0-9{Filler}]{{9}})(?'{nameof(DocumentNumberCheckDigit)}'[0-9]{{1}})(?'{nameof(Nationality)}'[A-Z{Filler}]{{3}})(?'{nameof(BirthDate)}'[0-9]{{6}})(?'{nameof(BirthDateCheckDigit)}'[0-9]{{1}})(?'{nameof(Gender)}'[MFX{Filler}]{{1}})(?'{nameof(ExpirationDate)}'[0-9]{{6}})(?'{nameof(ExpirationDateCheckDigit)}'[0-9]{{1}})(?'{nameof(OptionalData1)}'[A-Z0-9{Filler}]{{7}})(?'{nameof(OverallCheckDigit)}'[0-9]{{1}})";

                    pattern = $"^{td2Line1}[\r\n]*{td2Line2}[\r\n]*$";
                    break;
                case MrzFormat.TD3:
                    string td3Line1 = $"(?'{nameof(DocumentType)}'P{{1}}[A-Z0-9{Filler}]{{1}})(?'{nameof(IssuingState)}'[A-Z{Filler}]{{3}})(?'{nameof(Surname)}'[A-Z0-9{Filler}]*?){Filler}{{2}}(?'{nameof(Name)}'[A-Z0-9{Filler}]*)";
                    string td3Line2 = $"(?'{nameof(DocumentNumber)}'[A-Z0-9{Filler}]{{9}})(?'{nameof(DocumentNumberCheckDigit)}'[0-9]{{1}})(?'{nameof(Nationality)}'[A-Z{Filler}]{{3}})(?'{nameof(BirthDate)}'[0-9]{{6}})(?'{nameof(BirthDateCheckDigit)}'[0-9]{{1}})(?'{nameof(Gender)}'[MFX{Filler}]{{1}})(?'{nameof(ExpirationDate)}'[0-9]{{6}})(?'{nameof(ExpirationDateCheckDigit)}'[0-9]{{1}})(?'{nameof(OptionalData1)}'[A-Z0-9{Filler}]{{14}})(?'{nameof(OptionalData1CheckDigit)}'[0-9{Filler}]{{1}})(?'{nameof(OverallCheckDigit)}'[0-9]{{1}})";

                    pattern = $"^{td3Line1}[\r\n]*{td3Line2}[\r\n]*$";
                    break;
                case MrzFormat.MRVA:
                    string mrvaLine1 = $"(?'{nameof(DocumentType)}'V{{1}}[A-Z0-9{Filler}]{{1}})(?'{nameof(IssuingState)}'[A-Z{Filler}]{{3}})(?'{nameof(Surname)}'[A-Z0-9{Filler}]*?){Filler}{{2}}(?'{nameof(Name)}'[A-Z0-9{Filler}]*)";
                    string mrvaLine2 = $"(?'{nameof(DocumentNumber)}'[A-Z0-9{Filler}]{{9}})(?'{nameof(DocumentNumberCheckDigit)}'[0-9]{{1}})(?'{nameof(Nationality)}'[A-Z{Filler}]{{3}})(?'{nameof(BirthDate)}'[0-9]{{6}})(?'{nameof(BirthDateCheckDigit)}'[0-9]{{1}})(?'{nameof(Gender)}'[MFX{Filler}]{{1}})(?'{nameof(ExpirationDate)}'[0-9]{{6}})(?'{nameof(ExpirationDateCheckDigit)}'[0-9]{{1}})(?'{nameof(OptionalData1)}'[A-Z0-9{Filler}]{{16}})";

                    pattern = $"^{mrvaLine1}[\r\n]*{mrvaLine2}[\r\n]*$";
                    break;
                case MrzFormat.MRVB:
                    string mrvbLine1 = $"(?'{nameof(DocumentType)}'V{{1}}[A-Z0-9{Filler}]{{1}})(?'{nameof(IssuingState)}'[A-Z{Filler}]{{3}})(?'{nameof(Surname)}'[A-Z0-9{Filler}]*?){Filler}{{2}}(?'{nameof(Name)}'[A-Z0-9{Filler}]*)";
                    string mrvbLine2 = $"(?'{nameof(DocumentNumber)}'[A-Z0-9{Filler}]{{9}})(?'{nameof(DocumentNumberCheckDigit)}'[0-9]{{1}})(?'{nameof(Nationality)}'[A-Z{Filler}]{{3}})(?'{nameof(BirthDate)}'[0-9]{{6}})(?'{nameof(BirthDateCheckDigit)}'[0-9]{{1}})(?'{nameof(Gender)}'[MFX{Filler}]{{1}})(?'{nameof(ExpirationDate)}'[0-9]{{6}})(?'{nameof(ExpirationDateCheckDigit)}'[0-9]{{1}})(?'{nameof(OptionalData1)}'[A-Z0-9{Filler}]{{8}})";

                    pattern = $"^{mrvbLine1}[\r\n]*{mrvbLine2}[\r\n]*$";
                    break;
                case MrzFormat.IDFRA:
                    string idfraLine1 = $"(?'{nameof(DocumentType)}'[ACI]{{1}}[A-Z0-9{Filler}]{{1}})(?'{nameof(IssuingState)}'[A-Z{Filler}]{{3}})(?'{nameof(Surname)}'[A-Z0-9{Filler}]{{25}})(?'{nameof(OptionalData1)}'[0-9{Filler}]{{6}})";
                    string idfraLine2 = $"(?'{nameof(DocumentNumber)}'[A-Z0-9{Filler}]{{12}})(?'{nameof(DocumentNumberCheckDigit)}'[0-9{Filler}]{{1}})(?'{nameof(Name)}'[A-Z{Filler}]{{14}})(?'{nameof(BirthDate)}'[0-9]{{6}})(?'{nameof(BirthDateCheckDigit)}'[0-9]{{1}})(?'{nameof(Gender)}'[MFX{Filler}]{{1}})(?'{nameof(OverallCheckDigit)}'[0-9]{{1}})";

                    pattern = $"^{idfraLine1}[\r\n]*{idfraLine2}[\r\n]*$";
                    break;
                case MrzFormat.SDL:
                    string sdlLine1 = $"(?:[A-Z0-9]{{3}}[0-9]{{3}}[DFIR]{{1}}{Filler}{{2}})";
                    string sdlLine2 = $"(?'{nameof(DocumentType)}'FA)(?'{nameof(IssuingState)}'CHE|LIE)(?'{nameof(DocumentNumber)}'[0-9]{{12}}){Filler}{{2}}(?'{nameof(BirthDate)}'[0-9]{{6}}){Filler}{{5}}";
                    string sdlLine3 = $"(?'{nameof(Surname)}'[A-Z0-9{Filler}]*?){Filler}{{2}}(?'{nameof(Name)}'[A-Z0-9{Filler}]*)";

                    pattern = $"^{sdlLine1}[\r\n]*{sdlLine2}[\r\n]*{sdlLine3}[\r\n]*$";
                    break;
                default:
                    throw new FormatException("Unknown document format");
            }

            Match match = Regex.Match(Mrz, pattern, RegexOptions.Singleline);

            if (!match.Success)
            {
                throw new FormatException($"Invalid {DocumentFormat} document format");
            }

            // populate properties from successful regex match

            DocumentType = GetTrimmedValue(match, nameof(DocumentType));
            IssuingState = GetTrimmedValue(match, nameof(IssuingState));
            DocumentNumberCheckDigit = GetChar(match, nameof(DocumentNumberCheckDigit));
            DocumentNumber = GetTrimmedValue(match, nameof(DocumentNumber));
            OptionalData1CheckDigit = GetChar(match, nameof(OptionalData1CheckDigit));
            OptionalData1 = GetTrimmedValue(match, nameof(OptionalData1));
            BirthDateCheckDigit = GetChar(match, nameof(BirthDateCheckDigit));
            BirthDate = GetAndCheckDate(match, BirthDateCheckDigit, nameof(BirthDate)) ?? throw new FormatException("No birthdate provided");
            Gender = GetChar(match, nameof(Gender));
            ExpirationDateCheckDigit = GetChar(match, nameof(ExpirationDateCheckDigit));
            ExpirationDate = GetAndCheckDate(match, ExpirationDateCheckDigit, nameof(ExpirationDate));
            Nationality = GetTrimmedValue(match, nameof(Nationality));
            OptionalData2 = GetTrimmedValue(match, nameof(OptionalData2));
            OverallCheckDigit = GetChar(match, nameof(OverallCheckDigit));
            Surname = GetName(match, nameof(Surname));
            Name = GetName(match, nameof(Name));

            // apply correction to values if necessary

            if (ExpirationDate < BirthDate)
            {
                ExpirationDate = ExpirationDate.Value.AddYears(100);
            }

            if (DocumentFormat == MrzFormat.IDFRA)
            {
                Name = Name.Replace("  ", ", ");
            }

            // check validity

            CheckValidity(DocumentNumber, DocumentNumberCheckDigit, nameof(DocumentNumber));
            CheckValidity(OptionalData1, OptionalData1CheckDigit, nameof(OptionalData1));

            int sum = 0;

            switch (DocumentFormat)
            {
                case MrzFormat.TD1:
                    // Line 1
                    sum += ComputeWeightedSum(DocumentNumber, 0);
                    sum += ComputeWeightedSummand(DocumentNumberCheckDigit, 9);
                    sum += ComputeWeightedSum(OptionalData1, 10);
                    // Line 2
                    sum += ComputeWeightedSum(GetValue(match, nameof(BirthDate)), 25);
                    sum += ComputeWeightedSummand(BirthDateCheckDigit, 31);
                    sum += ComputeWeightedSum(GetValue(match, nameof(ExpirationDate)), 32);
                    sum += ComputeWeightedSummand(ExpirationDateCheckDigit, 38);
                    sum += ComputeWeightedSum(OptionalData2, 39);
                    break;
                case MrzFormat.TD2:
                    // Line 2
                    sum += ComputeWeightedSum(DocumentNumber, 0);
                    sum += ComputeWeightedSummand(DocumentNumberCheckDigit, 9);
                    sum += ComputeWeightedSum(GetValue(match, nameof(BirthDate)), 10);
                    sum += ComputeWeightedSummand(BirthDateCheckDigit, 16);
                    sum += ComputeWeightedSum(GetValue(match, nameof(ExpirationDate)), 17);
                    sum += ComputeWeightedSummand(ExpirationDateCheckDigit, 23);
                    sum += ComputeWeightedSum(OptionalData1, 24);
                    break;
                case MrzFormat.TD3:
                    // Line 2
                    sum += ComputeWeightedSum(DocumentNumber, 0);
                    sum += ComputeWeightedSummand(DocumentNumberCheckDigit, 9);
                    sum += ComputeWeightedSum(GetValue(match, nameof(BirthDate)), 10);
                    sum += ComputeWeightedSummand(BirthDateCheckDigit, 16);
                    sum += ComputeWeightedSum(GetValue(match, nameof(ExpirationDate)), 17);
                    sum += ComputeWeightedSummand(ExpirationDateCheckDigit, 23);
                    sum += ComputeWeightedSum(OptionalData1, 24);
                    sum += ComputeWeightedSummand(OptionalData1CheckDigit, 38);
                    break;
                case MrzFormat.IDFRA:
                    // Line 1
                    sum += ComputeWeightedSum(DocumentType, 0);
                    sum += ComputeWeightedSum(IssuingState, 2);
                    sum += ComputeWeightedSum(Surname, 5);
                    sum += ComputeWeightedSum(OptionalData1, 30);
                    // Line 2
                    sum += ComputeWeightedSum(DocumentNumber, 36);
                    sum += ComputeWeightedSummand(DocumentNumberCheckDigit, 48);
                    sum += ComputeWeightedSum(Name, 49);
                    sum += ComputeWeightedSum(GetValue(match, nameof(BirthDate)), 63);
                    sum += ComputeWeightedSummand(BirthDateCheckDigit, 69);
                    sum += ComputeWeightedSummand(Gender, 70);
                    break;
            }

            if (!CheckValidity(sum, OverallCheckDigit))
            {
                throw new FormatException("Overall check failed");
            }
        }

        private static string GetValue(Match match, string groupName)
        {
            return match.Groups[groupName].Value;
        }

        private static char GetChar(Match match, string groupName)
        {
            string str = GetValue(match, groupName);
            return str.Length > 0 ? str[0] : default;
        }

        private static string GetTrimmedValue(Match match, string groupName)
        {
            return GetValue(match, groupName).TrimEnd(Filler[0]);
        }

        private static string GetName(Match match, string groupName)
        {
            return GetTrimmedValue(match, groupName).Replace(Filler[0], ' ');
        }

        private static int CharToInt(char c)
        {
            return char.IsDigit(c)
                ? c - '0'
                : char.IsLetter(c)
                    ? c - 'A' + 10 // Letters start at value 10
                    : 0; // '<' should be 0
        }

        private static int ComputeWeightedSummand(char c, int weight)
        {
            return CharToInt(c) * Weights[weight % Weights.Length];
        }

        private static int ComputeWeightedSum(string text, int startWeight)
        {
            return text.Sum(c => ComputeWeightedSummand(c, startWeight++));
        }

        private static bool CheckValidity(int sum, char checkDigit)
        {
            return (sum % 10) == CharToInt(checkDigit);
        }

        private static bool CheckValidity(string text, char checkDigit)
        {
            int sum = ComputeWeightedSum(text, 0);
            return CheckValidity(sum, checkDigit);
        }

        private static void CheckValidity(string text, char checkDigit, string fieldName)
        {
            if (char.IsDigit(checkDigit) && !CheckValidity(text, checkDigit))
            {
                throw new FormatException(fieldName + " check failed");
            }
        }

        private static int ParseNumber(string text, int startIdx, int endIdx)
        {
            int sum = 0;
            while (startIdx < endIdx)
            {
                sum *= 10;
                sum += CharToInt(text[startIdx++]);
            }
            return sum;
        }

        private static int ParseNumber(string text)
        {
            return ParseNumber(text, 0, text.Length);
        }

        private static DateTime? ParseDate(string text)
        {
            if (text.Length != 6 || text.Any(c => !char.IsDigit(c)))
            {
                return null;
            }

            int year = ParseNumber(text, 0, 2); // e.g. 93
            int month = ParseNumber(text, 2, 4);
            int day = ParseNumber(text, 4, 6);

            int currentYear = DateTime.Now.Year; // e.g. 2024
            int currentDecade = currentYear % 100; // e.g. 24
            bool isCurrentCentury = year <= currentDecade;

            currentYear -= currentDecade; // e.g. 2000

            if (!isCurrentCentury)
            {
                currentYear -= 100; // e.g. 1900
            }

            year += currentYear; // e.g. 1993

            return new DateTime(year, month, day);
        }

        private static DateTime? GetAndCheckDate(Match match, char checkDigit, string groupName)
        {
            string text = GetValue(match, groupName);
            DateTime? date = ParseDate(text);

            if (date != null)
            {
                CheckValidity(text, checkDigit, groupName);
            }

            return date;
        }
    }
}
