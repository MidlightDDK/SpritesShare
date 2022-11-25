using System;

public static class ResourcesManager
{
    // Get a general path
    private static string GetTPath<T>( string folderPath, T elementPath ) where T : Enum
    {
        return folderPath + "/" + Enum.GetName( typeof(T), elementPath );
    }

    // Get Prefabs
    public const string PrefabFolderPath = "Prefabs";
    public enum PrefabPath
    {
        Sprites,
    }

    public static string GetPrefabPath( PrefabPath prefabPath )
    {
        return GetTPath<PrefabPath>( PrefabFolderPath, prefabPath );
    }
    public const string MaterialMainTexture = "_MainTex";
    public const string MaterialMainColor = "_Color";
}
