﻿using System;
using System.Collections.Generic;
using System.Data;
using Dev2.Common;
using Dev2.Data.Decisions.Operations;
using Dev2.Data.SystemTemplates.Models;
using Dev2.Data.Util;
using Dev2.DataList.Contract;
using Dev2.DataList.Contract.Binary_Objects;
using Dev2.DataList.Contract.Value_Objects;

namespace Dev2.Data.Decision
{
    public class Dev2DataListDecisionHandler
    {
        private static readonly IDataListCompiler _compiler = DataListFactory.CreateDataListCompiler();
        private static Dev2DataListDecisionHandler _inst;

        public static Dev2DataListDecisionHandler Instance
        {
            get
            {
                if(_inst == null)
                {
                    _inst = new Dev2DataListDecisionHandler();
                }

                return _inst;
            }

        }

        /// <summary>
        /// Fetches the switch data.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="oldAmbientData">The old ambient data.</param>
        /// <returns></returns>
        public string FetchSwitchData(string variableName, IList<string> oldAmbientData)
        {
            Guid dlID = FetchDataListID(oldAmbientData);
            IBinaryDataListEntry tmp = EvaluateForSwitch(variableName, dlID);

            if(tmp != null)
            {
                if(tmp.IsRecordset)
                {
                    string error;
                    return tmp.TryFetchLastIndexedRecordsetUpsertPayload(out error).TheValue;
                }

                var scalar = tmp.FetchScalar();

                return scalar.TheValue;
            }

            return string.Empty;
        }

        // Guid dlID
        /// <summary>
        /// Executes the decision stack.
        /// </summary>
        /// <param name="decisionDataPayload">The decision data payload.</param>
        /// <param name="oldAmbientData">The old ambient data.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.InvalidExpressionException">Could not evaluate decision data - No decision function found for [  + typeOf + ]</exception>
        public bool ExecuteDecisionStack(string decisionDataPayload, IList<string> oldAmbientData)
        {

            // Evaluate decisionDataPayload through the EvaluateFunction ;)
            Guid dlID = FetchDataListID(oldAmbientData);
            if(dlID == GlobalConstants.NullDataListID) throw new InvalidExpressionException("Could not evaluate decision data - no DataList ID sent!");
            // Swap out ! with a new internal token to avoid nasty issues with 
            string newDecisionData = Dev2DecisionStack.FromVBPersitableModelToJSON(decisionDataPayload);

            var dds = EvaluateRegion(newDecisionData, dlID);

            ErrorResultTO errors = new ErrorResultTO();

            if(dds != null)
            {
                if(dlID != GlobalConstants.NullDataListID)
                {
                    try
                    {

                        if(dds.TheStack != null)
                        {

                            for(int i = 0; i < dds.TotalDecisions; i++)
                            {
                                Dev2Decision dd = dds.GetModelItem(i);
                                enDecisionType typeOf = dd.EvaluationFn;

                                // Treat Errors special
                                if(typeOf == enDecisionType.IsError || typeOf == enDecisionType.IsNotError)
                                {
                                    dd.Col1 = _compiler.EvaluateSystemEntry(dlID, enSystemTag.Dev2Error, out errors);
                                }

                                IDecisionOperation op = Dev2DecisionFactory.Instance().FetchDecisionFunction(typeOf);
                                if(op != null)
                                {
                                    try
                                    {
                                        bool result = op.Invoke(dds.GetModelItem(i).FetchColsAsArray());

                                        if(!result && dds.Mode == Dev2DecisionMode.AND)
                                        {
                                            // Naughty stuff, we have a false in AND mode... break
                                            return false;
                                        }

                                        if(result && dds.Mode == Dev2DecisionMode.OR)
                                        {
                                            return true;
                                        }
                                    }
                                    catch(Exception e)
                                    {
                                        // An error, push into the DL
                                        ErrorResultTO errorErrors;
                                        errors.AddError(e.Message);
                                        _compiler.UpsertSystemTag(dlID, enSystemTag.Dev2Error, errors.MakeDataListReady(), out errorErrors);

                                        return false;
                                    }
                                }
                                else
                                {
                                    throw new InvalidExpressionException("Could not evaluate decision data - No decision function found for [ " + typeOf + " ]");
                                }
                            }

                            // else we are in AND mode and all have passed ;)
                            if(dds.Mode == Dev2DecisionMode.AND)
                            {
                                return true;
                            }

                            //finally, it must be OR mode with no matches ;(
                            return false;
                        }

                        throw new InvalidExpressionException("Could not evaluate decision data - Invalid model data sent!");
                    }
                    catch
                    {
                        // all hell has broken loose... ;)
                        throw new InvalidExpressionException("Could not evaluate decision data - No model data sent!");
                    }
                }

                throw new InvalidExpressionException("Could not evaluate decision data - no DataList ID sent!");
            }

            throw new InvalidExpressionException("Could not populate decision model - DataList Errors!");
        }

        private IBinaryDataListEntry EvaluateForSwitch(string payload, Guid dlID)
        {
            ErrorResultTO errors;
            IBinaryDataListEntry tmp = _compiler.Evaluate(dlID, enActionType.User, payload, false, out errors);

            return tmp;
        }

        /// <summary>
        /// Evaluates the region.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="dlID">The dl ID.</param>
        /// <returns></returns>
        private Dev2DecisionStack EvaluateRegion(string payload, Guid dlID)
        {
            ErrorResultTO errors;
            if(payload.StartsWith("{\"TheStack\":[{") || payload.StartsWith("{'TheStack':[{"))
            {
                //2013.05.06: Ashley Lewis for PBI 9460 - handle recordsets with stars in their index by resolving them
                var dds = _compiler.ConvertFromJsonToModel<Dev2DecisionStack>(payload);

                if(dds.TheStack != null)
                {
                    var effectedCols = new bool[] { false, false, false };
                    //Find decisions that mention record sets with starred indexes
                    var invalidDecisions = new List<Dev2Decision>();
                    for(int i = 0; i < dds.TotalDecisions; i++)
                    {
                        Dev2Decision dd = dds.GetModelItem(i);

                        if(dd.Col1 != null && DataListUtil.GetRecordsetIndexType(dd.Col1) == enRecordsetIndexType.Star)
                        {
                            invalidDecisions.Add(dd);
                            effectedCols[0] = true;
                        }
                        else
                        {
                            dd.Col1 = GetValueForDecisionVariable(dlID, dd.Col1);
                        }

                        if(dd.Col2 != null && DataListUtil.GetRecordsetIndexType(dd.Col2) == enRecordsetIndexType.Star)
                        {
                            if(!effectedCols[0])
                            {
                                invalidDecisions.Add(dd);
                            }
                            effectedCols[1] = true;
                        }
                        else
                        {
                            dd.Col2 = GetValueForDecisionVariable(dlID, dd.Col2);
                        }

                        if(dd.Col3 != null && DataListUtil.GetRecordsetIndexType(dd.Col3) == enRecordsetIndexType.Star)
                        {
                            if(!effectedCols[0] && !effectedCols[1])
                            {
                                invalidDecisions.Add(dd);
                            }
                            effectedCols[2] = true;
                        }
                        else
                        {
                            dd.Col3 = GetValueForDecisionVariable(dlID, dd.Col3);
                        }
                    }
                    //Remove those record sets and replace them with a new decision for each resolved value
                    foreach(Dev2Decision decision in invalidDecisions)
                    {
                        dds = ResolveAllRecords(dlID, dds, decision, effectedCols, out errors);
                    }
                }

                return dds;
            }
            return null;
        }

        static string GetValueForDecisionVariable(Guid dlID, string decisionColumn)
        {
            if(!String.IsNullOrEmpty(decisionColumn))
            {
                IBinaryDataListItem binaryDataListItem = null;
                ErrorResultTO errors;
                IBinaryDataListEntry entry = _compiler.Evaluate(dlID, enActionType.User, decisionColumn, false, out errors);
                if(entry.IsRecordset)
                {
                    string error;
                    var indexType = DataListUtil.GetRecordsetIndexType(decisionColumn);
                    if(indexType == enRecordsetIndexType.Numeric)
                    {
                        var index = int.Parse(DataListUtil.ExtractIndexRegionFromRecordset(decisionColumn));
                        var columnName = DataListUtil.ExtractFieldNameFromValue(decisionColumn);

                        binaryDataListItem = entry.TryFetchRecordsetColumnAtIndex(columnName, index, out error);
                    }
                    else
                    {
                        binaryDataListItem = entry.TryFetchLastIndexedRecordsetUpsertPayload(out error);
                    }
                }
                else
                {
                    binaryDataListItem = entry.FetchScalar();
                }
                if(binaryDataListItem != null)
                {
                    var value = binaryDataListItem.TheValue;
                    return value;
                }
            }
            return null;
        }

        /// <summary>
        /// Fetches the stack value.
        /// </summary>
        /// <param name="stack">The stack.</param>
        /// <param name="stackIndex">Index of the stack.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        private string FetchStackValue(Dev2DecisionStack stack, int stackIndex, int columnIndex)
        {
            // if out of bounds return an empty string ;)
            if(stackIndex >= stack.TheStack.Count)
            {
                return string.Empty;
            }

            if(columnIndex == 1)
            {
                return stack.TheStack[stackIndex].Col1;
            }
            else if(columnIndex == 2)
            {
                return stack.TheStack[stackIndex].Col2;
            }

            return string.Empty;
        }

        Dev2DecisionStack ResolveAllRecords(Guid id, Dev2DecisionStack stack, Dev2Decision decision, bool[] effectedCols, out ErrorResultTO errors)
        {
            int stackIndex = stack.TheStack.IndexOf(decision);
            stack.TheStack.Remove(decision);
            errors = new ErrorResultTO();
            if(effectedCols[0])
            {
                IDev2IteratorCollection colItr = Dev2ValueObjectFactory.CreateIteratorCollection();
                IBinaryDataListEntry col1Entry = _compiler.Evaluate(id, enActionType.User, decision.Col1, false, out errors);
                IDev2DataListEvaluateIterator col1Iterator = Dev2ValueObjectFactory.CreateEvaluateIterator(col1Entry);
                colItr.AddIterator(col1Iterator);
                int reStackIndex = stackIndex;

                while(colItr.HasMoreData())
                {
                    var newDecision = new Dev2Decision();
                    newDecision.Col1 = colItr.FetchNextRow(col1Iterator).TheValue;
                    newDecision.Col2 = decision.Col2;
                    newDecision.Col3 = decision.Col3;
                    newDecision.EvaluationFn = decision.EvaluationFn;
                    stack.TheStack.Insert(reStackIndex++, newDecision);
                }
            }
            if(effectedCols[1])
            {
                IDev2IteratorCollection colItr = Dev2ValueObjectFactory.CreateIteratorCollection();
                IBinaryDataListEntry col2Entry = _compiler.Evaluate(id, enActionType.User, decision.Col2, false, out errors);
                IDev2DataListEvaluateIterator col2Iterator = Dev2ValueObjectFactory.CreateEvaluateIterator(col2Entry);
                colItr.AddIterator(col2Iterator);
                int reStackIndex = stackIndex;

                while(colItr.HasMoreData())
                {
                    var newDecision = new Dev2Decision();
                    newDecision.Col1 = FetchStackValue(stack, reStackIndex, 1);
                    newDecision.Col2 = colItr.FetchNextRow(col2Iterator).TheValue;
                    newDecision.Col3 = decision.Col3;
                    newDecision.EvaluationFn = decision.EvaluationFn;
                    if(effectedCols[0])
                    {
                        // ensure we have the correct indexing ;)
                        if(reStackIndex < stack.TheStack.Count)
                        {
                            stack.TheStack[reStackIndex++] = newDecision;
                        }
                        else
                        {
                            stack.TheStack.Insert(reStackIndex++, newDecision);
                        }
                    }
                    else
                    {
                        stack.TheStack.Insert(reStackIndex++, newDecision);
                    }
                }
            }
            if(effectedCols[2])
            {
                IDev2IteratorCollection colItr = Dev2ValueObjectFactory.CreateIteratorCollection();
                IBinaryDataListEntry col3Entry = _compiler.Evaluate(id, enActionType.User, decision.Col3, false, out errors);
                IDev2DataListEvaluateIterator col3Iterator = Dev2ValueObjectFactory.CreateEvaluateIterator(col3Entry);
                colItr.AddIterator(col3Iterator);
                int reStackIndex = stackIndex;

                while(colItr.HasMoreData())
                {
                    var newDecision = new Dev2Decision();
                    newDecision.Col1 = FetchStackValue(stack, reStackIndex, 1);
                    newDecision.Col2 = FetchStackValue(stack, reStackIndex, 2);
                    newDecision.Col3 = colItr.FetchNextRow(col3Iterator).TheValue;
                    newDecision.EvaluationFn = decision.EvaluationFn;
                    if(effectedCols[0] || effectedCols[1])
                    {
                        // ensure we have the correct indexing ;)
                        if(reStackIndex < stack.TheStack.Count)
                        {
                            stack.TheStack[reStackIndex++] = newDecision;
                        }
                        else
                        {
                            stack.TheStack.Insert(reStackIndex++, newDecision);
                        }
                    }
                    else
                    {
                        stack.TheStack.Insert(reStackIndex++, newDecision);
                    }
                }
            }
            return stack;
        }

        /// <summary>
        /// Fetches the data list ID.
        /// </summary>
        /// <param name="oldAmbientData">The old ambient data.</param>
        /// <returns></returns>
        public Guid FetchDataListID(IList<string> oldAmbientData)
        {
            Guid dlID = GlobalConstants.NullDataListID;
            if(oldAmbientData != null && oldAmbientData.Count == 1)
            {
                Guid.TryParse(oldAmbientData[0], out dlID);
            }

            return dlID;
        }
    }
}