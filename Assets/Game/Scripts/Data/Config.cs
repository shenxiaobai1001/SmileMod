using System;
using System.Collections.Generic;

/// <summary>配置</summary>
public class Config
{
    public static int ClearType=1;
 
}

/// <summary>事件合集 </summary>
public enum Events
{
    None,
    GameRest,
    SaveSchedule,
    PlayerRestToSavePos,
    SevenBossDie,
    MapChanged,
    OnQLMove,
    OnTCMove,
    OnTrunckMove,
    OneFingerMove,
    RainBowCat,
    GaiyaTomato,
    GaiyaTomatoEnd,
}

public enum PlayerControState
{
    None,
    LRun, 
    LRuning,
    CanelLRun,
    RRun,
    RRuning,
    CanelRRun,
    ToFast,
    Fast,
    CancelFast,
    Jump, 
    Jumping,
    CJump,
    LStickJump,
    RStickJump,
    Boost,
    Boosting,
    CanelBoost,
}
public enum PLState
{
    None,
    Idel,
    LRun,
    RRun,
    FastLRun,
    FastRRun,
    BrakeF,
    FLBrake,
    FRBrake,
    Jump,
    LJumping,
    FLJumping,
    RJumping,
    FRJumping,
    Drop,
    LDroping,
    FLDroping,
    RDroping,
    FRDroping,
    LStickDrop,
    RStickDrop,
    Stick,
    StickJump,
    LStick,
    RStick,
    LStickJump,
    RStickJump,
    StickF,
    StickFJump,
    LStickF,
    RStickF,
    LStickFJump,
    RStickFJump,
    LBoost,
    RBoost,
    HorHit,
    VecHit,
    DownHit,
    HitOver
}

public enum PlayerCheckType
{
    None,
    BackGround,
    LeftWall,
    RightWall,
}
public enum AutomaticMoveType
{
    None,
    Run,
    FastRun,
    Jump,
    Fly,
}
