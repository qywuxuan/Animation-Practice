using UnityEditor;
using UnityEngine;

public class AnimationExtractor
{
    const bool deleteOriginFBX = false;

    [MenuItem("Assets/提取动画")]
    static void Extract()
    {
        var gameObjects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        int progressIndex = 0;

        foreach (Object gameObject in gameObjects)
        {
            var path = AssetDatabase.GetAssetPath(gameObject);
            var childAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(gameObject));
            EditorUtility.DisplayProgressBar("提取动画片段", path, progressIndex / (gameObjects.Length * 1f));

            foreach (var childAsset in childAssets)
            {
                if (childAsset is AnimationClip && !childAsset.name.Contains("__preview__"))
                {
                    var newClip = new AnimationClip();
                    EditorUtility.CopySerialized(childAsset, newClip);
                    newClip.name = gameObject.name;
                    AssetDatabase.CreateAsset(newClip, string.Format(path.Replace(".fbx", ".anim"), newClip.name));

                    if (deleteOriginFBX)
                    {
#pragma warning disable CS0162 // 检测到无法访问的代码
                        AssetDatabase.DeleteAsset(path);
#pragma warning restore CS0162 // 检测到无法访问的代码
                    }
                }
            }

            progressIndex++;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
}
