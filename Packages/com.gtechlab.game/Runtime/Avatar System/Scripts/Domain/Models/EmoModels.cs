using System;
using UnityEngine;

public enum EmotionCode {
    Undef,
    Angry,
    Happy,
    Frustration,
    Laugh,
    Clap,
    Greeting,
    Supriesed,
    Busy
}

[Serializable]
public class EmoDescModel {
    public EmotionCode emoId;
    public Sprite emoIcon;
    public string emoName;
}