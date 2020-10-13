using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace OfacLocalDbSyncApp
{
    public class OFACXML
    {
        private DataSet dsOfacXml = new DataSet();
        public bool loadOfacXML()
        {
            bool toReturn_success = false;
            try
            {

                string xmlPath = "";
                
                xmlPath = ConfigurationManager.AppSettings["xmlPath"].ToString();
                XmlReader xmlFile = XmlReader.Create(xmlPath, new XmlReaderSettings());
                dsOfacXml.Reset();
                dsOfacXml.ReadXml(xmlFile);
                if (dsOfacXml.Tables.Count > 0)
                {
                    toReturn_success = true;
                }
            }
            catch (Exception ex)
            {
                toReturn_success = false;
            }
            finally
            {

            }
            return toReturn_success;
        }

        private bool poulateOfac()
        {
            bool toReturn_success = false;
            string dsTableName = "";
            try
            {
                // Retrieves XML from online, loads into local dataset
                loadOfacXML();
                
                if (dsOfacXml.Tables.Count > 0)
                {
                    // If there are tables in the DataSet, clear the DB tables for the new data.
                    clearOFACTables();

                    // Loop through DataSet tables and load to DB
                    for (int i = 0; i < dsOfacXml.Tables.Count - 1; i++)
                    {
                        dsTableName = dsOfacXml.Tables[i].TableName;
                        if ((dsTableName.Equals("sdnEntry")) ||
                                (dsTableName.Equals("id")) ||
                                (dsTableName.Equals("aka")) ||
                                (dsTableName.Equals("address")))
                        {
                            // TODO log dsTableName
                            // TODO log number of rows in table
                            using (SqlConnection ofacConn = new SqlConnection(ConfigurationManager.ConnectionStrings["OfacConnectionString"].ConnectionString))
                            {
                                // TODO replace with SPROCS
                                SqlCommand cmd = new SqlCommand("select * from OFAC_" + dsTableName + " where 1=2", ofacConn);
                                SqlDataAdapter da = new SqlDataAdapter(cmd);
                                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                                ofacConn.Open();
                                da.Update(dsOfacXml.Tables[dsTableName]);
                                ofacConn.Close();
                            }

                        }

                    }

                }

                toReturn_success = true;
            }
            catch (Exception ex)
            {
                toReturn_success = false;
            }
            finally
            {

            }

            return toReturn_success;
        }


        // Clears OFAC DB Tables of all data before loading from XML
        private bool clearOFACTables()
        {
            bool toReturn_success = false;
            try
            {
                using (SqlConnection ofacConn = new SqlConnection(ConfigurationManager.ConnectionStrings["OfacConnectionString"].ConnectionString))
                {
                    // TODO Replace with Sproc
                    SqlCommand cmd = new SqlCommand("DELETE FROM OFAC_sdnEntry DELETE FROM " +
                                                    "OFAC_id DELETE FROM OFAC_aka DELETE FROM " +
                                                    "OFAC_address", ofacConn);
                    cmd.ExecuteNonQuery();
                }

                toReturn_success = true;
            }
            catch (Exception ex)
            {
                toReturn_success = false;
            }
            finally
            {

            }
            return toReturn_success;
        }
    }
}
