# MRZ Parser

[![License](https://img.shields.io/github/license/GioviQ/MRZ?color=blue)](https://github.com/GioviQ/MRZ/blob/master/LICENSE)
[![NuGet version (MRZ)](https://img.shields.io/nuget/v/MRZ?color=blue)](https://www.nuget.org/packages/MRZ)

Machine Readable Zone parser for official travel documents, sizes TD1, TD2, TD3, MRVA, MRVB, French ID cards (1988 - 2021) and Swiss driving licenses

- TD1 (3 lines, each has 30 characters)
- TD2 (2 lines, each has 36 characters)
- TD3 (2 lines, each has 44 characters)
- MRVA (2 lines, each has 44 characters)
- MRVB (2 lines, each has 36 characters)
- IDFRA (2 lines, each has 36 characters)
- SDL (3 lines, first has 9 characters, second and third have 30 characters)

MRZ parser is built according to International Civil Aviation Organization specifications (ICAO 9303):

* [Specifications Common to all Machine Readable Travel Documents (MRTDs)](https://www.icao.int/publications/Documents/9303_p3_cons_en.pdf)
* [Specifications for TD1 Size Machine Readable Official Travel Documents (MROTDs)](https://www.icao.int/publications/Documents/9303_p5_cons_en.pdf)
* [Specifications for TD2 Size Machine Readable Official Travel Documents (MROTDs)](https://www.icao.int/publications/Documents/9303_p6_cons_en.pdf)
* [Specifications for Machine Readable Passports (MRPs) and other TD3 Size MRTDs](https://www.icao.int/publications/Documents/9303_p4_cons_en.pdf)
* [Specifications for Machine Readable Visas (MRV)](https://www.icao.int/publications/Documents/9303_p7_cons_en.pdf)
* [Translation of the Swiss driving licence in credit-card format (DLC)](https://www.eda.admin.ch/content/dam/countries/countries-content/australia/en/Translation-of-the-Swiss-driving-licence-in-creditcard-format_EN.pdf)
