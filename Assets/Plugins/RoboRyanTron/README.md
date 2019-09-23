# UnityEditorJunkie
Unity code that get you that sweet editor scripting fix. Unity did a pretty good job building out a massive editor... but there are a few holes. Lets fill them one by one.

## SearchableEnum
Use the SearchableEnumAttribute on an enum to get an improved enum selector popup. This has a text search box to filter the list, scrolls like a real scroll list, works with keyboard navigation, and focuses on the current selection when opening. 

Use it on something like Unity's KeyCode to be less likley to want to kill yourself when trying to pick something at the bottom of the list.

### Example
```csharp
public class SearchableEnumDemo : ScriptableObject
{
    [SearchableEnum]
    public KeyCode AwesomeKeyCode;
}
```
![popup image](https://user-images.githubusercontent.com/20144789/39614240-5e844c24-4f3c-11e8-998a-e0fbf969ddd4.gif)

## SceneReference
Loading a scene in Unity normally requires a string name or index which can easily break. By using a SceneReference on your MonoBehaviour, you can hard reference a scene and get validation features. 
