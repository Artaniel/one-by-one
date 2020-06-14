using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EvilName", menuName = "ScriptableObject/EnemyName/EvilEnemyNames")]
public class EvilDictionary : ScriptableObject
{
    public virtual List<string> EvilNames()
    {
        return evilNames;
    }

    public List<string> evilNames = new List<string> {
        "Abomination", "Abuse", "Aggression", "Agony", "Affliction",
        "Bereavement", "Blame", "Betrayal", "Breakdown", "Bad news", "Bad feeling",
        "Catastrophe", "Chaos", "Conceit", "Conflict", "Crime", "Corruption", "Cheerless",
        "Danger", "Darkness", "Defect", "Derision", "Doubt", "Doom",
        "Dull", "Dolor", "Desperation", "Depression", "Dejection", "Desolation", "Devastation", "Does not matter",
        "Egotism", "Epidemic", "Exhausted", "Enragement", "Excruciation", "Envy", "Evil",
        "Fail", "Fallen hero", "Fake", "Fatal", "Fear", "Filth", "Fury", "Fall", "Frustration", "Fraud",
        "Gloomy", "Grumpy", "Grudge", "Grief", "Give up", "Go away!",
        "Hater", "Harassment", "Harmful", "Hatred", "Havoc", "Horrid",
        "Idiotic", "Ignore", "Impersonal", "Impolite", "Insanity",
        "Jaded", "Jarring", "Jeer", "Jobless",

        "Lack", "Lackadaisical", "Lawless", "Loneliness",
        "Mad", "Malaise", "Malign", "Manipulate", "Marginal", "Misery",
        "Mistake", "Madness", "Mirthless",
        "Negation", "Nervous", "Niggles", "Nonsense", "Not for me", "Never lucky",
        "Objection", "Obscene", "Obstruct", "Outrageous",
        "Pain", "Panic", "Paranoia", "Peeve", "Pollution", "Prejudice",
        "Quarrel", "Questionable",
        "Rabid", "Rage", "Rascal", "Repugnant",
        "Sad", "Sass", "Scam", "Scare", "Selfish", "Sorrow", "Sinister", "Spooky",
        "Tantrum", "Tease", "Terrible", "Tired", "Thwart",
        "Ugly","Uncaring", "Uncivil", "Unemployment", "Underdog", "Useless", "Unfair",
        "Vague", "Vain", "Venomous", "Vex", "Violent",
        "Wail", "Wasted", "Wrath", "Weak", "Wicked", "Who am I?",
        "Zap", "Zealous",
        ":(", ":'(", "-_-", "404", "...",

        "Coronavirus", "Covid", "Infection", "Desease", "Virus", "Pandemic", "Epidemy"
    };
}