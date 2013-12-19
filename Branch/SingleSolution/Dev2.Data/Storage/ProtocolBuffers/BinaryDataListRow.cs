﻿using System;
using Dev2.Data.SystemTemplates;
using System.Text;
using System.Linq;

namespace Dev2.Data.Storage.ProtocolBuffers
{
    [Serializable]
    public class BinaryDataListRow : AProtocolBuffer 
    {
        private short _colCnt;
        
        private int _storageCapacity;
        private int _usedStorage;

        private char[] _rowData;

        // TODO : pack two int arrays into single uint array ;)
        // Will impact wide tables the most ;)
        private int[] _startIdx;
        private int[] _columnLen;

        public BinaryDataListRow() : this(1)
        {
        }

        public BinaryDataListRow(short columnCnt)
        {
            _colCnt = columnCnt;

            BuildRow(columnCnt);
        }

        public bool IsEmpty
        {
            get
            {
                return (_startIdx.ToList().All(idx => idx == DataListConstants.EmptyRowStartIdx));
            }
        }

        public bool IsDeferredRead(int idx)
        {
            return false;
        }

        #region ProtoBuf

        public override byte[] ToByteArray()
        {

            // get row data ;)
            byte[] rowBytes = Encoding.UTF8.GetBytes(_rowData);
            int rowBytesLen = rowBytes.Length;

            // calc len of object ;)
            var len = sizeof(int) + sizeof (short) + (sizeof (int)*2) + (rowBytesLen) + ((sizeof(int) * _colCnt)*2);
            byte[] result = new byte[len];

            var offSet = 0;

            // copy over _rowBytesLen for unpacking ;)
            Buffer.BlockCopy(BitConverter.GetBytes(rowBytesLen), 0, result, offSet, 4);

            // copy over _colCnt
            offSet += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(_colCnt),0,result,offSet,2);


            // copy over _storageCapacity
            offSet += 2;
            Buffer.BlockCopy(BitConverter.GetBytes(_storageCapacity), 0, result, offSet, 4);

            // copy over _usedStorage
            offSet += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(_usedStorage), 0, result, offSet, 4);

            // copy over _rowBytes
            offSet += 4;
            Buffer.BlockCopy(rowBytes, 0, result, offSet, rowBytesLen);

            // copy over _startIdx
            offSet += rowBytes.Length;
            for(int i = 0; i < _colCnt; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(_startIdx[i]), 0, result, offSet, sizeof(int));
                offSet += sizeof(int);
            }

            // copy over _columnLen
            for(int i = 0; i < _colCnt; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(_columnLen[i]), 0, result, offSet, sizeof(int));
                offSet += sizeof(int);
            }

            return result;
        }

        public override void ToObject(byte[] bytes)
        {
            
            var offSet = 0;

            // unpack _rowBytesLen
            var intBytes = new byte[sizeof(int)];
            Buffer.BlockCopy(bytes, offSet, intBytes, 0, 4);
            int packedRowBytesLen = BitConverter.ToInt32(intBytes, 0);

            // unpack _colCnt
            offSet += 4;
            var shortBytes = new byte[sizeof(short)];
            Buffer.BlockCopy(bytes, offSet, shortBytes, 0, 2);
            _colCnt = BitConverter.ToInt16(shortBytes, 0);

            // unpack _storageCapacity
            offSet += 2;
            Buffer.BlockCopy(bytes, offSet, intBytes, 0, 4);
            _storageCapacity = BitConverter.ToInt32(intBytes, 0);

            // unpack _usedStorage
            offSet += 4;
            Buffer.BlockCopy(bytes, offSet, intBytes, 0, 4);
            _usedStorage = BitConverter.ToInt32(intBytes, 0);

            // unpack _rowData
            var charSize = packedRowBytesLen;
            var charBytes = new byte[charSize];
            offSet += 4;
            Buffer.BlockCopy(bytes, offSet, charBytes, 0, charSize);
            _rowData = Encoding.UTF8.GetString(charBytes).ToCharArray();

            // unpack _startIdx
            var intSize = _colCnt*sizeof (int);
            var intArrayBytes = new byte[sizeof (int)];
            offSet += charSize;
            _startIdx = new int[_colCnt];

            // process a block at a time ;)
            int pos = 0;
            for (int i = 0; i < intSize; i += 4)
            {
                Buffer.BlockCopy(bytes, offSet, intArrayBytes, 0, sizeof (int));
                int val = BitConverter.ToInt32(intArrayBytes, 0);
                _startIdx[pos] = val;
                pos++;
                offSet += 4;
            }

            // unpack _columnLen
            pos = 0;
            _columnLen = new int[_colCnt];

            // process a block at a time ;)
            for (int i = 0; i < intSize; i += 4)
            {
                Buffer.BlockCopy(bytes, offSet, intArrayBytes, 0, sizeof (int));
                var val = BitConverter.ToInt32(intArrayBytes, 0);
                _columnLen[pos] = val;
                pos++;
                offSet += 4;
            }

        }

        #endregion

        private void AdjustStorage(int targetColumnCount)
        {
            if(targetColumnCount >= _colCnt)
            {
                // ensure capacity at the meta-data level ;)

                // NOTE : this is for the case when both the parent row and child row exist, 
                // in a federated key AND the parent row has a reduced inital view 
                // this causes upsert actions to result in overflow issues ;)

                var dif = targetColumnCount - _colCnt; 
                if (dif == 0)
                {
                    dif = 1; // add 1 to account for current pos we want, it is >= after all
                }
                var amt = _colCnt + dif;

                // adjust the start indexes
                Array.Resize<int>(ref _startIdx, amt);

                // adjust the column length data
                Array.Resize<int>(ref _columnLen, amt);

                // now init the new stuff ;)
                for(int i = _colCnt; i < amt; i++)
                {
                    _startIdx[i] = DataListConstants.EmptyRowStartIdx;
                    _columnLen[i] = DataListConstants.EmptyRowStartIdx;
                }

                _colCnt = (short)amt;
            }
        }

        public string FetchValue(int idx, int masterViewColumnCount)
        {
            // adjust for master and child view differences ;)
            AdjustStorage(masterViewColumnCount);

            if (idx < _startIdx.Length)
            {
                int start = _startIdx[idx];
                if (start > DataListConstants.EmptyRowStartIdx)
                {
                    // we have data cool beans ;)   
                    int len = _columnLen[idx];
                    if (len > 0)
                    {
                        StringBuilder result = new StringBuilder();
                        int end = (start + len);

                        for (int i = start; i < end; i++)
                        {
                            result.Append(_rowData[i]);
                        }

                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }

        public void UpdateValue(string val, int idx, int masterViewColumnCount)
        {
            if (val != string.Empty)
            {
                // adjust for master and child view differences ;)
                AdjustStorage(masterViewColumnCount);

                int start = _startIdx[idx];
                if (start > DataListConstants.EmptyRowStartIdx)
                {
                    // we have data, cool beans ;)   
                    int candiateLen = val.Length;
                    int len = _columnLen[idx];

                    // if candiate is larger then prev value, we need to append to end of storage ;)   
                    if (candiateLen > len)
                    {

                        // first clear the old storage location ;)
                        for (int i = start; i < len; i++)
                        {
                            _rowData[i] = '\0';
                        }

                        // do we have space
                        int requiredSize = (_usedStorage + candiateLen);
                        if (requiredSize >= _storageCapacity)
                        {
                            // Nope, grow array
                            GrowRow(requiredSize);
                        }

                        // else yes we have the space, but in either case we need to append to end ;)
                        start = FetchStorageStartIdx();
                        int pos = 0;
                        int iterationLen = (start + candiateLen);

                        for (int i = start; i < iterationLen; i++)
                        {
                            _rowData[i] = val[pos];
                            pos++;
                        }

                        // finally update the storage location data
                        _startIdx[idx] = start;
                        _columnLen[idx] = candiateLen;
                        _usedStorage += candiateLen;
                    }
                    else
                    {
                        // same size or small we can fit it into the same location

                        // first update data
                        int pos = 0;
                        int overWriteLen = (start + candiateLen);
                        for (int i = start; i < overWriteLen; i++)
                        {
                            _rowData[i] = val[pos];
                            pos++;
                        }

                        // finally update length array ;)
                        _columnLen[idx] = candiateLen;
                    }
                }
                else
                {
                    // It is a new value that needs to be inserted ;)

                    int canidateLen = val.Length;
                    int storageReq = _usedStorage + canidateLen;

                    if (storageReq >= _storageCapacity)
                    {
                        // sad panda, we need to grow the storage
                        GrowRow(storageReq);
                    }

                    // now we have space, it is time to save the value ;)
                    start = FetchStorageStartIdx();
                    CharEnumerator itr = val.GetEnumerator();
                    int iterateLen = (start + canidateLen);
                    for (int i = start; i < iterateLen; i++)
                    {
                        itr.MoveNext();
                        _rowData[i] = itr.Current;
                    }


                    itr.Dispose();

                    // finally, update the storage data
                    _startIdx[idx] = start;
                    _columnLen[idx] = canidateLen;
                    _usedStorage += canidateLen;

                }
            }
            else
            {
                // clear the existing storage requirements out ;)
                if (idx < _colCnt)
                {
                    _startIdx[idx] = 0;
                    _columnLen[idx] = 0;
                }
                // TODO : Blank the data too ;)
            }
        }

        public string FetchDeferredLocation(int idx)
        {
            return string.Empty;
        }

        #region Private Methods

        private void Init()
        {
            // init start indexes and length
            for (int i = 0; i < _colCnt; i++)
            {
                _startIdx[i] = DataListConstants.EmptyRowStartIdx;
                _columnLen[i] = DataListConstants.EmptyRowStartIdx;
            }
        }

        /// <summary>
        /// Grows the row data storage capacity
        /// </summary>
        /// <param name="targetSize">Size of the target.</param>
        private void GrowRow(int targetSize)
        {

            // TODO : Get fancy and reclaim storage ;)

            // grow it by a factor of 1.5 of the target size to avoid growing too often ;)
            int growthSize = (int)(targetSize * DataListConstants.RowGrowthFactor);

            char[] tmp = new char[growthSize];


            Array.Copy(_rowData, tmp, _usedStorage);

            _rowData = tmp;
            _storageCapacity = growthSize;
        }

        private int FetchStorageStartIdx()
        {
            if (_usedStorage == 0)
            {
                return 0;
            }

            return (_usedStorage);

        }

        /// <summary>
        /// Builds the row.
        /// </summary>
        /// <param name="columnCnt">The column count.</param>
        private void BuildRow(int columnCnt)
        {
            _startIdx = new int[columnCnt];
            _columnLen = new int[columnCnt];

            if(columnCnt < DataListConstants.MinRowSize)
            {
                _storageCapacity = DataListConstants.MinRowSize;
            }
            else
            {
                _storageCapacity = columnCnt;
            }

            _rowData = new char[_storageCapacity]; // est a single char per column

            _usedStorage = 0;

            Init();
        }

        #endregion

    }
}