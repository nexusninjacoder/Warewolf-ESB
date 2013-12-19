﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Dev2.Common;
using Dev2.Common.Enums;
using Dev2.Data.Binary_Objects;
using Dev2.Data.Util;
using Dev2.DataList.Contract;
using Dev2.DataList.Contract.Binary_Objects;
using Dev2.DataList.Contract.TO;

namespace Dev2.Server.DataList.Translators
{
    internal sealed class StudioDebugTranslator : IDataListTranslator
    {
        private const string _rootTag = "DataList";
        private readonly DataListFormat _format;
        private readonly Encoding _encoding;


        public DataListFormat Format { get { return _format; } }
        public Encoding TextEncoding { get { return _encoding; } }

        public StudioDebugTranslator()
        {
            _format = DataListFormat.CreateFormat(GlobalConstants._Studio_Debug_XML);
            _encoding = Encoding.UTF8;
        }

        public DataListTranslatedPayloadTO ConvertFrom(IBinaryDataList payload, out ErrorResultTO errors)
        {
            if(payload == null)
            {
                throw new ArgumentNullException("payload");
            }

            StringBuilder result = new StringBuilder("<" + _rootTag + ">");
            errors = new ErrorResultTO();
            string error = string.Empty;

            IList<string> itemKeys = payload.FetchAllKeys();

            foreach(string key in itemKeys)
            {
                IBinaryDataListEntry entry = null;
                if(payload.TryGetEntry(key, out entry, out error))
                {

                    if(entry.IsRecordset)
                    {
                        int cnt = entry.FetchLastRecordsetIndex();
                        for(int i = 1; i <= cnt; i++)
                        {
                            IList<IBinaryDataListItem> rowData = entry.FetchRecordAt(i, out error);
                            errors.AddError(error);

                            result.Append("<");
                            result.Append(entry.Namespace);
                            result.Append(">");

                            foreach(IBinaryDataListItem col in rowData)
                            {
                                string fName = col.FieldName;

                                result.Append("<");
                                result.Append(fName);
                                result.Append(">");
                                result.Append(col.TheValue);
                                result.Append("</");
                                result.Append(fName);
                                result.Append(">");
                            }

                            result.Append("</");
                            result.Append(entry.Namespace);
                            result.Append(">");
                        }
                    }
                    else
                    {
                        string fName = entry.Namespace;
                        IBinaryDataListItem val = entry.FetchScalar();
                        if(val != null)
                        {
                            result.Append("<");
                            result.Append(fName);
                            result.Append(">");
                            result.Append(val.TheValue);
                            result.Append("</");
                            result.Append(fName);
                            result.Append(">");
                        }
                    }
                }

            }

            result.Append("</" + _rootTag + ">");

            DataListTranslatedPayloadTO tmp = new DataListTranslatedPayloadTO(result.ToString());

            return tmp;
        }

        public IBinaryDataList ConvertTo(byte[] input, string targetShape, out ErrorResultTO errors)
        {
            errors = new ErrorResultTO();
            string payload = Encoding.UTF8.GetString(input);
            string error = string.Empty;

            IBinaryDataList result = null;

            // build shape
            if(targetShape == null)
            {
                errors.AddError("Null payload or shape");
            }
            else
            {

                result = BuildTargetShape(targetShape, out error);
                errors.AddError(error);

                // populate the shape 
                if(payload != string.Empty)
                {
                    try
                    {
                        string toLoad = payload; // DataListUtil.StripCrap(payload); // clean up the rubish ;)
                        XmlDocument xDoc = new XmlDocument();
                        try
                        {
                            xDoc.LoadXml(toLoad);
                        }
                        catch
                        {
                            // Append new root tags ;)
                            toLoad = "<root>" + toLoad + "</root>";
                            xDoc.LoadXml(toLoad);
                        }

                        if(!string.IsNullOrEmpty(toLoad))
                        {
                            if(xDoc.DocumentElement != null)
                            {
                                XmlNodeList children = xDoc.DocumentElement.ChildNodes;

                                IDictionary<string, int> indexCache = new Dictionary<string, int>();

                                if(children != null)
                                {
                                    IBinaryDataListEntry entry = null;
                                    int idx = 1; // recset index

                                    // spin through each element in the XML
                                    foreach(XmlNode c in children)
                                    {
                                        var hasCorrectIoDirection = true;
                                        if(c.Attributes != null)
                                        {
                                            var columnIoDirectionAttribute = c.Attributes["ColumnIODirection"];
                                            if(columnIoDirectionAttribute != null)
                                            {
                                                var columnIoDirectionValue = columnIoDirectionAttribute.Value;
                                                var hasCorrectIoDirectionFromAttribute = columnIoDirectionValue == enDev2ColumnArgumentDirection.Output.ToString() || columnIoDirectionValue == enDev2ColumnArgumentDirection.Both.ToString();
                                                hasCorrectIoDirection = hasCorrectIoDirectionFromAttribute;
                                            }
                                        }

                                        if(DataListUtil.IsSystemTag(c.Name) && !hasCorrectIoDirection)
                                        {
                                            continue;
                                        }
                                        // scalars and recordset fetch
                                        if(result.TryGetEntry(c.Name, out entry, out error))
                                        {
                                            if(entry.IsRecordset)
                                            {
                                                // fetch recordset index
                                                int fetchIdx = 0;
                                                if(indexCache.TryGetValue(c.Name, out fetchIdx))
                                                {
                                                    idx = fetchIdx;
                                                }
                                                else
                                                {
                                                    idx = 1; //re-set idx on cache miss ;)
                                                }
                                                // process recordset
                                                XmlNodeList nl = c.ChildNodes;
                                                if(nl != null)
                                                {
                                                    foreach(XmlNode subc in nl)
                                                    {
                                                        entry.TryPutRecordItemAtIndex(Dev2BinaryDataListFactory.CreateBinaryItem(subc.InnerXml, c.Name, subc.Name, (idx + "")), idx, out error);
                                                        errors.AddError(error);
                                                    }
                                                    // update this recordset index
                                                    indexCache[c.Name] = ++idx;
                                                }
                                            }
                                            else
                                            {
                                                // process scalar
                                                entry.TryPutScalar(Dev2BinaryDataListFactory.CreateBinaryItem(c.InnerXml, c.Name), out error);
                                                errors.AddError(error);
                                            }
                                        }
                                        else
                                        {
                                            errors.AddError(error);
                                            entry = null;
                                        }
                                    }
                                }

                            }
                        }

                    }
                    catch(Exception e)
                    {
                        // if use passed in empty input they only wanted the shape ;)
                        if(input.Length > 0)
                        {
                            errors.AddError(e.Message);
                        }
                    }
                }
            }

            return result;

        }

        public IBinaryDataList ConvertTo(object input, string shape, out ErrorResultTO errors)
        {
            throw new NotImplementedException();
        }

        public Guid Populate(object input, Guid targetDL, string outputDefs, out ErrorResultTO errors)
        {
            throw new NotImplementedException();
        }

        public string ConvertAndFilter(IBinaryDataList payload, string filterShape, out ErrorResultTO errors)
        {
            if(payload == null)
            {
                throw new ArgumentNullException("payload");
            }

            StringBuilder result = new StringBuilder("<" + _rootTag + ">");
            errors = new ErrorResultTO();
            string error;

            IBinaryDataList targetDL = BuildTargetShape(filterShape, out error);

            IList<string> itemKeys = targetDL.FetchAllKeys();

            foreach(string key in itemKeys)
            {
                IBinaryDataListEntry entry = null;
                IBinaryDataListEntry tmpEntry = null;
                if(payload.TryGetEntry(key, out entry, out error) && targetDL.TryGetEntry(key, out tmpEntry, out error))
                {

                    if(entry.IsRecordset)
                    {
                        int cnt = entry.FetchLastRecordsetIndex();
                        for(int i = 1; i <= cnt; i++)
                        {
                            IList<IBinaryDataListItem> rowData = entry.FetchRecordAt(i, out error);
                            if(error != string.Empty)
                            {
                                errors.AddError(error);
                            }
                            result.Append("<");
                            result.Append(entry.Namespace);
                            result.Append(">");

                            foreach(IBinaryDataListItem col in rowData)
                            {
                                if(tmpEntry.Columns.Any((c => c.ColumnName == col.FieldName)))
                                {
                                    string fName = col.FieldName;

                                    result.Append("<");
                                    result.Append(fName);
                                    result.Append(">");
                                    result.Append(col.TheValue);
                                    result.Append("</");
                                    result.Append(fName);
                                    result.Append(">");
                                }
                            }


                            result.Append("</");
                            result.Append(entry.Namespace);
                            result.Append(">");
                        }
                    }
                    else
                    {
                        string fName = entry.Namespace;
                        IBinaryDataListItem val = entry.FetchScalar();
                        if(val != null)
                        {
                            result.Append("<");
                            result.Append(fName);
                            result.Append(">");
                            result.Append(val.TheValue);
                            result.Append("</");
                            result.Append(fName);
                            result.Append(">");
                        }
                    }
                }

            }

            result.Append("</" + _rootTag + ">");

            return result.ToString();
        }

        public DataTable ConvertToDataTable(IBinaryDataList input, string recsetName, out ErrorResultTO errors, PopulateOptions populateOptions)
        {
            errors = null;
            throw new NotImplementedException();
        }

        public DataListFormat HandlesType()
        {
            return _format;
        }


        #region Private Methods

        /// <summary>
        /// Build the template based upon the sent shape
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="error"></param>
        private IBinaryDataList BuildTargetShape(string shape, out string error, bool insertSysTags = true)
        {
            IBinaryDataList result = null;
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(shape);
                XmlNodeList children = xDoc.DocumentElement.ChildNodes;
                error = string.Empty;

                HashSet<string> procssesNamespaces = new HashSet<string>();

                if(children != null)
                {
                    result = Dev2BinaryDataListFactory.CreateDataList();

                    foreach(XmlNode c in children)
                    {
                        XmlAttribute descAttribute = null;
                        XmlAttribute columnIODirection = null;
                        if(!DataListUtil.IsSystemTag(c.Name))
                        {
                            if(c.HasChildNodes)
                            {
                                IList<Dev2Column> cols = new List<Dev2Column>();
                                //recordset
                                if(c.ChildNodes != null)
                                {
                                    // build template
                                    if(!procssesNamespaces.Contains(c.Name))
                                    {
                                        // build columns
                                        foreach(XmlNode subc in c.ChildNodes)
                                        {
                                            // It is possible for the .Attributes property to be null, a check should be added
                                            if(subc.Attributes != null)
                                            {
                                                descAttribute = subc.Attributes["Description"];
                                            }
                                            if(descAttribute != null)
                                            {
                                                cols.Add(DataListFactory.CreateDev2Column(subc.Name, descAttribute.Value));
                                            }
                                            else
                                            {
                                                cols.Add(DataListFactory.CreateDev2Column(subc.Name, string.Empty));
                                            }
                                        }
                                        string myError = string.Empty;
                                        // It is possible for the .Attributes property to be null, a check should be added
                                        if(c.Attributes != null)
                                        {
                                            descAttribute = c.Attributes["Description"];
                                            columnIODirection = c.Attributes["ColumnIODirection"];
                                        }
                                        string descriptionValue = "";
                                        var columnDirection = enDev2ColumnArgumentDirection.None;
                                        if(descAttribute != null)
                                        {
                                            descriptionValue = descAttribute.Value;
                                        }
                                        if(columnIODirection != null)
                                        {
                                            Enum.TryParse(columnIODirection.Value, true, out columnDirection);
                                        }
                                        if(!result.TryCreateRecordsetTemplate(c.Name, descriptionValue, cols, true, false, columnDirection, out myError))
                                        {
                                            error = myError;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //scalar
                                // It is possible for the .Attributes property to be null, a check should be added
                                if(c.Attributes != null)
                                {
                                    descAttribute = c.Attributes["Description"];
                                    columnIODirection = c.Attributes["ColumnIODirection"];
                                }
                                string descriptionValue = "";
                                var columnDirection = enDev2ColumnArgumentDirection.None;
                                if(descAttribute != null)
                                {
                                    descriptionValue = descAttribute.Value;
                                }
                                if(columnIODirection != null)
                                {
                                    Enum.TryParse(columnIODirection.Value, true, out columnDirection);
                                }
                                result.TryCreateScalarTemplate(string.Empty, c.Name, descriptionValue, true, true, columnDirection, out error);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                error = e.Message;
            }

            return result;
        }
        #endregion



    }
}