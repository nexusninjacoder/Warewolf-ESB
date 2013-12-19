using System.Collections.Concurrent;
using System.Threading;
using Dev2.DataList.Contract.Binary_Objects;

namespace Dev2.Data
{
    public class BackgroundDispatcher
    {
        // The Guid is the workspace ID of the writer
        private static ConcurrentQueue<IBinaryDataList> _binaryDataListQueue = new ConcurrentQueue<IBinaryDataList>();
        private static Thread _waitThread = new Thread(WriteLoop);
        private static ManualResetEventSlim _writeWaithandle = new ManualResetEventSlim(false);
        private static object _waitHandleGuard = new object();
        private static bool _shutdownRequested;

        #region Singleton Instance

        static BackgroundDispatcher _instance;
        public static BackgroundDispatcher Instance
        {
            get
            {
                return _instance ?? (_instance = new BackgroundDispatcher());
            }
        }

        #endregion

        #region Initialization

        static BackgroundDispatcher()
        {
            _waitThread.IsBackground = true;
            _waitThread.Start();
        }

        // Prevent instantiation
        BackgroundDispatcher()
        {

        }

        #endregion

        #region Properties


        #endregion

        #region Shutdown

        public void Shutdown()
        {
            _shutdownRequested = true;
            _writeWaithandle.Set(); // Maybe ?? Might cause more shit
        }

        #endregion Shutdown

        #region Write

        /// <summary>
        /// Writes the given state to the <see cref="IBinaryDataList" /> registered for the given workspace.
        /// <remarks>
        /// This must implement the one-way (fire and forget) message exchange pattern.
        /// </remarks>
        /// </summary>
        /// <param name="binaryDataList">The state to be written.</param>
        /// <returns>The task that was created.</returns>
        public void Add(IBinaryDataList binaryDataList)
        {
            if (binaryDataList != null)
            {
                lock (_waitHandleGuard)
                {
                    _binaryDataListQueue.Enqueue(binaryDataList);
                    _writeWaithandle.Set();
                }
            }
        }

        #endregion

        #region WriteLoop

        private static void WriteLoop()
        {
            while (true)
            {
                _writeWaithandle.Wait();

                if (_shutdownRequested)
                {
                    return;
                }

                IBinaryDataList binaryDataList;

                if (_binaryDataListQueue.TryDequeue(out binaryDataList))
                {
                    binaryDataList.Dispose();
                    binaryDataList = null;
                }

                lock (_waitHandleGuard)
                {
                    if (_binaryDataListQueue.Count == 0)
                    {
                        _writeWaithandle.Reset();
                    }
                }
            }
        }

        #endregion WriteLoop
    }
}