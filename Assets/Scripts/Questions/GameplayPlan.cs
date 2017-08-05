using System;
using UnityEngine;
using System.Collections.Generic;

public class GameplayPlan : ScriptableObject
{
    public List<RoundGameplayPlan> RoundsList;
}

[Serializable]
public class RoundGameplayPlan
{
    public List<ThemesGameplayPlan> ThemesList;
}

[Serializable]
public class ThemesGameplayPlan
{
    public string ThemeName;
    public List<QuestionsGameplayPlan> QuestionsList;
}

[Serializable]
public class QuestionsGameplayPlan
{
    public int Price;
    public string Question;
    public string Answer;
    public Sprite Picture;
    public AudioClip Audio;

    public CatInPokeQuestion CatInPoke;

    public bool IsCatInPoke => !string.IsNullOrEmpty(CatInPoke.Theme) && CatInPoke.Price > 0;
}

[Serializable]
public class CatInPokeQuestion
{
    public string Theme;
    public int Price;
}