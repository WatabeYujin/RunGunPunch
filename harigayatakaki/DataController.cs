
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DataController : MonoBehaviour
{
    [SerializeField]
    ResultScript ResultSet;

    private nn.account.Uid userId;
    private const string mountName = "MySave";
    private const string fileName = "MySaveData";
    private string filePath;
    private nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();

    private nn.hid.NpadState npadState;
    private nn.hid.NpadId[] npadIds = { nn.hid.NpadId.Handheld, nn.hid.NpadId.No1 };
    private const int saveDataSize = 256;
    private const int saveDataVersion = 1;



    void Start()
    {
        nn.account.Account.Initialize();
        nn.account.UserHandle userHandle = new nn.account.UserHandle();

        nn.account.Account.OpenPreselectedUser(ref userHandle);
        nn.account.Account.GetUserId(ref userId, userHandle);

        nn.Result result = nn.fs.SaveData.Mount(mountName, userId);
        result.abortUnlessSuccess();

        filePath = string.Format("{0}:/{1}", mountName, fileName);

        nn.hid.Npad.Initialize();
        nn.hid.Npad.SetSupportedStyleSet(nn.hid.NpadStyle.Handheld | nn.hid.NpadStyle.JoyDual);
        nn.hid.Npad.SetSupportedIdType(npadIds);
        npadState = new nn.hid.NpadState();
    }

    //void Update(){ }

    void OnDestroy()
    {
        nn.fs.FileSystem.Unmount(mountName);
    }

    public void Save(string[] player1, string[] player2, int[] score)
    {
        string saveData =
             player1[0] + "&" + player2[0] + "_" + score[0] + "%" + player1[1] + "&" + player2[1] + "_" + score[1] + "%" +
             player1[2] + "&" + player2[2] + "_" + score[2] + "%" + player1[3] + "&" + player2[3] + "_" + score[3] + "%" +
             player1[4] + "&" + player2[4] + "_" + score[4] + "%";

        byte[] data;
        using (MemoryStream stream = new MemoryStream(saveDataSize))
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(saveData);
            writer.Write(saveDataVersion);
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
        //  Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
    }

    public RankingDeta Load()
    {
        nn.fs.EntryType entryType = 0;
        nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
        if (nn.fs.FileSystem.ResultPathNotFound.Includes(result)) { return null; }
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

        string loadDeta;
        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(stream);
            loadDeta = reader.ReadString();
            int version = reader.ReadInt32();
            Debug.Assert(version == saveDataVersion);
        }
        RankingDeta ranking = new RankingDeta();

        char[] Cut = { '&', '_', };
        string[] CutDeta = new string[5];

        CutDeta = loadDeta.Split(new char[] { '%' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < 5; i++)
        {
            string[] splitDeta = CutDeta[i].Split(Cut, System.StringSplitOptions.RemoveEmptyEntries);
            ranking.name1[i] = splitDeta[0];
            ranking.name2[i] = splitDeta[1];
            ranking.score[i] = int.Parse(splitDeta[2]);
        }

        return ranking;
    }

}
