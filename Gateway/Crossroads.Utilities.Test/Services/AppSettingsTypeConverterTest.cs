﻿using Crossroads.Utilities.Interfaces;
using Crossroads.Utilities.Services;
using Moq;
using NUnit.Framework;
using System.ComponentModel;
using System.Globalization;

namespace Crossroads.Utilities.Test.Services
{
    class AppSettingsTypeConverterTest
    {
        private AppSettingsTypeConverter fixture;
        private Mock<IConfigurationWrapper> configurationWrapper;
        private Mock<ITypeDescriptorContext> typeDescriptorContext;
        private CultureInfo cultureInfo;

        [SetUp]
        public void SetUp()
        {
            configurationWrapper = new Mock<IConfigurationWrapper>();
            fixture = new AppSettingsTypeConverter(configurationWrapper.Object);

            typeDescriptorContext = new Mock<ITypeDescriptorContext>();
            cultureInfo = CultureInfo.CurrentCulture;
        }

        [Test]
        public void testCanConvertFrom()
        {
            Assert.IsTrue(fixture.CanConvertFrom(typeof(string)));
            Assert.IsFalse(fixture.CanConvertFrom(typeof(int)));
        }

        [Test]
        public void testConvertFrom()
        {
            configurationWrapper.Setup(mocked => mocked.GetConfigValue("KeyToGet")).Returns("TheValue");

            var val = fixture.ConvertFrom(typeDescriptorContext.Object, cultureInfo, "KeyToGet");
            configurationWrapper.VerifyAll();

            Assert.IsNotNull(val);
            Assert.AreEqual("TheValue", val);
        }



    }
}
