#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using System.Globalization;
using System.Numerics;
using System;

namespace DebugUtils.Unity.Tests
{
    public class NumericFormatterTests
    {
        [TestCase(arg1: "B", arg2: "0b101010_u8")]
        [TestCase(arg1: "D", arg2: "42_u8")]
        [TestCase(arg1: "X", arg2: "0x2A_u8")]
        public void TestByteRepr(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected, actual: ((byte)42).Repr(config: config));
        }

        [TestCase(arg1: "B", arg2: "-0b101010_i32")]
        [TestCase(arg1: "D", arg2: "-42_i32")]
        [TestCase(arg1: "X", arg2: "-0x2A_i32")]
        public void TestIntRepr(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected, actual: (-42).Repr(config: config));
        }

        [Test]
        public void TestBigIntRepr()
        {
            var config = ReprConfig.Configure()
                                   .Build();
            Assert.AreEqual(expected: "-42_n",
                actual: new BigInteger(value: -42).Repr(config: config));
        }

        // Floating Point Types
        [Test]
        public void TestFloatRepr_Exact()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "EX")
                                   .Build();
            Assert.AreEqual(expected: "3.1415927410125732421875E+000_f32", actual: Single
               .Parse(s: "3.1415926535")
               .Repr(config: config));
        }

        [Test]
        public void TestDoubleRepr_Round()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "F5")
                                   .Build();
            Assert.AreEqual(expected: "3.14159_f64", actual: Double.Parse(s: "3.1415926535")
               .Repr(config: config));
        }

        [Test]
        public void TestDecimalRepr_HexPower()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "HP")
                                   .Build();
            Assert.AreEqual(expected: "0x6582A536_0B143885_41B65F29p10-028_m",
                actual: 3.1415926535897932384626433832795m.Repr(config: config));
        }

        [Test]
        public void TestFloatRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "QuietNaN(0x400000)_f32", actual: Single.NaN.Repr());
            Assert.AreEqual(expected: "Infinity_f32", actual: Single.PositiveInfinity.Repr());
            Assert.AreEqual(expected: "-Infinity_f32", actual: Single.NegativeInfinity.Repr());
        }

        [Test]
        public void TestDoubleRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "QuietNaN(0x8000000000000)_f64", actual: Double.NaN.Repr());
            Assert.AreEqual(expected: "Infinity_f64", actual: Double.PositiveInfinity.Repr());
            Assert.AreEqual(expected: "-Infinity_f64", actual: Double.NegativeInfinity.Repr());
        }

        [TestCase(arg1: "D", arg2: "42")]
        [TestCase(arg1: "X", arg2: "0x2A")]
        [TestCase(arg1: "x", arg2: "0x2a")]
        [TestCase(arg1: "N0", arg2: "42")]
        [TestCase(arg1: "B", arg2: "0b101010")]
        [TestCase(arg1: "b", arg2: "0b101010")]
        public void TestIntFormatString_PositiveValues(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected + "_i32", actual: 42.Repr(config: config));
        }

        [TestCase(arg1: "D", arg2: "-42")]
        [TestCase(arg1: "X", arg2: "-0x2A")]
        [TestCase(arg1: "x", arg2: "-0x2a")]
        [TestCase(arg1: "B", arg2: "-0b101010")]
        [TestCase(arg1: "b", arg2: "-0b101010")]
        public void TestIntFormatString_NegativeValues(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected + "_i32", actual: (-42).Repr(config: config));
        }

        [TestCase(arg1: "D", arg2: "255_u8")]
        [TestCase(arg1: "X", arg2: "0xFF_u8")]
        [TestCase(arg1: "B", arg2: "0b11111111_u8")]
        public void TestIntFormatString_ByteValues(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected, actual: ((byte)255).Repr(config: config));
        }

        [TestCase(arg1: "D", arg2: "9223372036854775807_i64")]
        [TestCase(arg1: "X", arg2: "0x7FFFFFFFFFFFFFFF_i64")]
        public void TestIntFormatString_LongValues(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected, actual: Int64.MaxValue.Repr(config: config));
        }

        [Test]
        public void TestIntFormatString_DefaultBehavior()
        {
            // Default format string should use decimal
            var config = ReprConfig.Configure()
                                   .Build();
            Assert.AreEqual(expected: "42_i32", actual: 42.Repr(config: config));
        }

        [TestCase(arg1: "F2", arg2: "3.14_f32")]
        [TestCase(arg1: "E2", arg2: "3.14E+000_f32")]
        [TestCase(arg1: "G", arg2: "3.14159_f32")]
        [TestCase(arg1: "N2", arg2: "3.14_f32")]
        public void TestFloatFormatString_StandardFormats(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: format)
                                   .Build();
            var actual = 3.14159f.Repr(config: config);
            Assert.AreEqual(expected: expected, actual: actual);
        }

        [Test]
        public void TestFloatFormatString_ExactMode()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "EX")
                                   .Build();
            var result = 3.14159f.Repr(config: config);
            Assert.AreEqual(expected: "3.141590118408203125E+000_f32", actual: result);
        }

        [Test]
        public void TestFloatFormatString_HexPower()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "HP")
                                   .Build();
            var result = 3.14159f.Repr(config: config);
            Assert.AreEqual(expected: "0x1.921FA0p+001_f32", actual: result);
        }

        [TestCase(arg1: "F2", arg2: "3.14_f64")]
        [TestCase(arg1: "E5", arg2: "3.14159E+000_f64")]
        [TestCase(arg1: "EX",
            arg2: "3.14158999999999988261834005243144929409027099609375E+000_f64")] // EX should produce exact representation
        public void TestFloatFormatString_DoubleValues(string format, string exact)
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: format)
                                   .Build();
            var result = 3.14159.Repr(config: config);
            Assert.AreEqual(expected: exact, actual: result);
        }

        [Test]
        public void TestFloatFormatString_SpecialValues()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "F2")
                                   .Build();
            Assert.AreEqual(expected: "QuietNaN(0x400000)_f32",
                actual: Single.NaN.Repr(config: config));
            Assert.AreEqual(expected: "Infinity_f32",
                actual: Single.PositiveInfinity.Repr(config: config));
            Assert.AreEqual(expected: "-Infinity_f32",
                actual: Single.NegativeInfinity.Repr(config: config));
        }

        [Test]
        public void TestFloatFormatString_FallbackToString()
        {
            // Empty format string should fall back to toString behavior
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "")
                                   .Build();
            var result = 3.14159.Repr(config: config);
            Assert.AreEqual(expected: "3.14159_f64", actual: result);
        }

        [TestCase(arg1: "O", arg2: "0o52_i32")] // Octal with 0o prefix
        [TestCase(arg1: "Q", arg2: "0q222_i32")] // Quaternary with 0q prefix
        public void TestIntFormatString_NewCustomFormats(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected, actual: 42.Repr(config: config));
        }

        [TestCase(arg1: "O", arg2: "0o377_u8")] // Octal 255
        [TestCase(arg1: "Q", arg2: "0q3333_u8")] // Quaternary 255
        public void TestIntFormatString_NewCustomFormats_Byte(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            Assert.AreEqual(expected: expected, actual: ((byte)255).Repr(config: config));
        }

        [Test]
        public void TestFloatFormatString_HexPower_Detailed()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "HP")
                                   .Build();
            // Test positive number - should use IEEE 754 hex notation
            var result = 3.14159f.Repr(config: config);
            Assert.AreEqual(expected: "0x1.921FA0p+001_f32",
                actual: result); // Should have power notation
            // Test negative number - should have minus sign
            result = (-3.14159f).Repr(config: config);
            Assert.AreEqual(expected: "-0x1.921FA0p+001_f32", actual: result);
        }

        [Test]
        public void TestFloatFormatString_HexPower_SpecialValues()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "HP")
                                   .Build();
            // Special values should still work with HP format
            Assert.AreEqual(expected: "QuietNaN(0x400000)_f32",
                actual: Single.NaN.Repr(config: config));
            Assert.AreEqual(expected: "Infinity_f32",
                actual: Single.PositiveInfinity.Repr(config: config));
            Assert.AreEqual(expected: "-Infinity_f32",
                actual: Single.NegativeInfinity.Repr(config: config));
        }

        #region Edge Cases

        [Test]
        public void TestIntFormatString_WithPrefixes()
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: "X4")
                                   .Build();
            Assert.AreEqual(expected: "0x002A_i32", actual: 42.Repr(config: config));
        }

        [Test]
        public void TestIntFormatString_BinaryStartsWith()
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: "B8")
                                   .Build();
            var result = 42.Repr(config: config);
            Assert.AreEqual(expected: "0b00101010_i32",
                actual: result); // Should use C# binary formatter with prefix.
        }

        [TestCase(arg1: "X2", arg2: "0x2A_i32")]
        [TestCase(arg1: "x4", arg2: "0x002a_i32")]
        [TestCase(arg1: "B4", arg2: "0b101010_i32")]
        [TestCase(arg1: "b8", arg2: "0b00101010_i32")]
        public void TestIntFormatString_StartsWithHandling(string format, string expected)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            var result = 42.Repr(config: config);
            Assert.AreEqual(expected: expected,
                actual: result); // Should use C# binary formatter with prefix.
        }

        #endregion

        #region CultureInfo Tests

        [Test]
        public void TestCultureInfo_InvariantCulture()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "F2")
                                   .Culture(culture: CultureInfo.InvariantCulture)
                                   .Build();
            // Should use period as decimal separator regardless of system culture
            Assert.AreEqual(expected: "3.14_f32", actual: 3.14159f.Repr(config: config));
        }

        [Test]
        public void TestCultureInfo_GermanCulture()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "F2")
                                   .Culture(culture: new CultureInfo(name: "de-DE"))
                                   .Build();
            // German culture uses comma as decimal separator
            Assert.AreEqual(expected: "3,14_f32", actual: 3.14159f.Repr(config: config));
        }

        [Test]
        public void TestCultureInfo_NumberFormatting()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "N2")
                                   .Culture(culture: CultureInfo.InvariantCulture)
                                   .Build();
            // Number format with thousand separators
            Assert.AreEqual(expected: "1,234.57_f32", actual: 1234.5678f.Repr(config: config));
        }

        [Test]
        public void TestCultureInfo_DefaultBehavior()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "F2")
                                   .Culture(culture: null) // Should use current culture
                                   .Build();
            // Should not throw and produce some valid output
            var result = 3.14159f.Repr(config: config);
            StringAssert.Contains(expected: "3", actual: result);
            StringAssert.Contains(expected: "14", actual: result);
        }

        [Test]
        public void TestCultureInfo_CustomFormatsIgnoreCulture()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "EX")
                                   .Culture(culture: new CultureInfo(name: "de-DE"))
                                   .Build();
            // EX format should ignore culture and always use invariant formatting
            var result = 3.14159f.Repr(config: config);
            StringAssert.Contains(expected: ".", actual: result); // Should use period, not comma
            StringAssert.Contains(expected: "E+", actual: result);
        }

        [Test]
        public void TestCultureInfo_IntegerFormattingWithCulture()
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: "N0")
                                   .Culture(culture: CultureInfo.InvariantCulture)
                                   .Build();
            // Integer with thousand separators
            Assert.AreEqual(expected: "1,234_i32", actual: 1234.Repr(config: config));
        }

        #endregion

        #region Default Exact Format Tests (converted from ExactFormatTest)

        // Decimal Tests
        [Test]
        public void TestDecimal_Exact_Normal()
        {
            Assert.AreEqual(expected: "1.0E+000_m", actual: 1.0m.Repr());
            Assert.AreEqual(expected: "-1.0E+000_m", actual: (-1.0m).Repr());
            Assert.AreEqual(expected: "3.1415926535897932384626433833E+000_m",
                actual: 3.1415926535897932384626433833m.Repr());
            Assert.AreEqual(expected: "1.23456789E+028_m",
                actual: 12345678900000000000000000000m.Repr());
            Assert.AreEqual(expected: "1.0E-028_m",
                actual: 0.0000000000000000000000000001m.Repr());
            // Additional theory test values
            Assert.AreEqual(expected: "2.0E+000_m", actual: 2.0m.Repr());
            Assert.AreEqual(expected: "3.1415926535897932384626433833E+000_m",
                actual: 3.141592653589793238462643383279m.Repr());
            Assert.AreEqual(expected: "1.0E-001_m", actual: 0.1m.Repr());
            Assert.AreEqual(expected: "1.0E-028_m",
                actual: 0.0000000000000000000000000001m.Repr());
            Assert.AreEqual(expected: "7.9228162514264328797450928128E+028_m",
                actual: 79228162514264328797450928128.0m.Repr());
        }

        [Test]
        public void TestDecimal_Exact_Zero()
        {
            Assert.AreEqual(expected: "0.0E+000_m", actual: 0.0m.Repr());
            Assert.AreEqual(expected: "0.0E+000_m", actual: (-0.0m).Repr());
        }

        [Test]
        public void TestDecimal_Exact_MaxMin()
        {
            Assert.AreEqual(expected: "7.9228162514264337593543950335E+028_m",
                actual: Decimal.MaxValue.Repr());
            Assert.AreEqual(expected: "-7.9228162514264337593543950335E+028_m",
                actual: Decimal.MinValue.Repr());
        }

        // Float Tests
        [Test]
        public void TestFloat_Exact_Normal()
        {
            Assert.AreEqual(expected: "0.0E+000_f32", actual: 0.0f.Repr());
            Assert.AreEqual(expected: "-0.0E+000_f32", actual: (-0.0f).Repr());
            Assert.AreEqual(expected: "1.0E+000_f32", actual: 1.0f.Repr());
            Assert.AreEqual(expected: "-1.0E+000_f32", actual: (-1.0f).Repr());
            Assert.AreEqual(expected: "1.5E+000_f32", actual: 1.5f.Repr());
            Assert.AreEqual(expected: "2.5E+000_f32", actual: 2.5f.Repr());
            // Additional theory test values
            Assert.AreEqual(expected: "2.0E+000_f32", actual: 2.0f.Repr());
            Assert.AreEqual(expected: "3.141590118408203125E+000_f32", actual: 3.14159f.Repr());
            Assert.AreEqual(expected: "1.00000001490116119384765625E-001_f32",
                actual: 0.1f.Repr());
            Assert.AreEqual(expected: "1.00000001335143196001808973960578441619873046875E-010_f32",
                actual: 1e-10f.Repr());
            Assert.AreEqual(expected: "1.00000002004087734272E+020_f32", actual: 1e20f.Repr());
        }

        [Test]
        public void TestFloat_Exact_Subnormal()
        {
            Assert.AreEqual(
                expected:
                "1.40129846432481707092372958328991613128026194187651577175706828388979108268586060148663818836212158203125E-045_f32",
                actual: 1.401298E-45f.Repr());
            Assert.AreEqual(
                expected:
                "9.99994610111475958152591905227349949604220526961919185041279068749432712426283842432894743978977203369140625E-041_f32",
                actual: 1e-40f.Repr());
        }

        [Test]
        public void TestFloat_Exact_MaxMin()
        {
            Assert.AreEqual(expected: "3.4028234663852885981170418348451692544E+038_f32",
                actual: Single.MaxValue.Repr());
            Assert.AreEqual(expected: "-3.4028234663852885981170418348451692544E+038_f32",
                actual: Single.MinValue.Repr());
            Assert.AreEqual(
                expected:
                "1.40129846432481707092372958328991613128026194187651577175706828388979108268586060148663818836212158203125E-045_f32",
                actual: Single.Epsilon.Repr());
        }

        // Double Tests
        [Test]
        public void TestDouble_Exact_Normal()
        {
            Assert.AreEqual(expected: "0.0E+000_f64", actual: 0.0.Repr());
            Assert.AreEqual(expected: "-0.0E+000_f64", actual: (-0.0).Repr());
            Assert.AreEqual(expected: "1.0E+000_f64", actual: 1.0.Repr());
            Assert.AreEqual(expected: "-1.0E+000_f64", actual: (-1.0).Repr());
            Assert.AreEqual(expected: "1.5E+000_f64", actual: 1.5.Repr());
            Assert.AreEqual(expected: "2.5E+000_f64", actual: 2.5.Repr());
            // Additional theory test values
            Assert.AreEqual(expected: "2.0E+000_f64", actual: 2.0.Repr());
            Assert.AreEqual(
                expected: "3.141592653589790007373494518105871975421905517578125E+000_f64",
                actual: 3.14159265358979.Repr());
            Assert.AreEqual(
                expected: "1.000000000000000055511151231257827021181583404541015625E-001_f64",
                actual: 0.1.Repr());
            Assert.AreEqual(
                expected:
                "1.00000000000000001999189980260288361964776078853415942018260300593659569925554346761767628861329298958274607481091185079852827053974965402226843604196126360835628314127871794272492894246908066589163059300043457860230145025079449986855914338755579873208034769049845635890960693359375E-100_f64",
                actual: 1e-100.Repr());
            Assert.AreEqual(
                expected:
                "1.0000000000000000159028911097599180468360808563945281389781327557747838772170381060813469985856815104E+100_f64",
                actual: 1e100.Repr());
        }

        [Test]
        public void TestDouble_Exact_Subnormal()
        {
            Assert.AreEqual(
                expected:
                "4.940656458412465441765687928682213723650598026143247644255856825006755072702087518652998363616359923797965646954457177309266567103559397963987747960107818781263007131903114045278458171678489821036887186360569987307230500063874091535649843873124733972731696151400317153853980741262385655911710266585566867681870395603106249319452715914924553293054565444011274801297099995419319894090804165633245247571478690147267801593552386115501348035264934720193790268107107491703332226844753335720832431936092382893458368060106011506169809753078342277318329247904982524730776375927247874656084778203734469699533647017972677717585125660551199131504891101451037862738167250955837389733598993664809941164205702637090279242767544565229087538682506419718265533447265625E-324_f64",
                actual: 4.9406564584124654E-324.Repr());
            Assert.AreEqual(
                expected:
                "9.999888671826830054133752367652800576668810404913933231973854213813672267149025137753668687959512485767082469435821326873955531817604221479111201871258225213276326434971902827643599339477263397778659665193793654309834532129281161268155283999204461560808953010434241919400457020315068567565301579569187340188105680700687048622572297011807295865142440458678820197825330390728703465639787631241688381084672868858070030425350029497774728423376227873672231502648785563207544427133780751498964842238650982976359736953654567288487694940230564769292298397759684630055091384876749698303915591084358566671856101564376699700392294336955627042165899589336900634182050515934614876820804363177575320916352342137470725187361510200023673178293392993509769439697265625E-321_f64",
                actual: 1e-320.Repr());
        }

        [Test]
        public void TestDouble_Exact_MaxMin()
        {
            Assert.AreEqual(
                expected:
                "1.79769313486231570814527423731704356798070567525844996598917476803157260780028538760589558632766878171540458953514382464234321326889464182768467546703537516986049910576551282076245490090389328944075868508455133942304583236903222948165808559332123348274797826204144723168738177180919299881250404026184124858368E+308_f64",
                actual: Double.MaxValue.Repr());
            Assert.AreEqual(
                expected:
                "-1.79769313486231570814527423731704356798070567525844996598917476803157260780028538760589558632766878171540458953514382464234321326889464182768467546703537516986049910576551282076245490090389328944075868508455133942304583236903222948165808559332123348274797826204144723168738177180919299881250404026184124858368E+308_f64",
                actual: Double.MinValue.Repr());
            Assert.AreEqual(
                expected:
                "4.940656458412465441765687928682213723650598026143247644255856825006755072702087518652998363616359923797965646954457177309266567103559397963987747960107818781263007131903114045278458171678489821036887186360569987307230500063874091535649843873124733972731696151400317153853980741262385655911710266585566867681870395603106249319452715914924553293054565444011274801297099995419319894090804165633245247571478690147267801593552386115501348035264934720193790268107107491703332226844753335720832431936092382893458368060106011506169809753078342277318329247904982524730776375927247874656084778203734469699533647017972677717585125660551199131504891101451037862738167250955837389733598993664809941164205702637090279242767544565229087538682506419718265533447265625E-324_f64",
                actual: Double.Epsilon.Repr());
        }

        #if NET5_0_OR_GREATER
    // Half Tests
    [Fact]
    public void TestHalf_Exact_Normal()
    {
        Assert.Equal(expected: "0.0E+000_f16", actual: Half.Zero.Repr());
        Assert.Equal(expected: "-0.0E+000_f16", actual: Half.NegativeZero.Repr());
        Assert.Equal(expected: "1.0E+000_f16", actual: ((Half)1.0f).Repr());
        Assert.Equal(expected: "-1.0E+000_f16", actual: ((Half)(-1.0f)).Repr());
    }

    [Fact]
    public void TestHalf_Exact_Subnormal()
    {
        Assert.Equal(expected: "5.9604644775390625E-008_f16",
            actual: ((Half)5.9604645E-8f).Repr());
    }

    [Fact]
    public void TestHalf_Exact_MaxMin()
    {
        Assert.Equal(expected: "6.5504E+004_f16", actual: Half.MaxValue.Repr());
        Assert.Equal(expected: "-6.5504E+004_f16", actual: Half.MinValue.Repr());
        Assert.Equal(expected: "5.9604644775390625E-008_f16", actual: Half.Epsilon.Repr());
    }

    [Fact]
    public void TestHalf_Exact_WorstCaseScenarios()
    {
        Assert.Equal(expected: "1.22010707855224609375E-004_f16",
            actual: BitConverter.UInt16BitsToHalf(0x07FF)
                                .Repr());
        Assert.Equal(expected: "6.0975551605224609375E-005_f16",
            actual: BitConverter.UInt16BitsToHalf(0x03FF)
                                .Repr());
    }
        #endif
        // Bit Pattern Tests
        [Test]
        public void TestFloat_Exact_WorstCaseScenarios()
        {
            Assert.AreEqual(
                expected:
                "2.350988561514728583455765982071533026645717985517980855365926236850006129930346077117064851336181163787841796875E-038_f32",
                actual: BitConverter.UInt32BitsToSingle(bits: 0x00FF_FFFF)
                                    .Repr());
            Assert.AreEqual(
                expected:
                "1.175494210692441075487029444849287348827052428745893333857174530571588870475618904265502351336181163787841796875E-038_f32",
                actual: BitConverter.UInt32BitsToSingle(bits: 0x007F_FFFF)
                                    .Repr());
        }

        [Test]
        public void TestDouble_Exact_WorstCaseScenarios()
        {
            Assert.AreEqual(
                expected:
                "2.2250738585072008890245868760858598876504231122409594654935248025624400092282356951787758888037591552642309780950434312085877387158357291821993020294379224223559819827501242041788969571311791082261043971979604000454897391938079198936081525613113376149842043271751033627391549782731594143828136275113838604094249464942286316695429105080201815926642134996606517803095075913058719846423906068637102005108723282784678843631944515866135041223479014792369585208321597621066375401613736583044193603714778355306682834535634005074073040135602968046375918583163124224521599262546494300836851861719422417646455137135420132217031370496583210154654068035397417906022589503023501937519773030945763173210852507299305089761582519159720757232455434770912461317493580281734466552734375E-308_f64",
                actual: BitConverter.UInt64BitsToDouble(bits: 0x000F_FFFF_FFFF_FFFF)
                                    .Repr());
            Assert.AreEqual(
                expected:
                "4.4501477170144022721148195934182639518696390927032912960468522194496444440421538910330590478162701758282983178260792422137401728773891892910553144148156412434867599762821265346585071045737627442980259622449029037796981144446145705102663115100318287949527959668236039986479250965780342141637013812613333119898765515451440315261253813266652951306000184917766328660755595837392240989947807556594098101021612198814605258742579179000071675999344145086087205681577915435923018910334964869420614052182892431445797605163650903606514140377217442262561590244668525767372446430075513332450079650686719491377688478005309963967709758965844137894433796621993967316936280457084866613206797017728916080020698679408551343728867675409720757232455434770912461317493580281734466552734375E-308_f64",
                actual: BitConverter.UInt64BitsToDouble(bits: 0x001F_FFFF_FFFF_FFFF)
                                    .Repr());
        }

        #endregion

        #region Integer Formatting Tests (converted from IntegerFormattingTest)

        [Test]
        public void TestBinaryFormatting()
        {
            // Test positive values
            Assert.AreEqual(expected: "0b101010_i32", actual: 42.Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "B")
               .Build()));
            Assert.AreEqual(expected: "0b11111111_u8", actual: ((byte)255).Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "B")
               .Build()));
            Assert.AreEqual(expected: "0b0_i32", actual: 0.Repr(config: ReprConfig.Configure()
               .IntFormat(format: "B")
               .Build()));
            // Test negative values
            Assert.AreEqual(expected: "-0b101010_i32", actual: (-42).Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "B")
               .Build()));
        }

        [Test]
        public void TestOctalFormatting()
        {
            // Test positive values
            Assert.AreEqual(expected: "0o52_i32", actual: 42.Repr(config: ReprConfig.Configure()
               .IntFormat(format: "O")
               .Build()));
            Assert.AreEqual(expected: "0o377_u8", actual: ((byte)255).Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "O")
               .Build()));
            Assert.AreEqual(expected: "0o0_i32", actual: 0.Repr(config: ReprConfig.Configure()
               .IntFormat(format: "O")
               .Build()));
            // Test negative values
            Assert.AreEqual(expected: "-0o52_i32", actual: (-42).Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "O")
               .Build()));
        }

        [Test]
        public void TestQuaternaryFormatting()
        {
            // Test positive values
            Assert.AreEqual(expected: "0q222_i32", actual: 42.Repr(config: ReprConfig.Configure()
               .IntFormat(format: "Q")
               .Build()));
            Assert.AreEqual(expected: "0q3333_u8", actual: ((byte)255).Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "Q")
               .Build()));
            Assert.AreEqual(expected: "0q0_i32", actual: 0.Repr(config: ReprConfig.Configure()
               .IntFormat(format: "Q")
               .Build()));
            // Test negative values
            Assert.AreEqual(expected: "-0q222_i32", actual: (-42).Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "Q")
               .Build()));
        }

        #endregion
    }
}