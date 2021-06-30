using Microsoft.VisualStudio.TestTools.UnitTesting;
using MRZ;
using System;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestParserTD1()
        {
            var parser = new Parser();

            var doc = parser.DetectFields(
@"I<UTOD231458907<<<<<<<<<<<<<<<
7408122F1204159UTO<<<<<<<<<<<6
ERIKSSON<<ANNA<MARIA<<<<<<<<<<");

            Assert.AreEqual(doc.Format, MrzFormat.TD1, "Wrong document format");
            Assert.AreEqual(doc.Type, "I", "Wrong document type");
            Assert.AreEqual(doc.CountryCode, "UTO", "Wrong document country code");
            Assert.AreEqual(doc.Number, "D23145890", "Wrong document number");
            Assert.AreEqual(doc.OptionalData1, string.Empty, "Wrong document optional data 1");
            Assert.AreEqual(doc.BirthDate, new DateTime(1974, 8, 12), "Wrong birth date");
            Assert.AreEqual(doc.Gender, Gender.Female, "Wrong gender");
            Assert.AreEqual(doc.ExpirationDate, new DateTime(2012, 4, 15), "Wrong expiration date");
            Assert.AreEqual(doc.Nationality, "UTO", "Wrong nationality country code");
            Assert.AreEqual(doc.OptionalData2, string.Empty, "Wrong document optional data 2");
            Assert.AreEqual(doc.Surname, "ERIKSSON", "Wrong surname");
            Assert.AreEqual(doc.Name, "ANNA MARIA", "Wrong name");

            doc = parser.DetectFields(
@"I<PRT000024759<ZZ72<<<<<<<<<<<
8010100F2006017PRT<<<<<<<<<<<8
CARLOS<MONTEIRO<<AMELIA<VANESS");

            Assert.AreEqual(doc.Format, MrzFormat.TD1, "Wrong document format");
            Assert.AreEqual(doc.Type, "I", "Wrong document type");
            Assert.AreEqual(doc.CountryCode, "PRT", "Wrong document country code");
            Assert.AreEqual(doc.Number, "000024759", "Wrong document number");
            Assert.AreEqual(doc.OptionalData1, "ZZ72", "Wrong document optional data 1");
            Assert.AreEqual(doc.BirthDate, new DateTime(1980, 10, 10), "Wrong birth date");
            Assert.AreEqual(doc.Gender, Gender.Female, "Wrong gender");
            Assert.AreEqual(doc.ExpirationDate, new DateTime(2020, 6, 1), "Wrong expiration date");
            Assert.AreEqual(doc.Nationality, "PRT", "Wrong nationality country code");
            Assert.AreEqual(doc.OptionalData2, string.Empty, "Wrong document optional data 2");
            Assert.AreEqual(doc.Surname, "CARLOS MONTEIRO", "Wrong surname");
            Assert.AreEqual(doc.Name, "AMELIA VANESS", "Wrong name");

            doc = parser.DetectFields(
@"IPUSAC030049646<<10<30<B22<498
8101017M1911297USA<<0754052296
TRAVELER<<HAPPY<<<<<<<<<<<<<<<");

            Assert.AreEqual(doc.Format, MrzFormat.TD1, "Wrong document format");
            Assert.AreEqual(doc.Type, "IP", "Wrong document type");
            Assert.AreEqual(doc.CountryCode, "USA", "Wrong document country code");
            Assert.AreEqual(doc.Number, "C03004964", "Wrong document number");
            Assert.AreEqual(doc.OptionalData1, "10<30<B22<498", "Wrong document optional data 1");
            Assert.AreEqual(doc.BirthDate, new DateTime(1981, 01, 01), "Wrong birth date");
            Assert.AreEqual(doc.Gender, Gender.Male, "Wrong gender");
            Assert.AreEqual(doc.ExpirationDate, new DateTime(2019, 11, 29), "Wrong expiration date");
            Assert.AreEqual(doc.Nationality, "USA", "Wrong nationality country code");
            Assert.AreEqual(doc.OptionalData2, "075405229", "Wrong document optional data 2");
            Assert.AreEqual(doc.Surname, "TRAVELER", "Wrong surname");
            Assert.AreEqual(doc.Name, "HAPPY", "Wrong name");

        }

        [TestMethod]
        public void TestParserTD2()
        {
            var parser = new Parser();

            var doc = parser.DetectFields(
@"I<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<
D231458907UTO7408122F1204159<<<<<<<6");

            Assert.AreEqual(doc.Format, MrzFormat.TD2, "Wrong document format");
            Assert.AreEqual(doc.Type, "I", "Wrong document type");
            Assert.AreEqual(doc.CountryCode, "UTO", "Wrong document country code");
            Assert.AreEqual(doc.Number, "D23145890", "Wrong document number");
            Assert.AreEqual(doc.OptionalData1, string.Empty, "Wrong document optional data 1");
            Assert.AreEqual(doc.BirthDate, new DateTime(1974, 8, 12), "Wrong birth date");
            Assert.AreEqual(doc.Gender, Gender.Female, "Wrong gender");
            Assert.AreEqual(doc.ExpirationDate, new DateTime(2012, 4, 15), "Wrong expiration date");
            Assert.AreEqual(doc.Nationality, "UTO", "Wrong nationality country code");
            Assert.AreEqual(doc.Surname, "ERIKSSON", "Wrong surname");
            Assert.AreEqual(doc.Name, "ANNA MARIA", "Wrong name");
        }

        [TestMethod]
        public void TestParserTD3()
        {
            var parser = new Parser();

            var doc = parser.DetectFields(
@"P<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<
L898902C36UTO7408122F1204159ZE184226B<<<<<10");

            Assert.AreEqual(doc.Format, MrzFormat.TD3, "Wrong document format");
            Assert.AreEqual(doc.Type, "P", "Wrong document type");
            Assert.AreEqual(doc.CountryCode, "UTO", "Wrong document country code");
            Assert.AreEqual(doc.Number, "L898902C3", "Wrong document number");
            Assert.AreEqual(doc.OptionalData1, "ZE184226B", "Wrong document optional data 1");
            Assert.AreEqual(doc.BirthDate, new DateTime(1974, 8, 12), "Wrong birth date");
            Assert.AreEqual(doc.Gender, Gender.Female, "Wrong gender");
            Assert.AreEqual(doc.ExpirationDate, new DateTime(2012, 4, 15), "Wrong expiration date");
            Assert.AreEqual(doc.Nationality, "UTO", "Wrong nationality country code");
            Assert.AreEqual(doc.Surname, "ERIKSSON", "Wrong surname");
            Assert.AreEqual(doc.Name, "ANNA MARIA", "Wrong name");
        }

        [TestMethod]
        public void TestParserMRVA()
        {
            var parser = new Parser();

            var doc = parser.DetectFields(
@"V<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<
L8988901C4XXX4009078F96121096ZE184226B<<<<<<");

            Assert.AreEqual(doc.Format, MrzFormat.MRVA, "Wrong document format");
            Assert.AreEqual(doc.Type, "V", "Wrong document type");
            Assert.AreEqual(doc.CountryCode, "UTO", "Wrong document country code");
            Assert.AreEqual(doc.Number, "L8988901C", "Wrong document number");
            Assert.AreEqual(doc.OptionalData1, "6ZE184226B", "Wrong document optional data 1");
            Assert.AreEqual(doc.BirthDate, new DateTime(1940, 9, 7), "Wrong birth date");
            Assert.AreEqual(doc.Gender, Gender.Female, "Wrong gender");
            Assert.AreEqual(doc.ExpirationDate, new DateTime(1996, 12, 10), "Wrong expiration date");
            Assert.AreEqual(doc.Nationality, "XXX", "Wrong nationality country code");
            Assert.AreEqual(doc.Surname, "ERIKSSON", "Wrong surname");
            Assert.AreEqual(doc.Name, "ANNA MARIA", "Wrong name");
        }

        [TestMethod]
        public void TestParserMRVB()
        {
            var parser = new Parser();

            var doc = parser.DetectFields(
@"V<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<
L8988901C4XXX4009078F9612109<<<<<<<<");

            Assert.AreEqual(doc.Format, MrzFormat.MRVB, "Wrong document format");
            Assert.AreEqual(doc.Type, "V", "Wrong document type");
            Assert.AreEqual(doc.CountryCode, "UTO", "Wrong document country code");
            Assert.AreEqual(doc.Number, "L8988901C", "Wrong document number");
            Assert.AreEqual(doc.OptionalData1, string.Empty, "Wrong document optional data 1");
            Assert.AreEqual(doc.BirthDate, new DateTime(1940, 9, 7), "Wrong birth date");
            Assert.AreEqual(doc.Gender, Gender.Female, "Wrong gender");
            Assert.AreEqual(doc.ExpirationDate, new DateTime(1996, 12, 10), "Wrong expiration date");
            Assert.AreEqual(doc.Nationality, "XXX", "Wrong nationality country code");
            Assert.AreEqual(doc.Surname, "ERIKSSON", "Wrong surname");
            Assert.AreEqual(doc.Name, "ANNA MARIA", "Wrong name");
        }
    }
}
