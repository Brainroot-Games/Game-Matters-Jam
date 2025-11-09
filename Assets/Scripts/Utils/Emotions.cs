using System.Collections.Generic;
using UnityEngine;

public static class Emotions
{
    public enum Emotion
    {
        Impulse,
        Anger,
        Sadness,
        Disgust,
        Joy,
        Fear
    }

    private static readonly Dictionary<Emotion, Color> EmotionColors = new Dictionary<Emotion, Color>()
    {
        { Emotion.Impulse, Color.white },
        { Emotion.Anger, Color.red },
        { Emotion.Sadness, Color.blue },
        { Emotion.Disgust, Color.green },
        { Emotion.Joy, Color.yellow },
        { Emotion.Fear, Color.purple }
    };

    public static Color GetColor(Emotion emotion)
    {
        return EmotionColors[emotion];
    }
}
