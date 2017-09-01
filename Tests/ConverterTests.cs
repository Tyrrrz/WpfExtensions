using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Mocks;
using Tyrrrz.WpfExtensions.Converters;

namespace Tests
{
    [TestClass]
    public class ConverterTests
    {
        private readonly CultureInfo _cult = CultureInfo.InvariantCulture;

        [TestMethod]
        public void ArrayToStringConverterTest()
        {
            var conv = new ArrayToStringConverter();

            var a = new[] {"asd", "qwe", "zzz"};
            var b = new[] {1, 2, 3};

            var aForward = conv.Convert(a, null, ";", _cult) as string;
            var bForward = conv.Convert(b, null, null, _cult) as string;

            Assert.IsNotNull(aForward);
            Assert.AreEqual("asd;qwe;zzz", aForward);
            Assert.IsNotNull(bForward);
            Assert.AreEqual("1, 2, 3", bForward);

            var aBack = conv.ConvertBack(aForward, null, ";", _cult) as string[];
            var bBack = conv.ConvertBack(bForward, null, null, _cult) as string[];

            Assert.IsNotNull(aBack);
            CollectionAssert.AreEqual(a, aBack);
            Assert.IsNotNull(bBack);
            CollectionAssert.AreEqual(b, bBack.Select(int.Parse).ToArray());
        }

        [TestMethod]
        public void BoolAndMultiConverterTest()
        {
            var conv = new BoolAndMultiConverter();

            var a = new object[] {true, true, true};
            var b = new object[] {true, false, false};
            var c = new object[] {false, false, false};

            var aForward = (bool) conv.Convert(a, null, null, _cult);
            var bForward = (bool) conv.Convert(b, null, null, _cult);
            var cForward = (bool) conv.Convert(c, null, null, _cult);

            Assert.IsTrue(aForward);
            Assert.IsFalse(bForward);
            Assert.IsFalse(cForward);

            // Back conversion doesn't matter
        }

        [TestMethod]
        public void BoolOrMultiConverterTest()
        {
            var conv = new BoolOrMultiConverter();

            var a = new object[] {true, true, true};
            var b = new object[] {true, false, false};
            var c = new object[] {false, false, false};

            var aForward = (bool) conv.Convert(a, null, null, _cult);
            var bForward = (bool) conv.Convert(b, null, null, _cult);
            var cForward = (bool) conv.Convert(c, null, null, _cult);

            Assert.IsTrue(aForward);
            Assert.IsTrue(bForward);
            Assert.IsFalse(cForward);

            // Back conversion doesn't matter
        }

        [TestMethod]
        public void BoolXorMultiConverterTest()
        {
            var conv = new BoolXorMultiConverter();

            var a = new object[] {true, true, true};
            var b = new object[] {true, false, false};
            var c = new object[] {false, false, false};

            var aForward = (bool) conv.Convert(a, null, null, _cult);
            var bForward = (bool) conv.Convert(b, null, null, _cult);
            var cForward = (bool) conv.Convert(c, null, null, _cult);

            Assert.IsFalse(aForward);
            Assert.IsTrue(bForward);
            Assert.IsFalse(cForward);

            // Back conversion doesn't matter
        }

        [TestMethod]
        public void BoolToVisibilityConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new BoolToVisibilityConverter();

            var trueForward = (Visibility) conv.Convert(true, null, null, _cult);
            var falseForward = (Visibility) conv.Convert(false, null, null, _cult);
            var falseForwardCollapsed = (Visibility) conv.Convert(false, null, Visibility.Collapsed, _cult);

            Assert.AreEqual(Visibility.Visible, trueForward);
            Assert.AreEqual(Visibility.Hidden, falseForward);
            Assert.AreEqual(Visibility.Collapsed, falseForwardCollapsed);

            var trueBack = (bool) conv.ConvertBack(trueForward, null, null, _cult);
            var falseBack = (bool) conv.ConvertBack(falseForward, null, null, _cult);
            var falseBackCollapsed = (bool) conv.ConvertBack(falseForwardCollapsed, null, Visibility.Collapsed, _cult);

            Assert.IsTrue(trueBack);
            Assert.IsFalse(falseBack);
            Assert.IsFalse(falseBackCollapsed);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void ByteArrayToImageConverterTest()
        {
            var conv = new ByteArrayToImageConverter();

            var bytes =
                Convert.FromBase64String(
                    "R0lGODlhDwAPAKECAAAAzMzM/////wAAACwAAAAADwAPAAACIISPeQHsrZ5ModrLlN48CXF8m2iQ3YmmKqVlRtW4MLwWACH+H09wdGltaXplZCBieSBVbGVhZCBTbWFydFNhdmVyIQAAOw==");

            var bytesForward = conv.Convert(bytes, null, null, _cult) as ImageSource;

            Assert.IsNotNull(bytesForward);
            Assert.AreEqual(15, bytesForward.Height);
            Assert.AreEqual(15, bytesForward.Width);

            // Back conversion doesn't matter
        }

        [TestMethod]
        public void ByteArrayToStringConverterTest()
        {
            var conv = new ByteArrayToStringConverter();

            var bytes = Convert.FromBase64String("SGVsbG8gd29ybGQ=");

            var bytesForward = conv.Convert(bytes, null, null, _cult) as string;

            Assert.AreEqual("Hello world", bytesForward);

            var bytesBack = conv.ConvertBack(bytesForward, null, null, _cult) as byte[];

            CollectionAssert.AreEqual(bytes, bytesBack);
        }

        [TestMethod]
        public void ColorToBrushConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new ColorToBrushConverter();

            var color = Colors.Red;

            var colorForward = conv.Convert(color, null, null, _cult) as Brush;

            Assert.IsNotNull(colorForward);

            var colorBack = (Color) conv.ConvertBack(colorForward, null, null, _cult);

            Assert.AreEqual(color, colorBack);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void EnumToStringConverterTest()
        {
            var conv = new EnumToStringConverter();

            var a = MockEnum.Simple;
            var b = MockEnum.MultipleWordsInTheName;

            var aForward = conv.Convert(a, typeof(MockEnum), null, _cult) as string;
            var bForward = conv.Convert(b, typeof(MockEnum), null, _cult) as string;

            Assert.AreEqual("Simple", aForward);
            Assert.AreEqual("Multiple Words In The Name", bForward);

            var aBack = conv.ConvertBack(aForward, typeof(MockEnum), null, _cult);
            var bBack = conv.ConvertBack(bForward, typeof(MockEnum), null, _cult);

            Assert.AreEqual(a, aBack);
            Assert.AreEqual(b, bBack);
        }

        [TestMethod]
        public void InvertBoolConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new InvertBoolConverter();

            var trueForward = (bool) conv.Convert(true, null, null, _cult);
            var falseForward = (bool) conv.Convert(false, null, null, _cult);

            Assert.IsFalse(trueForward);
            Assert.IsTrue(falseForward);

            var trueBack = (bool) conv.ConvertBack(false, null, null, _cult);
            var falseBack = (bool) conv.ConvertBack(true, null, null, _cult);

            Assert.IsTrue(trueBack);
            Assert.IsFalse(falseBack);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void InvertBoolToVisibilityConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new InvertBoolToVisibilityConverter();

            var trueForward = (Visibility) conv.Convert(true, null, null, _cult);
            var trueForwardCollapsed = (Visibility) conv.Convert(true, null, Visibility.Collapsed, _cult);
            var falseForward = (Visibility) conv.Convert(false, null, null, _cult);

            Assert.AreEqual(Visibility.Hidden, trueForward);
            Assert.AreEqual(Visibility.Collapsed, trueForwardCollapsed);
            Assert.AreEqual(Visibility.Visible, falseForward);

            var trueBack = (bool) conv.ConvertBack(trueForward, null, null, _cult);
            var trueBackCollapsed = (bool) conv.ConvertBack(trueForwardCollapsed, null, Visibility.Collapsed, _cult);
            var falseBack = (bool) conv.ConvertBack(falseForward, null, null, _cult);

            Assert.IsTrue(trueBack);
            Assert.IsTrue(trueBackCollapsed);
            Assert.IsFalse(falseBack);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void TimeSpanToDaysConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new TimeSpanToDaysConverter();

            var ts = new TimeSpan(5, 0, 0, 0);

            var tsForward = (double) conv.Convert(ts, null, null, _cult);

            Assert.AreEqual(5, tsForward, 10e-10);

            var tsBack = (TimeSpan) conv.ConvertBack(tsForward, null, null, _cult);

            Assert.AreEqual(ts, tsBack);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void TimeSpanToHoursConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new TimeSpanToHoursConverter();

            var ts = new TimeSpan(5, 0, 0, 0);

            var tsForward = (double) conv.Convert(ts, null, null, _cult);

            Assert.AreEqual(5 * 24, tsForward, 10e-10);

            var tsBack = (TimeSpan) conv.ConvertBack(tsForward, null, null, _cult);

            Assert.AreEqual(ts, tsBack);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void TimeSpanToMinutesConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new TimeSpanToMinutesConverter();

            var ts = new TimeSpan(5, 0, 0, 0);

            var tsForward = (double) conv.Convert(ts, null, null, _cult);

            Assert.AreEqual(5 * 24 * 60, tsForward, 10e-10);

            var tsBack = (TimeSpan) conv.ConvertBack(tsForward, null, null, _cult);

            Assert.AreEqual(ts, tsBack);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void TimeSpanToSecondsConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new TimeSpanToSecondsConverter();

            var ts = new TimeSpan(5, 0, 0, 0);

            var tsForward = (double) conv.Convert(ts, null, null, _cult);

            Assert.AreEqual(5 * 24 * 60 * 60, tsForward, 10e-10);

            var tsBack = (TimeSpan) conv.ConvertBack(tsForward, null, null, _cult);

            Assert.AreEqual(ts, tsBack);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void TimeSpanToMillisecondsConverterTest()
        {
            // ReSharper disable PossibleNullReferenceException
            var conv = new TimeSpanToMillisecondsConverter();

            var ts = new TimeSpan(5, 0, 0, 0);

            var tsForward = (double) conv.Convert(ts, null, null, _cult);

            Assert.AreEqual(5 * 24 * 60 * 60 * 1000, tsForward, 10e-10);

            var tsBack = (TimeSpan) conv.ConvertBack(tsForward, null, null, _cult);

            Assert.AreEqual(ts, tsBack);
            // ReSharper restore PossibleNullReferenceException
        }
    }
}