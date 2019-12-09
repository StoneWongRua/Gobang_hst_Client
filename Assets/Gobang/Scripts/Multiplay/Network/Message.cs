
using System;



    public enum MessageType
    {
        None, //空
        HeartBeat, //心跳
        Enroll, //注册
        CreateRoom, //创建房间
        EnterRoom, //加入房间
        ExitRoom, // 退出房间
        StartGame, //开始游戏
        PlayGame, //具体游戏开始
    }

    ///<summary>
    /// 玩家类型
    /// </summary>
    public enum Chess
    {
        None,  // 空
        Black, // 黑
        White, // 白

        Draw, //平局
        Null, //表示无结果(如操作失败情况下的返回值)
    }

    [Serializable]
    public class Enroll
    {
        public string username; //玩家姓名
        public bool isSuc; //是否成功
    }

    [Serializable]
    public class CreateRoom
    {
        public int roomId; //房间号码
        public bool isSuc;   //是否成功
    }

    [Serializable]
    public class EnterRoom
    {
        public int roomId;      //房间号码
        public Result result;   //结果

        public enum Result
        {
            None,
            Player,
            Observer,
        }
    }

    [Serializable]
    public class ExitRoom
    {
        public int roomId;  //房间号码
        public bool isSuc;    //是否成功
    }

    [Serializable]
    public class StartGame
    {
        public int roomId;            //房间号码

        public bool isSuc;              //是否成功
        public bool isFirst;            //是否先手
        public bool isWatch;            //是否是观察者
    }

    [Serializable]
    public class PlayGame
    {
        public int roomId;       //房间号码
        public Chess Chess;      //棋子类型
        public int X;            //棋子坐标
        public int Y;            //棋子坐标

        public bool isSuc;         //操作结果
        public Chess Challenger; //胜利者
    }
