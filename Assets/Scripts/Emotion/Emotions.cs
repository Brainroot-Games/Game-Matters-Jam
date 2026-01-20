using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private static readonly int emotionCount = Enum.GetNames(typeof(Emotion)).Length;

    private static readonly Dictionary<Emotion, Color> EmotionColors = new Dictionary<Emotion, Color>()
    {
        { Emotion.Neutral, Color.white },
        { Emotion.Joy, Color.yellow },
        { Emotion.Sadness, Color.blue },
        { Emotion.Anger, Color.red },
        { Emotion.Fear, Color.purple },
        { Emotion.Disgust, Color.green }
    };

    public static bool TryGetEmotionFromInput(string input, out Emotion emotion)
    {
        bool result = Enum.TryParse(input, out Emotion output);
        emotion = output;
        return result;
    }

    public static int GetEmotionIndex(Emotion emotion) =>
        (int)emotion;

    public static Color GetEmotionColor(Emotion emotion) =>
        EmotionColors[emotion];

    public static Emotion GetRandomEmotion() =>
        (Emotion)Random.Range(1, emotionCount);
}
