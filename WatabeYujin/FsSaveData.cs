/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

using System.IO;
using UnityEngine;

public class FsSaveData : MonoBehaviour
{
    //private UnityEngine.UI.Text textComponent;
    private System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
    private nn.account.Uid userId;
    private const string mountName = "MySave";
    private const string fileName = "MySaveData";
    private string filePath;
    private nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();

    private nn.hid.NpadState npadState;
    private nn.hid.NpadId[] npadIds = { nn.hid.NpadId.Handheld, nn.hid.NpadId.No1 };
    private const int saveDataVersion = 1;
    private const int saveDataSize = 8;
    private int counter = 0;
    private int saveData = 0;
    private int loadData = 0;

    void Start()
    {
       // textComponent = GameObject.Find("/Canvas/Text").GetComponent<UnityEngine.UI.Text>();

        nn.account.Account.Initialize();
        nn.account.UserHandle userHandle = new nn.account.UserHandle();

        nn.account.Account.OpenPreselectedUser(ref userHandle);
        nn.account.Account.GetUserId(ref userId, userHandle);

        nn.Result result = nn.fs.SaveData.Mount(mountName, userId);
        result.abortUnlessSuccess();

        filePath = string.Format("{0}:/{1}", mountName, fileName);
        Load();

        nn.hid.Npad.Initialize();
        nn.hid.Npad.SetSupportedStyleSet(nn.hid.NpadStyle.Handheld | nn.hid.NpadStyle.JoyDual);
        nn.hid.Npad.SetSupportedIdType(npadIds);
        npadState = new nn.hid.NpadState();
    }

    void Update()
    {
        stringBuilder.Length = 0;

        if (counter - loadData >= 300)
        {
            Load();
            loadData = counter;
        }
        else if (counter - saveData >= 100)
        {
            Save();
            saveData = counter;
        }
        for (int i = 0; i < npadIds.Length; i++)
        {
            nn.hid.Npad.GetState(ref npadState, npadIds[i], nn.hid.Npad.GetStyleSet(npadIds[i]));
            if ((npadState.buttons & nn.hid.NpadButton.Y) != 0)
            {
                ResetSaveData();
            }
            else if ((npadState.buttons & nn.hid.NpadButton.B) != 0)
            {
                Load();
                loadData = counter;
            }
            else if ((npadState.buttons & nn.hid.NpadButton.A) != 0)
            {
                Save();
                saveData = counter;
            }
        }

        stringBuilder.AppendFormat("A:Save, B:Load, Y:Reset\nCounter: {0}\nSave data: {1}\nLoad data {2}",
            counter, saveData, loadData);
        counter++;

        //textComponent.text = stringBuilder.ToString();
    }

    void OnDestroy()
    {
        nn.fs.FileSystem.Unmount(mountName);
    }

    private void Save()
    {
        byte[] data;
        using (MemoryStream stream = new MemoryStream(saveDataSize))
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(saveDataVersion);
            writer.Write(counter);
            stream.Close();
            data = stream.GetBuffer();
            Debug.Assert(data.Length == saveDataSize);
        }

#if UNITY_SWITCH
        // Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

        nn.Result result = nn.fs.File.Delete(filePath);
        if (!nn.fs.FileSystem.ResultPathNotFound.Includes(result))
        {
            result.abortUnlessSuccess();
        }

        result = nn.fs.File.Create(filePath, saveDataSize);
        result.abortUnlessSuccess();

        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);
        result.abortUnlessSuccess();

        result = nn.fs.File.Write(fileHandle, 0, data, data.LongLength, nn.fs.WriteOption.Flush);
        result.abortUnlessSuccess();

        nn.fs.File.Close(fileHandle);
        result = nn.fs.SaveData.Commit(mountName);
        result.abortUnlessSuccess();

#if UNITY_SWITCH
        // Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
    }

    private void Load()
    {
        nn.fs.EntryType entryType = 0;
        nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
        if (nn.fs.FileSystem.ResultPathNotFound.Includes(result)) { return; }
        result.abortUnlessSuccess();

        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        result.abortUnlessSuccess();

        long fileSize = 0;
        result = nn.fs.File.GetSize(ref fileSize, fileHandle);
        result.abortUnlessSuccess();

        byte[] data = new byte[fileSize];
        result = nn.fs.File.Read(fileHandle, 0, data, fileSize);
        result.abortUnlessSuccess();

        nn.fs.File.Close(fileHandle);

        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(stream);
            int version = reader.ReadInt32();
            Debug.Assert(version == saveDataVersion); // Save data version up
            counter = reader.ReadInt32();
        }
    }

    private void ResetSaveData()
    {
        counter = 0;
        Save();
        saveData = counter;
    }
}
