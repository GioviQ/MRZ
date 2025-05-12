using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MRZ.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestDocumentTD1_Success()
        {
            var doc = new Document(
@"I<UTOD231458907<<<<<<<<<<<<<<<
7408122F1204159UTO<<<<<<<<<<<6
ERIKSSON<<ANNA<MARIA<<<<<<<<<<");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.TD1, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "I", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "UTO", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "D23145890", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1974, 8, 12), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(2012, 4, 15), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "UTO", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.OptionalData2, expected: "", message: "Wrong optional data 2");
            Assert.AreEqual(actual: doc.Surname, expected: "ERIKSSON", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "ANNA MARIA", message: "Wrong name");

            doc = new Document(
@"I<PRT000024759<ZZ72<<<<<<<<<<<
8010100F2006017PRT<<<<<<<<<<<8
CARLOS<MONTEIRO<<AMELIA<VANESS");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.TD1, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "I", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "PRT", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "000024759", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "ZZ72", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1980, 10, 10), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(2020, 6, 1), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "PRT", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.OptionalData2, expected: "", message: "Wrong optional data 2");
            Assert.AreEqual(actual: doc.Surname, expected: "CARLOS MONTEIRO", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "AMELIA VANESS", message: "Wrong name");

            doc = new Document(
@"IPUSAC030049646<<10<30<B22<498
8101017M1911297USA<<0754052296
TRAVELER<<HAPPY<<<<<<<<<<<<<<<");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.TD1, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "IP", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "USA", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "C03004964", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "<<10<30<B22<498", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1981, 01, 01), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'M', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(2019, 11, 29), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "USA", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.OptionalData2, expected: "<<075405229", message: "Wrong optional data 2");
            Assert.AreEqual(actual: doc.Surname, expected: "TRAVELER", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "HAPPY", message: "Wrong name");

            // Taken from https://github.com/snifter/MRZCode.NET/commit/b6da286b16a1423f495615ecab6ec423c368e51c
            // Test for no expiry date (Moldavian ID Card for seniors)
            doc = new Document(
@"I<UTOD231458907<<<<<<<<<<<<<<<
7408122F<<<<<<0UTO<<<<<<<<<<<6
ERIKSSON<<ANNA<MARIA<<<<<<<<<<");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.TD1, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "I", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "UTO", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "D23145890", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1974, 8, 12), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: null, message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "UTO", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.OptionalData2, expected: "", message: "Wrong optional data 2");
            Assert.AreEqual(actual: doc.Surname, expected: "ERIKSSON", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "ANNA MARIA", message: "Wrong name");

            // Test ExpirationDate correction when compared to BirthDate
            doc = new Document(
@"I<PRT000024759<ZZ72<<<<<<<<<<<
8010100F7006012PRT<<<<<<<<<<<8
CARLOS<MONTEIRO<<AMELIA<VANESS");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.TD1, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "I", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "PRT", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "000024759", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "ZZ72", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1980, 10, 10), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(2070, 6, 1), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "PRT", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.OptionalData2, expected: "", message: "Wrong optional data 2");
            Assert.AreEqual(actual: doc.Surname, expected: "CARLOS MONTEIRO", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "AMELIA VANESS", message: "Wrong name");
        }

        [TestMethod]
        public void TestDocumentTD2_Success()
        {
            var doc = new Document(
@"I<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<
D231458907UTO7408122F1204159<<<<<<<6");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.TD2, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "I", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "UTO", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "D23145890", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1974, 8, 12), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(2012, 4, 15), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "UTO", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.Surname, expected: "ERIKSSON", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "ANNA MARIA", message: "Wrong name");
        }

        [TestMethod]
        public void TestDocumentTD2_Failure()
        {
            Assert.ThrowsException<FormatException>(() => new Document(
@"I<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<
D23145890<UTO7408122F1204159<<<<<<<6"), "'<' is not allowed as document number check digit");
        }

        [TestMethod]
        public void TestDocumentTD3_Success()
        {
            var doc = new Document(
@"P<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<
L898902C36UTO7408122F1204159ZE184226B<<<<<10");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.TD3, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "P", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "UTO", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "L898902C3", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "ZE184226B", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1974, 8, 12), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(2012, 4, 15), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "UTO", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.Surname, expected: "ERIKSSON", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "ANNA MARIA", message: "Wrong name");
        }

        [TestMethod]
        public void TestDocumentMRVA_Success()
        {
            var doc = new Document(
@"V<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<
L8988901C4XXX4009078F96121096ZE184226B<<<<<<");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.MRVA, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "V", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "UTO", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "L8988901C", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "6ZE184226B", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1940, 9, 7), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(1996, 12, 10), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "XXX", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.Surname, expected: "ERIKSSON", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "ANNA MARIA", message: "Wrong name");
        }

        [TestMethod]
        public void TestDocumentMRVB_Success()
        {
            var doc = new Document(
@"V<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<
L8988901C4XXX4009078F9612109<<<<<<<<");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.MRVB, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "V", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "UTO", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "L8988901C", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1940, 9, 7), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: new DateTime(1996, 12, 10), message: "Wrong expiration date");
            Assert.AreEqual(actual: doc.Nationality, expected: "XXX", message: "Wrong nationality country code");
            Assert.AreEqual(actual: doc.Surname, expected: "ERIKSSON", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "ANNA MARIA", message: "Wrong name");
        }

        [TestMethod]
        public void TestDocumentIDFRA_Success()
        {
            var doc = new Document(
@"IDFRALOISEAU<<<<<<<<<<<<<<<<<<<<<<<<
970675K002774HERVE<<DJAMEL<7303216M4");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.IDFRA, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "ID", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "FRA", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "970675K00277", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1973, 3, 21), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: null, message: "IDFRA does not contain an expiration date");
            Assert.AreEqual(actual: doc.Gender, expected: 'M', message: "Wrong gender");
            Assert.AreEqual(actual: doc.Nationality, expected: "", message: "IDFRA does not contain nationality information");
            Assert.AreEqual(actual: doc.Surname, expected: "LOISEAU", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "HERVE, DJAMEL", message: "Wrong name");

            doc = new Document(
@"IDFRADOUEL<<<<<<<<<<<<<<<<<<<<932013
0506932020438CHRISTIANE<<NI2906209F3");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.IDFRA, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "ID", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "FRA", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "050693202043", message: "Wrong document number");
            Assert.AreEqual(actual: doc.OptionalData1, expected: "932013", message: "Wrong optional data 1");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1929, 6, 20), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: null, message: "IDFRA does not contain an expiration date");
            Assert.AreEqual(actual: doc.Gender, expected: 'F', message: "Wrong gender");
            Assert.AreEqual(actual: doc.Nationality, expected: "", message: "IDFRA does not contain nationality information");
            Assert.AreEqual(actual: doc.Surname, expected: "DOUEL", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "CHRISTIANE, NI", message: "Wrong name");
        }

        [TestMethod]
        public void TestDocumentSDL_Success()
        {
            var doc = new Document(
@"CTD718D<<
FACHE123456788001<<800126<<<<<
MARCHAND<<FABIENNE<<<<<<<<<<<<");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.SDL, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "FA", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "CHE", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "123456788001", message: "Wrong document number");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1980, 1, 26), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: null, message: "SDL does not contain an expiration date");
            Assert.AreEqual(actual: doc.Gender, expected: default, message: "SDL does not contain gender information");
            Assert.AreEqual(actual: doc.Nationality, expected: "", message: "SDL does not contain nationality information");
            Assert.AreEqual(actual: doc.Surname, expected: "MARCHAND", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "FABIENNE", message: "Wrong name");

            doc = new Document(
@"ABC123D<<
FACHE123456789001<<410624<<<<<
HUBER<<PETER<FRANZ<XAVER<<<<<<");

            Assert.AreEqual(actual: doc.DocumentFormat, expected: MrzFormat.SDL, message: "Wrong document format");
            Assert.AreEqual(actual: doc.DocumentType, expected: "FA", message: "Wrong document type");
            Assert.AreEqual(actual: doc.IssuingState, expected: "CHE", message: "Wrong issuing state");
            Assert.AreEqual(actual: doc.DocumentNumber, expected: "123456789001", message: "Wrong document number");
            Assert.AreEqual(actual: doc.BirthDate, expected: new DateTime(1941, 6, 24), message: "Wrong birth date");
            Assert.AreEqual(actual: doc.ExpirationDate, expected: null, message: "SDL does not contain an expiration date");
            Assert.AreEqual(actual: doc.Gender, expected: default, message: "SDL does not contain gender information");
            Assert.AreEqual(actual: doc.Nationality, expected: "", message: "SDL does not contain nationality information");
            Assert.AreEqual(actual: doc.Surname, expected: "HUBER", message: "Wrong surname");
            Assert.AreEqual(actual: doc.Name, expected: "PETER FRANZ XAVER", message: "Wrong name");
        }

        [TestMethod]
        [DataRow(
@"I<UTOD231458907<<<<<<<<<<<<<<<
7408122F1204159UTO<<<<<<<<<<<6
ERIKSSON<<ANNA<MARIA<<<<<<<<<<", '|', MrzFormat.TD1)]
        [DataRow(
@"I<PRT000024759<ZZ72<<<<<<<<<<<
8010100F2006017PRT<<<<<<<<<<<8
CARLOS<MONTEIRO<<AMELIA<VANESS", '|', MrzFormat.TD1)]
        [DataRow(
@"IPUSAC030049646<<10<30<B22<498
8101017M1911297USA<<0754052296
TRAVELER<<HAPPY<<<<<<<<<<<<<<<", '|', MrzFormat.TD1)]
        [DataRow(
@"I<UTOD231458907<<<<<<<<<<<<<<<
7408122F<<<<<<0UTO<<<<<<<<<<<6
ERIKSSON<<ANNA<MARIA<<<<<<<<<<", '|', MrzFormat.TD1)]
        [DataRow(
@"I<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<
D231458907UTO7408122F1204159<<<<<<<6", '|', MrzFormat.TD2)]
        [DataRow(
@"P<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<
L898902C36UTO7408122F1204159ZE184226B<<<<<10", '|', MrzFormat.TD3)]
        [DataRow(
@"V<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<
L8988901C4XXX4009078F96121096ZE184226B<<<<<<", '|', MrzFormat.MRVA)]
        [DataRow(
@"V<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<
L8988901C4XXX4009078F9612109<<<<<<<<", '|', MrzFormat.MRVB)]
        [DataRow(
@"IDFRALOISEAU<<<<<<<<<<<<<<<<<<<<<<<<
970675K002774HERVE<<DJAMEL<7303216M4", '|', MrzFormat.IDFRA)]
        [DataRow(
@"IDFRADOUEL<<<<<<<<<<<<<<<<<<<<932013
0506932020438CHRISTIANE<<NI2906209F3", '|', MrzFormat.IDFRA)]
        [DataRow(
@"CTD718D<<
FACHE123456788001<<800126<<<<<
MARCHAND<<FABIENNE<<<<<<<<<<<<", '|', MrzFormat.SDL)]
        [DataRow(
@"ABC123D<<
FACHE123456789001<<410624<<<<<
HUBER<<PETER<FRANZ<XAVER<<<<<<", '|', MrzFormat.SDL)]
        public void TestAllPositionsThrow(string mrz, char illegalChar, MrzFormat expectedFormat)
        {
            foreach (var (wrongMrz, charPos) in CreateVariations(mrz, illegalChar))
            {
                var ex = Assert.ThrowsException<FormatException>(() => new Document(wrongMrz), $"'{illegalChar}' should not be allowed at position {charPos} in a {expectedFormat} document");

                MrzFormat correctedformat = expectedFormat switch
                {
                    MrzFormat.MRVA when charPos == 1 => MrzFormat.TD3,
                    MrzFormat.MRVB when charPos == 1 => MrzFormat.TD2,
                    MrzFormat.IDFRA when charPos <= 5 => MrzFormat.TD2,
                    _ => expectedFormat,
                };

                Assert.AreEqual($"Invalid {correctedformat} document format", ex.Message, "Thrown exception should be from an unsuccessful regex match");
            }
        }

        /// <summary>
        /// Generates an <see cref="IEnumerable{T}"/> which contains variations of the given <paramref name="mrz"/>
        /// where each variation has one character replaced with <paramref name="c"/>
        /// </summary>
        private static IEnumerable<(string wrongMrz, int charPos)> CreateVariations(string mrz, char c, int startIdx = 0)
        {
            char[] mrzArr = mrz.ToCharArray();

            int controlCount = 0;

            for (int i = startIdx; i < mrz.Length; i++)
            {
                if (char.IsControl(mrz[i]))
                {
                    controlCount++;
                    continue;
                }

                mrzArr[i] = c;
                yield return (new(mrzArr), i + 1 - controlCount);
                mrzArr[i] = mrz[i];
            }
        }
    }
}
