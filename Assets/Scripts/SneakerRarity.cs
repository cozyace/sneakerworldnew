using System;

public enum SneakerRarity
{
    Common = 1,
    Uncommon = 2,
    Rare = 3,
    Epic = 4,
    Legendary = 5
}

public static class SneakerRarityExtension
{
    public static T ToEnum<T>(this String enumValue)
    {
        return (T) Enum.Parse(typeof(T), enumValue);
    }
}