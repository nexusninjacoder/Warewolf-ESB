﻿using System.Collections.Generic;
using Dev2.Composition;
using Dev2.Studio.AppResources.Exceptions;
using Dev2.Studio.AppResources.ExtensionMethods;
using Dev2.Studio.Core.Controller;
using Dev2.Studio.Core.Helpers;
using Dev2.Studio.Core.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using Dev2.Studio.Core.Utils;


namespace Dev2.Studio.Feedback.Actions
{
    [Export(typeof(IFeedbackAction))]
    [Export(typeof(IAsyncFeedbackAction))]
    public class RecorderFeedbackAction : IAsyncFeedbackAction
    {
        #region Class Members

        private string _outputPath;
        private Action<Exception> _onCompleted;

        #endregion Class Members

        #region ctor
        public RecorderFeedbackAction()
        {

            FeedBackInvoker = ImportService.GetExportValue<IFeedbackInvoker>();
            FeedBackRecorder = ImportService.GetExportValue<IFeedBackRecorder>();
            Popup = ImportService.GetExportValue<IPopupController>(); 
        }
        #endregion

        #region Properties

        public IPopupController Popup { get; private set; }

        public IFeedBackRecorder FeedBackRecorder { get; private set; }

        public IFeedbackInvoker FeedBackInvoker { get; private set; }

        public bool CanProvideFeedback
        {
            get
            {
                return FeedBackRecorder.IsRecorderAvailable;
            }
        }

        public int Priority
        {
            get { return 1; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Starts the feedback.
        /// </summary>
        public void StartFeedback()
        {
            TryStartFeedback(null);
        }

        public void StartFeedback(Action<Exception> onOncompleted)
        {
            Action<Exception> onOncompletedActual = onOncompleted ?? (e => { });
            TryStartFeedback(onOncompletedActual);
        }

        public void FinishFeedBack(IEnvironmentModel environmentModel = null)
        {
            TryFinishFeedback(environmentModel);
        }

        public void CancelFeedback()
        {
            TryCancelFeedback();
        }

        #endregion Methods

        #region Private Methods

        /// <summary>
        /// Tries to start feedback.
        /// </summary>
        private void TryStartFeedback(Action<Exception> onOncompleted)
        {
            _onCompleted = onOncompleted;
            Exception completedResult = null;

            try
            {
                _outputPath = GetOuputPath();
                MessageBoxResult result = Popup.Show("Your actions are now being recorded. When you are ready to send your feedback please click 'Stop recording feedback' in the top right corner.", "Recording In Progress", MessageBoxButton.OK, MessageBoxImage.Information);
                if (result == MessageBoxResult.None)
                {
                    if (onOncompleted != null)
                    {
                        onOncompleted(null);
                    }
                    return;
                }
                FeedBackRecorder.StartRecording(_outputPath);
            }
            catch (FeedbackRecordingInprogressException feedbackRecordingInprogressException)
            {
                MessageBoxResult result = Popup.Show("A recording session is already in progress, would you like to try end it?", "Recording In Progress", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    FeedBackRecorder.KillAllRecordingTasks();
                    TryStartFeedback(onOncompleted);
                }
                else
                {
                    completedResult = feedbackRecordingInprogressException;
                }
            }
            //2013.02.06: Ashley Lewis - Bug 8611
            catch (FeedbackRecordingProcessFailedToStartException feedbackRecordingProcessFailedToStartException)
            {
                MessageBoxResult result = Popup.Show("The recording session cannot start at this time, would you like to send a standard email feedback?", "Recording Not Started", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    new FeedbackInvoker().InvokeFeedback(Factory.FeedbackFactory.CreateEmailFeedbackAction(new Dictionary<string, string>(), ServerUtil.GetLocalhostServer()));
                    TryCancelFeedback();
                }
                else completedResult = feedbackRecordingProcessFailedToStartException;
            }
            catch (Exception e)
            {
                if (onOncompleted != null)
                {
                    completedResult = e;
                }
                else
                {
                    throw e;
                }
            }

            if (onOncompleted != null && completedResult != null)
            {
                onOncompleted(completedResult);
            }
        }

        /// <summary>
        /// Tries to finish feedback.
        /// </summary>
        private void TryFinishFeedback(IEnvironmentModel environmentModel = null)
        {
            try
            {
                FeedBackRecorder.StopRecording();
            }
            catch (FeedbackRecordingNoProcessesExcpetion feedbackRecordingNoProcessesExcpetion)
            {
                Popup.Show("A recorded feedback session was ended but wasn't running. Please try start another recorded feedback session.", "No Recorded Feedback Session", MessageBoxButton.OK, MessageBoxImage.Error);
                if (_onCompleted != null)
                {
                    _onCompleted(feedbackRecordingNoProcessesExcpetion);
                }
                return;
            }
            catch (FeedbackRecordingTimeoutException feedbackRecordingTimeoutException)
            {
                Popup.Show("A timeout occured waiting for the recorded feedback session to end. Please try finish the recorded feedback session again.", "Recorded Feedback Session Timeout", MessageBoxButton.OK, MessageBoxImage.Error);
                if (_onCompleted != null)
                {
                    _onCompleted(feedbackRecordingTimeoutException);
                }
                return;
            }
           
            var attachedFiles = new Dictionary<string, string>();
            attachedFiles.Add("RecordingLog", _outputPath);
            if(environmentModel != null)
            {
                attachedFiles.Add("ServerLog", environmentModel.ResourceRepository.GetServerLogTempPath(environmentModel));    
            }            
            attachedFiles.Add("StudioLog", FileHelper.GetStudioLogTempPath());
            IFeedbackAction emailFeedbackAction = new EmailFeedbackAction(attachedFiles, environmentModel);
            //ImportService.SatisfyImports(emailFeedbackAction);

            if (_onCompleted != null)
            {
                _onCompleted(null);
            }

            FeedBackInvoker.InvokeFeedback(emailFeedbackAction);
        }

        /// <summary>
        /// Tries to cancel feedback.
        /// </summary>
        private void TryCancelFeedback()
        {
            Exception completedResult = null;

            try
            {
                FeedBackRecorder.KillAllRecordingTasks();
            }
            catch (FeedbackRecordingTimeoutException feedbackRecordingTimeoutException)
            {
                Popup.Show("A timeout occured waiting for the recorded feedback session to end. Please try cancel the recorded feedback session again.", "Recorded Feedback Session Timeout", MessageBoxButton.OK, MessageBoxImage.Error);
                completedResult = feedbackRecordingTimeoutException;
            }

            if (_onCompleted != null)
            {
                _onCompleted(completedResult);
            }
        }

        /// <summary>
        /// Gets the ouput path.
        /// </summary>
        private string GetOuputPath()
        {
            string path = Path.Combine(new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                StringResources.App_Data_Directory,
                StringResources.Feedback_Recordings_Directory,
                Guid.NewGuid().ToString() + ".zip"
            });
            return path;
        }

        #endregion Private Methods
    }
}