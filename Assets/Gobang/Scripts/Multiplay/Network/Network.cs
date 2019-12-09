﻿using UnityEngine;

public class Network : MonoBehaviour
{
    private Network() { }
    public static Network Instance { get; private set; }

    /// <summary>
    /// 注册
    /// </summary>
    public void EnrollRequest(string name)
    {
        Enroll request = new Enroll();
        request.username = name;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.Enroll, data);
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public void CreatRoomRequest(int roomId)
    {
        CreateRoom request = new CreateRoom();
        request.roomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.CreateRoom, data);
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    public void EnterRoomRequest(int roomId)
    {
        EnterRoom request = new EnterRoom();
        request.roomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.EnterRoom, data);
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    public void ExitRoomRequest(int roomId)
    {
        ExitRoom request = new ExitRoom();
        request.roomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.ExitRoom, data);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGameRequest(int roomId)
    {
        StartGame request = new StartGame();
        request.roomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.StartGame, data);
    }

    /// <summary>
    /// 下棋请求
    /// </summary>
    public void PlayChessRequest(int roomId)
    {
        //进行棋盘操作检测
        Vec2 pos = NetworkGameplay.Instance.PlayChess();

        if (pos.X == -1)
        {
            return;
        }

        PlayGame request = new PlayGame();
        request.roomId = roomId;
        request.Chess = NetworkPlayer.Instance.Chess;
        request.X = pos.X;
        request.Y = pos.Y;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.PlayGame, data);
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        NetworkClient.Register(MessageType.HeartBeat, _Heartbeat);
        NetworkClient.Register(MessageType.Enroll, _Enroll);
        NetworkClient.Register(MessageType.CreateRoom, _CreatRoom);
        NetworkClient.Register(MessageType.EnterRoom, _EnterRoom);
        NetworkClient.Register(MessageType.ExitRoom, _ExitRoom);
        NetworkClient.Register(MessageType.StartGame, _StartGame);
        NetworkClient.Register(MessageType.PlayGame, _PlayChess);
    }


    private void _Heartbeat(byte[] data)
    {
        NetworkClient.Received = true;
        Debug.Log("收到心跳包回应");
    }

    private void _Enroll(byte[] data)
    {
        Enroll result = NetworkUtils.Deserialize<Enroll>(data);
        if (result.isSuc)
        {
            NetworkPlayer.Instance.OnNameChange(result.username);

            Info.Instance.Print("注册成功");
        }
        else
        {
            Info.Instance.Print("注册失败");
        }
    }

    private void _CreatRoom(byte[] data)
    {
        CreateRoom result = NetworkUtils.Deserialize<CreateRoom>(data);

        if (result.isSuc)
        {
            NetworkPlayer.Instance.OnRoomIdChange(result.roomId);

            Info.Instance.Print(string.Format("创建房间成功, 你的房间号是{0}", NetworkPlayer.Instance.RoomId));
        }
        else
        {
            Info.Instance.Print("创建房间失败");
        }
    }

    private void _EnterRoom(byte[] data)
    {
        EnterRoom result = NetworkUtils.Deserialize<EnterRoom>(data);

        if (result.result == EnterRoom.Result.Player)
        {
            Info.Instance.Print("加入房间成功, 你是一名玩家");
        }
        else if (result.result == EnterRoom.Result.Observer)
        {
            Info.Instance.Print("加入房间成功, 你是一名观察者");
        }
        else
        {
            Info.Instance.Print("加入房间失败");
            return;
        }

        //进入房间
        NetworkPlayer.Instance.OnRoomIdChange(result.roomId);
    }

    private void _ExitRoom(byte[] data)
    {
        ExitRoom result = NetworkUtils.Deserialize<ExitRoom>(data);

        if (result.isSuc)
        {
            //房间号变为默认
            NetworkPlayer.Instance.OnRoomIdChange(0);
            //玩家状态改变
            NetworkPlayer.Instance.OnPlayingChange(false);

            Info.Instance.Print("退出房间成功");
        }
        else
        {
            Info.Instance.Print("退出房间失败");
        }
    }

    private void _StartGame(byte[] data)
    {
        StartGame result = NetworkUtils.Deserialize<StartGame>(data);

        if (result.isSuc)
        {
            //开始游戏事件
            NetworkPlayer.Instance.OnPlayingChange(true);

            //观察者
            if (result.isWatch)
            {
                NetworkPlayer.Instance.OnStartGame(Chess.None);
            }
            //玩家
            else
            {
                //是否先手(先手执黑棋, 后手执白棋)
                if (result.isFirst)
                    NetworkPlayer.Instance.OnStartGame(Chess.Black);
                else
                    NetworkPlayer.Instance.OnStartGame(Chess.White);
            }
        }
        else
        {
            Info.Instance.Print("开始游戏失败");
        }
    }

    private void _PlayChess(byte[] data)
    {
        PlayGame result = NetworkUtils.Deserialize<PlayGame>(data);

        if (!result.isSuc)
        {
            Info.Instance.Print("下棋操作失败");
            return;
        }
        
        switch (result.Challenger)
        {
            case Chess.None:
                break;
            case Chess.Black:
                NetworkPlayer.Instance.OnPlayingChange(false);
                Info.Instance.Print("黑棋胜利");
                break;
            case Chess.White:
                NetworkPlayer.Instance.OnPlayingChange(false);
                Info.Instance.Print("白棋胜利");
                break;
            case Chess.Draw:
                NetworkPlayer.Instance.OnPlayingChange(false);
                Info.Instance.Print("平局");
                break;
        }

        //实例化棋子
        NetworkGameplay.Instance.InstChess(result.Chess, new Vec2(result.X, result.Y));
    }

}