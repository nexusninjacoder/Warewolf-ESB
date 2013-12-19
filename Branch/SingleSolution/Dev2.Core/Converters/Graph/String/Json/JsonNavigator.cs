﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using Dev2;
using Newtonsoft.Json.Linq;
using Unlimited.Framework.Converters.Graph.Interfaces;

namespace Unlimited.Framework.Converters.Graph.String.Json
{
    public class JsonNavigator : INavigator
    {
        #region Constructor

        public JsonNavigator(object data)
        {
            Data = JToken.Parse(data.ToString());
        }

        #endregion Constructor

        #region Properties

        public object Data { get; internal set; }

        #endregion Properties

        #region Methods

        public object SelectScalar(IPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            JsonPath jsonPath = path as JsonPath;

            if (jsonPath == null)
            {
                throw new Exception(string.Format("Path of type '{0}' expected, path of type '{1}' received.", typeof(JsonPath).ToString(), path.GetType().ToString()));
            }

            JToken currentData = Data as JToken;

            if (currentData == null)
            {
                throw new Exception(string.Format("Type of {0} was expected for data, type of {1} was found instead.", typeof(JToken).ToString(), Data.GetType().ToString()));
            }

            if (path.ActualPath == JsonPath.SeperatorSymbol)
            {
                //nothing to do here yet
            }
            else if (path.ActualPath == JsonPath.EnumerableSymbol + JsonPath.SeperatorSymbol)
            {
                IEnumerable enumerableData = currentData as IEnumerable;

                if (enumerableData == null)
                {
                    currentData = null;
                }
                else
                {
                    IEnumerator enumerator = enumerableData.GetEnumerator();
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        currentData = enumerator.Current as JToken;
                    }
                }
            }
            else
            {
                List<IPathSegment> pathSegments = jsonPath.GetSegements().ToList();
                int segmentIndex = 0;

                while (currentData != null && segmentIndex < pathSegments.Count)
                {
                    if (pathSegments[segmentIndex].IsEnumarable)
                    {
                        IEnumerable enumerableData = GetEnumerableValueForPathSegment(pathSegments[segmentIndex], currentData);

                        if (enumerableData == null)
                        {
                            currentData = null;
                        }
                        else
                        {
                            IEnumerator enumerator = enumerableData.GetEnumerator();
                            enumerator.Reset();
                            while (enumerator.MoveNext())
                            {
                                currentData = enumerator.Current as JToken;
                            }
                        }
                    }
                    else
                    {
                        currentData = GetScalarValueForPathSegement(pathSegments[segmentIndex], currentData);
                    }

                    segmentIndex++;
                }
            }

            string returnVal = "";

            if (currentData != null)
            {
                returnVal = currentData.ToString();
            }

            return returnVal;
        }

        public IEnumerable<object> SelectEnumerable(IPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            JsonPath jsonPath = path as JsonPath;

            if (jsonPath == null)
            {
                throw new Exception(string.Format("Path of type '{0}' expected, path of type '{1}' received.", typeof(JsonPath).ToString(), path.GetType().ToString()));
            }

            List<object> returnData;

            if (path.ActualPath == JsonPath.SeperatorSymbol)
            {
                returnData = new List<object> { Data };
            }
            else if (path.ActualPath == JsonPath.EnumerableSymbol + JsonPath.SeperatorSymbol)
            {
                IEnumerable enumerableData = Data as IEnumerable;
                returnData = new List<object>();

                if (enumerableData != null)
                {
                    IEnumerator enumerator = enumerableData.GetEnumerator();
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        returnData.Add(enumerator.Current);
                    }
                }
            }
            else
            {
                returnData = new List<object>(SelectEnumberable(jsonPath.GetSegements().ToList(), Data as JToken).Select(o => o.ToString()));
            }

            return returnData;
        }

        public Dictionary<IPath, IList<object>> SelectEnumerablesAsRelated(IList<IPath> paths)
        {
            //
            // Get valid paths
            //
            IList<IPath> validPaths = new List<IPath>(paths.OfType<JsonPath>().ToList());

            //
            // Setup results structure
            //
            Dictionary<IPath, IList<object>> results = new Dictionary<IPath, IList<object>>();
            BuildResultsStructure(validPaths, results);

            if (validPaths.Count == 1 && validPaths[0].ActualPath == JsonPath.SeperatorSymbol)
            {
                results[validPaths[0]].Add(Data);
            }
            else if (validPaths.Count == 1 && validPaths[0].ActualPath == JsonPath.EnumerableSymbol + JsonPath.SeperatorSymbol)
            {
                IEnumerable enumerableData = Data as IEnumerable;

                if (enumerableData != null)
                {
                    IEnumerator enumerator = enumerableData.GetEnumerator();
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        results[validPaths[0]].Add(enumerator.Current);
                    }
                }
            }
            else
            {
                //
                // Create the root node
                //
                IndexedPathSegmentTreeNode<string> rootIndexedValueTreeNode = new IndexedPathSegmentTreeNode<string>();
                rootIndexedValueTreeNode.CurrentValue = Data;

                //
                // Index the segments of all the paths, this is done so that they don't have to be
                // regenerated for every use.
                //
                Dictionary<IPath, List<IPathSegment>> indexedPathSegments = new Dictionary<IPath, List<IPathSegment>>();
                IndexPathSegments(validPaths, indexedPathSegments);

                do
                {
                    BuildIndexedTree(validPaths, indexedPathSegments, rootIndexedValueTreeNode);
                    WriteToResults(validPaths, indexedPathSegments, rootIndexedValueTreeNode, results);
                } while (EnumerateIndexedTree(rootIndexedValueTreeNode) > 0);
            }
            return results;
        }

        public void Dispose()
        {
            Data = null;
        }

        #endregion Methods

        #region Private Methods

        private IEnumerable<object> SelectEnumberable(IList<IPathSegment> pathSegments, JToken data)
        {
            List<object> returnData = new List<object>();
            JToken currentData = data;
            bool lastSegment = false;

            for (int i = 0; i < pathSegments.Count; i++)
            {
                IPathSegment pathSegment = pathSegments[i];
                lastSegment = (i == pathSegments.Count - 1);

                if (pathSegment.IsEnumarable)
                {
                    IEnumerable enumerableData = GetEnumerableValueForPathSegment(pathSegment, currentData);

                    if (enumerableData != null)
                    {
                        IEnumerator enumerator = enumerableData.GetEnumerator();
                        enumerator.Reset();

                        JToken testToken = enumerableData as JToken;

                        if (testToken.IsEnumerableOfPrimitives())
                        {
                            while (enumerator.MoveNext())
                            {
                                JToken currentToken = enumerator.Current as JToken;
                                if (currentData != null)
                                {
                                    returnData.Add(currentToken.ToString());
                                }
                            }
                        }
                        else
                        {
                            while (enumerator.MoveNext())
                            {
                                returnData.AddRange(SelectEnumberable(pathSegments.Skip(i + 1).ToList(), enumerator.Current as JToken));
                            }
                        }
                    }
                    else
                    {
                        currentData = null;
                    }

                    return returnData;
                }
                else
                {
                    currentData = GetScalarValueForPathSegement(pathSegment, currentData);

                    if (currentData != null && lastSegment)
                    {
                        returnData.Add(currentData.ToString());
                    }
                }
            }

            return returnData;
        }

        private void BuildResultsStructure(IList<IPath> paths, Dictionary<IPath, IList<object>> results)
        {
            foreach (IPath path in paths)
            {
                results.Add(path, new List<object>());
            }
        }

        private void IndexPathSegments(IList<IPath> paths, Dictionary<IPath, List<IPathSegment>> indexedPathSegments)
        {
            indexedPathSegments.Clear();

            foreach (IPath path in paths)
            {
                indexedPathSegments.Add(path, new List<IPathSegment>(path.GetSegements()));
            }
        }

        private void BuildIndexedTree(IList<IPath> paths, Dictionary<IPath, List<IPathSegment>> indexedPathSegments, IndexedPathSegmentTreeNode<string> rootIndexedValueTreeNode)
        {
            foreach (IPath path in paths)
            {
                IndexedPathSegmentTreeNode<string> IndexedPathSegmentTreeNode = rootIndexedValueTreeNode;
                int pathSegmentCount = 0;

                while (pathSegmentCount < indexedPathSegments[path].Count)
                {
                    IndexedPathSegmentTreeNode<string> tmpIndexedPathSegmentTreeNode;
                    IPathSegment pathSegment = indexedPathSegments[path][pathSegmentCount];
                    if (!IndexedPathSegmentTreeNode.TryGetValue(pathSegment.ActualSegment, out tmpIndexedPathSegmentTreeNode))
                    {
                        IndexedPathSegmentTreeNode<string> newIndexedPathSegmentTreeNode = CreatePathSegmentIndexedPathSegmentTreeNode(pathSegment, IndexedPathSegmentTreeNode);
                        IndexedPathSegmentTreeNode.Add(pathSegment.ActualSegment, newIndexedPathSegmentTreeNode);
                        IndexedPathSegmentTreeNode = newIndexedPathSegmentTreeNode;
                    }
                    else
                    {
                        IndexedPathSegmentTreeNode = tmpIndexedPathSegmentTreeNode;
                    }

                    pathSegmentCount++;
                }
            }
        }

        private void WriteToResults(IList<IPath> paths, Dictionary<IPath, List<IPathSegment>> indexedPathSegments, IndexedPathSegmentTreeNode<string> rootIndexedValueTreeNode, Dictionary<IPath, IList<object>> results)
        {
            foreach (IPath path in paths)
            {
                List<IPathSegment> indexedPathSegment = indexedPathSegments[path];
                List<string> complexKey = indexedPathSegment.Select(p => p.ActualSegment).ToList();
                IndexedPathSegmentTreeNode<string> IndexedPathSegmentTreeNode = rootIndexedValueTreeNode[complexKey];
                results[path].Add(IndexedPathSegmentTreeNode.CurrentValue.ToString());
            }
        }

        private long EnumerateIndexedTree(IndexedPathSegmentTreeNode<string> node)
        {
            long enumerationCount = 0;

            foreach (IndexedPathSegmentTreeNode<string> childNode in node.Values)
            {
                enumerationCount += EnumerateIndexedTree(childNode);
            }

            if (node.Enumerator != null && enumerationCount == 0)
            {
                node.EnumerationComplete = !node.Enumerator.MoveNext();
                if (node.EnumerationComplete)
                {
                    node.CurrentValue = string.Empty;
                }
                else
                {
                    node.CurrentValue = node.Enumerator.Current;
                    enumerationCount++;
                }

                node.Clear();
            }

            return enumerationCount;
        }

        private IndexedPathSegmentTreeNode<string> CreatePathSegmentIndexedPathSegmentTreeNode(IPathSegment pathSegment, IndexedPathSegmentTreeNode<string> parentNode)
        {
            IndexedPathSegmentTreeNode<string> newIndexedValueTreeNode = new IndexedPathSegmentTreeNode<string>();

            if (parentNode.EnumerationComplete)
            {
                newIndexedValueTreeNode.CurrentValue = string.Empty;
                newIndexedValueTreeNode.EnumerationComplete = true;
            }
            else
            {
                if (pathSegment.IsEnumarable)
                {
                    var data = parentNode.CurrentValue as JToken;
                    newIndexedValueTreeNode.EnumerableValue = GetEnumerableValueForPathSegment(pathSegment, data);
                    
                    if (newIndexedValueTreeNode.EnumerableValue == null)
                    {
                        newIndexedValueTreeNode.CurrentValue = string.Empty;
                        newIndexedValueTreeNode.EnumerationComplete = true;
                    }
                    else
                    {
                        bool isPrimitiveArray = false;
                        JObject jObject = data as JObject;
                        if(jObject != null)
                        {
                            JProperty property = jObject.Property(pathSegment.ActualSegment);
                            isPrimitiveArray = property.IsEnumerableOfPrimitives();
                        }

                        newIndexedValueTreeNode.Enumerator = newIndexedValueTreeNode.EnumerableValue.GetEnumerator();
                        newIndexedValueTreeNode.Enumerator.Reset();

                        if(isPrimitiveArray)
                        {
                            var valueBuilder = new StringBuilder();
                            while(newIndexedValueTreeNode.Enumerator.MoveNext())
                            {
                                valueBuilder.Append(newIndexedValueTreeNode.Enumerator.Current);
                                valueBuilder.Append(",");
                            }
                            newIndexedValueTreeNode.EnumerationComplete = true;
                            newIndexedValueTreeNode.CurrentValue = valueBuilder.ToString().TrimEnd(',');
                        }
                        else
                        {

                            if(!newIndexedValueTreeNode.Enumerator.MoveNext())
                            {
                                newIndexedValueTreeNode.CurrentValue = string.Empty;
                                newIndexedValueTreeNode.EnumerationComplete = true;
                            }
                            else
                            {
                                newIndexedValueTreeNode.CurrentValue = newIndexedValueTreeNode.Enumerator.Current;
                            }
                        }
                    }
                }
                else
                {
                    newIndexedValueTreeNode.CurrentValue = GetScalarValueForPathSegement(pathSegment, parentNode.CurrentValue as JToken);

                    if (newIndexedValueTreeNode.CurrentValue == null)
                    {
                        newIndexedValueTreeNode.CurrentValue = string.Empty;
                        newIndexedValueTreeNode.EnumerationComplete = true;
                    }
                }
            }

            return newIndexedValueTreeNode;
        }

        private JToken GetScalarValueForPathSegement(IPathSegment pathSegment, JToken data)
        {
            JObject jObject = data as JObject;

            JToken returnVal = null;
            if (jObject != null)
            {
                JProperty property = jObject.Property(pathSegment.ActualSegment);

                if (property != null)
                {
                    returnVal = property.Value;
                }
            }

            return returnVal;
        }

        private IEnumerable GetEnumerableValueForPathSegment(IPathSegment pathSegment, JToken data)
        {
            JObject jObject = data as JObject;
            
            IEnumerable returnVal = null;
            if (jObject != null)
            {
                JProperty property = jObject.Property(pathSegment.ActualSegment);
                
                if (property != null && property.IsEnumerable())
                {
                    returnVal = property.Value as JArray;
                }
            }

            return returnVal;
        }

        #endregion Private Methods
    }
}