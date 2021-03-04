﻿using System;

using Codice.Client.BaseCommands.CheckIn.Progress;
using Codice.Client.Commands.CheckIn;
using Codice.CM.Common;
using PlasticGui;

namespace Codice.Developer
{
    internal class CheckinProgress
    {
        internal bool CancelPressed;

        internal CheckinProgress(WorkspaceInfo wkInfo, PlasticGUIClient guiClient)
        {
            mWkInfo = wkInfo;
            mGuiClient = guiClient;

            mGuiClient.Progress.CanCancelProgress = true;

            mProgressRender = new CheckinUploadProgressRender(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinProgressMultiThreadUploading),
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinProgressMultiThreadNumOfBlocks),
                PlasticLocalization.GetString(PlasticLocalization.Name.CheckinProgressUploadingFiles),
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinProgressUploadingFileData),
                PlasticLocalization.GetString(PlasticLocalization.Name.CheckinProgressOf));
        }

        internal void Refresh(CheckinStatus checkinStatus)
        {
            if (checkinStatus == null)
                return;

            var progress = mGuiClient.Progress;

            progress.ProgressHeader = checkinStatus.StatusString;

            if (checkinStatus.Status >= EnumCheckinStatus.eciConfirming)
                progress.CanCancelProgress = false;

            if (checkinStatus.Status == EnumCheckinStatus.eciCancelling)
                return;

            int nowTicks = Environment.TickCount;

            progress.TotalProgressMessage = mProgressRender.GetUploadSize(
                checkinStatus.TransferredSize, checkinStatus.TotalSize, nowTicks);

            progress.TotalProgressPercent = ((float)CheckinUploadProgressRender.CalculateProgress(
                checkinStatus.TransferredSize, checkinStatus.TotalSize)) / 100f;

            progress.ShowCurrentBlock = mProgressRender.
                NeedShowCurrentBlockForCheckinStatus(checkinStatus, nowTicks);

            string currentFileInfo = mProgressRender.GetCurrentFileInfo(
                checkinStatus.CurrentCheckinBlock, mWkInfo.ClientPath);

            progress.ProgressHeader = currentFileInfo;

            float fileProgressBarValue = ((float)CheckinUploadProgressRender.CalculateProgress(
                 checkinStatus.CurrentCheckinBlock.UploadedSize,
                 checkinStatus.CurrentCheckinBlock.BlockSize)) / 100f;

            progress.CurrentBlockProgressPercent = fileProgressBarValue;

            progress.CurrentBlockProgressMessage = mProgressRender.GetCurrentBlockUploadSize(
                 checkinStatus.CurrentCheckinBlock, nowTicks);
        }

        CheckinUploadProgressRender mProgressRender;
        PlasticGUIClient mGuiClient;
        WorkspaceInfo mWkInfo;
    }
}
