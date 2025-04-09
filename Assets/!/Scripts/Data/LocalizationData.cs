[System.Serializable]
public class LocalizationValue
{
    public string Lang;
    public string Value;
}

[System.Serializable]
public class LocalizationMapping
{
    public string Key;
    public LocalizationValue[] Values;
}

public class LocalizationData
{
    public string[] Languages;
    public LocalizationMapping[] Table;
}