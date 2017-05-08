using System;
using UnityEngine;
using System.Collections;
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
}