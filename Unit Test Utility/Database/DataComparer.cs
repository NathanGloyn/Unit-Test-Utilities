using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using System.Data;

namespace UnitTest.Database
{
    public class DataComparer
    {
        /// <summary>
        /// Compares 2 data tables and ensures that the columns and data match
        /// </summary>
        /// <param name="expected">DataTable holding expected data</param>
        /// <param name="actual">DataTable holding actual data</param>
        public static void CheckDataIsAsExpected(DataTable expected, DataTable actual)
        {
            foreach (DataColumn column in expected.Columns)
            {
                for (int i = 0; i < actual.Rows.Count; i++)
                {
                    object expectedValue = expected.Rows[i][column.ColumnName];
                    object actualValue = actual.Rows[i][column.ColumnName];

                    if (column.DataType == typeof(string))
                    {
                        expectedValue = expectedValue.ToString().Trim();
                        actualValue = actualValue.ToString().Trim();
                    }

                    Assert.AreEqual(expectedValue,
                                   actualValue,
                                    string.Format("expected and actual don't match for {0}.\r\nExpected: {1}\r\nActual:{2}",
                                                   column.ColumnName,
                                                   expected.Rows[i][column.ColumnName],
                                                   actual.Rows[i][column.ColumnName]));
                }
            }
        }

        /// <summary>
        /// Compares the first table in a data set against an expected table of data
        /// </summary>
        /// <param name="expected">DataTable holding expected data</param>
        /// <param name="actualData">DataSet that holds table to be compared against</param>
        public static void CheckDataIsAsExpected(System.Data.DataTable expected, DataSet actualData)
        {
            Assert.IsTrue(actualData.Tables.Count == 1);

            Assert.AreEqual(expected.Rows.Count, actualData.Tables[0].Rows.Count);

            CheckDataIsAsExpected(expected, actualData.Tables[0]);

        }

        /// <summary>
        /// Compares an object to data extracted from the db
        /// </summary>
        /// <param name="toCompare">actual object to compare</param>
        /// <param name="expected">expected data to compare object against</param>
        /// <returns>List of any properties that don't match</returns>
        /// <remarks>The columns in the data row must be named the same as the properties on the object or the data will not be compared</remarks>
        public static List<string> CheckObjectMatchesData(object toCompare, DataRow expected)
        {

            var errors = new List<string>();

            //Reflection
            foreach (PropertyInfo property in toCompare.GetType().GetProperties())
            {
                if (expected.Table.Columns.Contains(property.Name))
                {
                    if (expected[property.Name].ToString().Trim() != property.GetValue(toCompare, null).ToString().Trim())
                    {
                        errors.Add(string.Format("{0} doesn't match\r\nExpected:{1}\r\nActual:{2}", property.Name, expected[property.Name].ToString(), property.GetValue(toCompare, null).ToString()));
                    }
                }

            }

            return errors;

        }

        /// <summary>
        /// Compares the first data table of 2 data sets to ensure they match
        /// </summary>
        /// <param name="firstDataSet">DataSet to compare</param>
        /// <param name="sortClause1">Clause to sort firstDataSet by</param>
        /// <param name="secondDataSet">DataSet to compare</param>
        /// <param name="sortClause2">Clause to sort secondDataSet by</param>
        /// <returns></returns>
        public static bool CompareDataSets(DataSet firstDataSet, string sortClause1, DataSet secondDataSet, string sortClause2)
        {
            bool datasetIdentical = true;

            if (firstDataSet.Tables[0].Rows.Count != secondDataSet.Tables[0].Rows.Count)
            {
                return false;
            }
            else
            {

                using (DataView firstView = new DataView(firstDataSet.Tables[0]))
                using (DataView secondView = new DataView(secondDataSet.Tables[0]))
                {
                    if (sortClause1 != string.Empty)
                        firstView.Sort = sortClause1;

                    if (sortClause2 != string.Empty)
                        secondView.Sort = sortClause2;

                    for (int i = 0; i < secondView.Table.Rows.Count; i++)
                        for (int j = 0; j < firstView.Table.Columns.Count; j++)
                            if (secondView[i][j].ToString().Trim() != firstView[i][j].ToString().Trim())
                            {
                                datasetIdentical = false;
                                break;
                            }
                }

            }

            return datasetIdentical;
        }
    }
}
