﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MinistryPlatform.Translation.Services;

namespace MinistryPlatform.Translation.Test
{
    [TestFixture]
    public class TranslationTests
    {
        [Test]
        public void ShouldGetPageRecords()
        {
            var pageId = 455;
            var record = MinistryPlatform.Translation.Services.MinistryPlatform.GetMyPageRecords(pageId);
            Assert.IsNotNull(record);
            Assert.IsNotEmpty(record);
            Assert.AreEqual( "Tony", record.FirstOrDefault()["First_Name"].ToString());
        }

        [Test]
        public void ShouldGetNoPageRecords()
        {
            var pageId = 0;
            var record = MinistryPlatform.Translation.Services.MinistryPlatform.GetMyPageRecords(pageId);
            Assert.IsNull(record);
        }

        [Test]
        public void ShouldGetPageRecord()
        {
            var pageId = 292;
            var recordId = 618602;
            var record = MinistryPlatform.Translation.Services.MinistryPlatform.GetMyPageRecord(pageId, recordId);
            Assert.IsNotNull(record);
            Assert.IsNotEmpty(record);
            Assert.AreEqual("Andrew", record.FirstOrDefault()["First_Name"].ToString());
        }

        [Test]
        public void ShouldGetNoPageRecord()
        {
            var pageId = 292;
            var recordId = 0;
            var record = MinistryPlatform.Translation.Services.MinistryPlatform.GetMyPageRecord(pageId, recordId);
            Assert.IsNull(record);

        }
    }
}
