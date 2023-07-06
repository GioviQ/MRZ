using System;

namespace MRZ
{
    public enum MrzFormat
    {
        TD1,
        TD2,
        TD3,
        MRVA,
        MRVB,
        IDFRA
    }

    public enum Gender
    {
        NotSpecified,
        Male,
        Female
    }
    public class Document
    {
        public MrzFormat Format { get; set; }
        public string Type { get; set; }
        public string CountryCode { get; set; }
        public string Number { get; set; }
        public string OptionalData1 { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Nationality { get; set; }
        public string OptionalData2 { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
