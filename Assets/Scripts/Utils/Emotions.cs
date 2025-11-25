using System.Collections.Generic;
using UnityEngine;

public static class Emotions
{
    public enum Emotion
    {
        Neutral,
        Joy,
        Sadness,
        Anger,
        Fear,
        Disgust
    }

    private static readonly Dictionary<Emotion, Color> EmotionColors = new Dictionary<Emotion, Color>()
    {
        { Emotion.Neutral, Color.white },
        { Emotion.Joy, Color.yellow },
        { Emotion.Sadness, Color.blue },
        { Emotion.Anger, Color.red },
        { Emotion.Fear, Color.purple },
        { Emotion.Disgust, Color.green }
    };

    public static Color GetColor(Emotion emotion)
    {
        return EmotionColors[emotion];
    }
}
