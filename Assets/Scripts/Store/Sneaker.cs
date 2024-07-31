[System.Serializable]
public struct Sneaker
{
    public string name;
    public SneakerRarity rarity;
    public string imagePath;

    public Sneaker(string Name, SneakerRarity Rarity, string ImagePath)
    {
        name = Name;
        rarity = Rarity;
        imagePath = ImagePath;
    }
}