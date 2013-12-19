﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dev2.Common.ExtMethods;
using Dev2.DataList.Contract;
using Dev2.DataList.Contract.Binary_Objects;

namespace Dev2.DataList
{
    /// <summary>
    /// Class for the "Is Text" recordset search option 
    /// </summary>
    public class RsOpIsText : AbstractRecsetSearchValidation
    {
        public RsOpIsText()
        {

        }

        public override Func<IList<string>> BuildSearchExpression(IBinaryDataList scopingObj, IRecsetSearch to)
        {
            // Default to a null function result
            Func<IList<string>> result = () => { return null; };

            result = () => {
                ErrorResultTO err = new ErrorResultTO();
                IList<RecordSetSearchPayload> operationRange = GenerateInputRange(to, scopingObj, out err).Invoke();
                IList<string> fnResult = new List<string>();

                foreach (RecordSetSearchPayload p in operationRange) {

                    if (p.Payload.IsAlpha()) {
                        fnResult.Add(p.Index.ToString());
                    }
                    else
                    {
                        if(to.RequireAllFieldsToMatch)
                        {
                            return new List<string>();
                        }
                    }
                }

                return fnResult.Distinct().ToList();
            };


            return result;
        }

        public override string HandlesType()
        {
            return "Is Text";
        }
    }
}