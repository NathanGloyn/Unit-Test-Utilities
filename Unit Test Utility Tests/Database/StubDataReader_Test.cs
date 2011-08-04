using System;
using NUnit.Framework;
using UnitTest.Database;

namespace BH.Utilities.UnitTest_Tests.Database
{
    /// <summary>
    ///This is a test class for StubDataReader_Test and is intended
    ///to contain all StubDataReader_Test Unit Tests
    ///</summary>
    [TestFixture]
    public class StubDataReader_Test
    {

        /// <summary>
        ///A test for IsDBNull
        ///</summary>
        [Test]
        public void IsDBNull_DBNull_Test()
        {
            StubResultSetCollection stubResultSets = new StubResultSetCollection();
            stubResultSets.Add(CreateIsDBNullResultSet());

            StubDataReader target = new StubDataReader(stubResultSets);
            target.Read();
            int i = 1; 
            bool expected = true; 
            bool actual;
            actual = target.IsDBNull(i);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for IsDBNull
        ///</summary>
        [Test]
        public void IsDBNull_Null_Test()
        {
            StubResultSetCollection stubResultSets = new StubResultSetCollection();
            stubResultSets.Add(CreateIsDBNullResultSet());

            StubDataReader target = new StubDataReader(stubResultSets);
            target.Read();
            int i = 0; 
            bool expected = true; 
            bool actual;
            actual = target.IsDBNull(i);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsDBNull
        ///</summary>
        [Test]
        public void IsDBNull_NotNull_Test()
        {
            StubResultSetCollection stubResultSets = new StubResultSetCollection();
            stubResultSets.Add(CreateIsDBNullResultSet());

            StubDataReader target = new StubDataReader(stubResultSets);
            target.Read();
            int i = 2; 
            bool expected = false;
            bool actual;
            actual = target.IsDBNull(i);
            Assert.AreEqual(expected, actual);
        }

        public StubResultSet CreateIsDBNullResultSet()
        {
            StubResultSet srs = new StubResultSet("IsNull", "IsDBNull", "NotNull");

            srs.AddRow(null, DBNull.Value, "abc");

            return srs;
        }
    }
}
