using System.Collections.Generic;
using UnityEngine;

public static class Emotions
{
    public enum Emotion
    {
        Impulse,
        Joy,
        Anger,
        Disgust,
        Sadness,
        Fear
    }

    private static readonly Dictionary<Emotion, Color> EmotionColors = new Dictionary<Emotion, Color>()
    {
        { Emotion.Impulse, Color.white },
        { Emotion.Joy, Color.yellow },
        { Emotion.Anger, Color.red },
        { Emotion.Disgust, Color.green },
        { Emotion.Sadness, Color.blue },
        { Emotion.Fear, Color.purple }
    };

    public static Color GetColor(Emotion emotion)
    {
        return EmotionColors[emotion];
    }
}
