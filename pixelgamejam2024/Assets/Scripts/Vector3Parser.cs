using UnityEngine;

public static class Vector3Parser
{
    public static bool TryParse(string vectorString, out Vector3 result)
    {
        result = Vector3.zero;

        // Remove parentheses and split the string by commas
        string[] sArray = vectorString.Trim('(', ')').Split(',');

        if (sArray.Length != 3)
            return false; // Invalid format (not three components)

        if (float.TryParse(sArray[0], out float x) &&
            float.TryParse(sArray[1], out float y) &&
            float.TryParse(sArray[2], out float z))
        {
            result = new Vector3(x, y, z);
            return true; // Successfully parsed
        }

        return false; // Parsing failed
    }
}