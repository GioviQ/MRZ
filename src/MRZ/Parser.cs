using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MRZ
{
    public class Parser
    {
        private char filler = '<';
        private static readonly int[] weights = { 7, 3, 1 };
        private readonly Dictionary<char, int> mappedValues = new Dictionary<char, int>();
        private const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private int sum;
        private const int size1 = 30;
        private const int size2 = 36;
        private const int size3 = 44;
        public Parser(char filler = '<')
        {
            this.filler = filler;

            for (int i = 0; i < charset.Length; ++i)
            {
                mappedValues.Add(charset[i], i);
            }

            mappedValues.Add(filler, 0);
        }

        private void computeWeightedSum(string line, int start, int end, int w)
        {
            for (int i = start, j = w; i <= end; ++i, ++j)
            {
                sum += mappedValues[line[i]] * weights[j % 3];
            }
        }

        private bool checkValidity(string line, int start, int end, int check)
        {
            sum = 0;
            computeWeightedSum(line, start, end, 0);
            return (sum % 10) == mappedValues[line[check]];
        }

        private DateTime parseDate(string date)
        {
            int y = int.Parse(date.Substring(0, 2));

            int yy = ((y <= DateTime.Now.Year % 100) ? 2000 : 1900) + y;

            int m = int.Parse(date.Substring(2, 2));

            int d = int.Parse(date.Substring(4, 2));

            return new DateTime(yy, m, d);
        }

        private void setNames(Document doc, string group)
        {
            var names = group.Trim(filler).Split(new string[] { $"{filler}{filler}" }, StringSplitOptions.RemoveEmptyEntries);

            doc.Surname = names[0].Replace($"{filler}", " ");
            doc.Name = names[1].Replace($"{filler}", " ");
        }
        public Document DetectFields(string mrz)
        {
            var doc = new Document();

            mrz = mrz.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).ToUpperInvariant();

            switch (mrz.Length)
            {
                case size1 * 3:
                    doc.Format = MrzFormat.TD1;
                    break;

                case size2 * 2:
                    doc.Format = mrz.StartsWith("V") ? MrzFormat.MRVB : MrzFormat.TD2;
                    break;

                case size3 * 2:
                    doc.Format = mrz.StartsWith("V") ? MrzFormat.MRVA : MrzFormat.TD3;
                    break;
            }

            switch (doc.Format)
            {
                case MrzFormat.TD1:
                    var regEx = new Regex($"([A|C|I][A-Z0-9{filler}]{{1}})([A-Z{filler}]{{3}})([A-Z0-9{filler}]{{9}})([0-9{filler}]{{1}})([A-Z0-9{filler}]{{15}})");

                    var line1 = mrz.Substring(0, size1);

                    var match = regEx.Match(line1);

                    doc.Type = match.Groups[1].Value.Trim(filler);
                    doc.CountryCode = match.Groups[2].Value;
                    doc.Number = match.Groups[3].Value.Trim(filler);

                    if (doc.Number != string.Empty && match.Groups[4].Value.Trim(filler) != string.Empty && !checkValidity(line1, 5, 13, 14))
                        throw new Exception("Document number check failed");

                    doc.OptionalData1 = match.Groups[5].Value.Trim(filler);

                    regEx = new Regex($"([0-9]{{6}})([0-9]{{1}})([M|F|X|{filler}]{{1}})([0-9]{{6}})([0-9]{{1}})([A-Z{filler}]{{3}})([A-Z0-9{filler}]{{11}})([0-9]{{1}})");

                    var line2 = mrz.Substring(size1, size1);

                    match = regEx.Match(line2);

                    doc.BirthDate = parseDate(match.Groups[1].Value);

                    if (!checkValidity(line2, 0, 5, 6))
                        throw new Exception("Birth date check failed");

                    switch (match.Groups[3].Value.Trim(filler))
                    {
                        case "M":
                            doc.Gender = Gender.Male;
                            break;
                        case "F":
                            doc.Gender = Gender.Female;
                            break;
                    }

                    doc.ExpirationDate = parseDate(match.Groups[4].Value);

                    if (!checkValidity(line2, 8, 13, 14))
                        throw new Exception("Expiration date check failed");

                    if (doc.ExpirationDate < doc.BirthDate)
                        doc.ExpirationDate = doc.ExpirationDate.AddYears(100);

                    doc.Nationality = match.Groups[6].Value;
                    doc.OptionalData2 = match.Groups[7].Value.Trim(filler);

                    sum = 0;

                    computeWeightedSum(line1, 5, 29, 0);
                    computeWeightedSum(line2, 0, 6, 25);
                    computeWeightedSum(line2, 8, 14, 32);
                    computeWeightedSum(line2, 18, 28, 39);

                    if ((sum % 10) != mappedValues[line2[29]])
                        throw new Exception("upper and middle lines check failed");


                    regEx = new Regex($"([A-Z0-9{filler}]{{30}})");

                    match = regEx.Match(mrz.Substring(size1 * 2, size1));

                    setNames(doc, match.Groups[1].Value);
                    break;

                case MrzFormat.TD2:
                    regEx = new Regex($"([A|C|I][A-Z0-9{filler}]{{1}})([A-Z{filler}]{{3}})([A-Z0-9{filler}]{{31}})");

                    line1 = mrz.Substring(0, size2);

                    match = regEx.Match(line1);

                    doc.Type = match.Groups[1].Value.Trim(filler);
                    doc.CountryCode = match.Groups[2].Value;

                    setNames(doc, match.Groups[3].Value);

                    regEx = new Regex($"([A-Z0-9{filler}]{{9}})([0-9{filler}]{{1}})([A-Z{filler}]{{3}})([0-9]{{6}})([0-9]{{1}})([M|F|X|{filler}]{{1}})([0-9]{{6}})([0-9]{{1}})([A-Z0-9{filler}]{{7}})([0-9]{{1}})");

                    line2 = mrz.Substring(size2, size2);

                    match = regEx.Match(line2);

                    doc.Number = match.Groups[1].Value.Trim(filler);

                    if (doc.Number != string.Empty && !checkValidity(line2, 0, 8, 9))
                        throw new Exception("Document number check failed");

                    doc.Nationality = match.Groups[3].Value;

                    doc.BirthDate = parseDate(match.Groups[4].Value);

                    if (!checkValidity(line2, 13, 18, 19))
                        throw new Exception("Birth date check failed");

                    switch (match.Groups[6].Value.Trim(filler))
                    {
                        case "M":
                            doc.Gender = Gender.Male;
                            break;
                        case "F":
                            doc.Gender = Gender.Female;
                            break;
                    }

                    doc.ExpirationDate = parseDate(match.Groups[7].Value);

                    if (!checkValidity(line2, 21, 26, 27))
                        throw new Exception("Expiration date check failed");

                    if (doc.ExpirationDate < doc.BirthDate)
                        doc.ExpirationDate = doc.ExpirationDate.AddYears(100);

                    doc.OptionalData1 = match.Groups[9].Value.Trim(filler);

                    sum = 0;

                    computeWeightedSum(line2, 0, 9, 0);
                    computeWeightedSum(line2, 13, 19, 10);
                    computeWeightedSum(line2, 21, 34, 17);

                    if ((sum % 10) != mappedValues[line2[35]])
                        throw new Exception("upper and middle lines check failed");
                    break;

                case MrzFormat.TD3:
                    regEx = new Regex($"(P[A-Z0-9{filler}]{{1}})([A-Z{filler}]{{3}})([A-Z0-9{filler}]{{39}})");

                    line1 = mrz.Substring(0, size3);

                    match = regEx.Match(line1);

                    doc.Type = match.Groups[1].Value.Trim(filler);
                    doc.CountryCode = match.Groups[2].Value;

                    setNames(doc, match.Groups[3].Value);

                    regEx = new Regex($"([A-Z0-9{filler}]{{9}})([0-9]{{1}})([A-Z{filler}]{{3}})([0-9]{{6}})([0-9]{{1}})([M|F|X|{filler}]{{1}})([0-9]{{6}})([0-9]{{1}})([A-Z0-9{filler}]{{14}})([0-9{filler}]{{1}})([0-9]{{1}})");

                    line2 = mrz.Substring(size3, size3);

                    match = regEx.Match(line2);

                    doc.Number = match.Groups[1].Value.Trim(filler);

                    if (!checkValidity(line2, 0, 8, 9))
                        throw new Exception("Document number check failed");

                    doc.Nationality = match.Groups[3].Value;

                    doc.BirthDate = parseDate(match.Groups[4].Value);

                    if (!checkValidity(line2, 13, 18, 19))
                        throw new Exception("Birth date check failed");

                    switch (match.Groups[6].Value.Trim(filler))
                    {
                        case "M":
                            doc.Gender = Gender.Male;
                            break;
                        case "F":
                            doc.Gender = Gender.Female;
                            break;
                    }

                    doc.ExpirationDate = parseDate(match.Groups[7].Value);

                    if (doc.ExpirationDate < doc.BirthDate)
                        doc.ExpirationDate = doc.ExpirationDate.AddYears(100);

                    if (!checkValidity(line2, 21, 26, 27))
                        throw new Exception("Expiration date check failed");

                    doc.OptionalData1 = match.Groups[9].Value.Trim(filler);

                    if (doc.OptionalData1 != string.Empty && !checkValidity(line2, 28, 41, 42))
                        throw new Exception("Optional data 1 check failed");

                    sum = 0;

                    computeWeightedSum(line2, 0, 9, 0);
                    computeWeightedSum(line2, 13, 19, 10);
                    computeWeightedSum(line2, 21, 42, 17);

                    if ((sum % 10) != mappedValues[line2[43]])
                        throw new Exception("upper and middle lines check failed");
                    break;

                case MrzFormat.MRVA:
                    regEx = new Regex($"(V[A-Z0-9{filler}]{{1}})([A-Z{filler}]{{3}})([A-Z0-9{filler}]{{39}})");

                    line1 = mrz.Substring(0, size3);

                    match = regEx.Match(line1);

                    doc.Type = match.Groups[1].Value.Trim(filler);
                    doc.CountryCode = match.Groups[2].Value;

                    setNames(doc, match.Groups[3].Value);

                    regEx = new Regex($"([A-Z0-9{filler}]{{9}})([0-9]{{1}})([A-Z{filler}]{{3}})([0-9]{{6}})([0-9]{{1}})([M|F|X|{filler}]{{1}})([0-9]{{6}})([0-9]{{1}})([A-Z0-9{filler}]{{16}})");

                    line2 = mrz.Substring(size3, size3);

                    match = regEx.Match(line2);

                    doc.Number = match.Groups[1].Value.Trim(filler);

                    if (!checkValidity(line2, 0, 8, 9))
                        throw new Exception("Document number check failed");

                    doc.Nationality = match.Groups[3].Value;

                    doc.BirthDate = parseDate(match.Groups[4].Value);

                    if (!checkValidity(line2, 13, 18, 19))
                        throw new Exception("Birth date check failed");

                    switch (match.Groups[6].Value.Trim(filler))
                    {
                        case "M":
                            doc.Gender = Gender.Male;
                            break;
                        case "F":
                            doc.Gender = Gender.Female;
                            break;
                    }

                    doc.ExpirationDate = parseDate(match.Groups[7].Value);

                    if (!checkValidity(line2, 21, 26, 27))
                        throw new Exception("Expiration date check failed");

                    if (doc.ExpirationDate < doc.BirthDate)
                        doc.ExpirationDate = doc.ExpirationDate.AddYears(100);

                    doc.OptionalData1 = match.Groups[9].Value.Trim(filler);
                    break;

                case MrzFormat.MRVB:
                    regEx = new Regex($"(V[A-Z0-9{filler}]{{1}})([A-Z{filler}]{{3}})([A-Z0-9{filler}]{{31}})");

                    line1 = mrz.Substring(0, 36);

                    match = regEx.Match(line1);

                    doc.Type = match.Groups[1].Value.Trim(filler);
                    doc.CountryCode = match.Groups[2].Value;

                    setNames(doc, match.Groups[3].Value);

                    regEx = new Regex($"([A-Z0-9{filler}]{{9}})([0-9]{{1}})([A-Z{filler}]{{3}})([0-9]{{6}})([0-9]{{1}})([M|F|X|{filler}]{{1}})([0-9]{{6}})([0-9]{{1}})([A-Z0-9{filler}]{{8}})");

                    line2 = mrz.Substring(36, 36);

                    match = regEx.Match(line2);

                    doc.Number = match.Groups[1].Value.Trim(filler);

                    if (!checkValidity(line2, 0, 8, 9))
                        throw new Exception("Document number check failed");

                    doc.Nationality = match.Groups[3].Value;

                    doc.BirthDate = parseDate(match.Groups[4].Value);

                    if (!checkValidity(line2, 13, 18, 19))
                        throw new Exception("Birth date check failed");

                    switch (match.Groups[6].Value.Trim(filler))
                    {
                        case "M":
                            doc.Gender = Gender.Male;
                            break;
                        case "F":
                            doc.Gender = Gender.Female;
                            break;
                    }

                    doc.ExpirationDate = parseDate(match.Groups[7].Value);

                    if (!checkValidity(line2, 21, 26, 27))
                        throw new Exception("Expiration date check failed");

                    if (doc.ExpirationDate < doc.BirthDate)
                        doc.ExpirationDate = doc.ExpirationDate.AddYears(100);

                    doc.OptionalData1 = match.Groups[9].Value.Trim(filler);
                    break;
            }

            return doc;
        }
    }
}
