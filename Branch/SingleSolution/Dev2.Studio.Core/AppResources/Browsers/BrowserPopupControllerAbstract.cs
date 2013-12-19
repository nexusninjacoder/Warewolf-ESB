﻿using System;

namespace Dev2.Studio.Core.AppResources.Browsers
{
    // BUG 9798 - 2013.06.25 - TWR : modified to handle both internal and external
    public abstract class BrowserPopupControllerAbstract : IBrowserPopupController
    {
        public void ConfigurePopup()
        {
            var hwnd = FindPopup();
            if(hwnd != IntPtr.Zero)
            {
                SetPopupForeground(hwnd);
                SetPopupTitle(hwnd);
                SetPopupIcon(hwnd);
            }
        }

        public abstract bool ShowPopup(string url);

        public abstract IntPtr FindPopup();

        public abstract void SetPopupTitle(IntPtr hwnd);

        public abstract void SetPopupForeground(IntPtr hwnd);

        public abstract void SetPopupIcon(IntPtr hwnd);
    }
}