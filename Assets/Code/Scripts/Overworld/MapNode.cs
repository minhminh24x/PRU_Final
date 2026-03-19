using UnityEngine;

public class MapNode : MonoBehaviour
{
    public MapNode upNode;
    public MapNode downNode;
    public MapNode leftNode;
    public MapNode rightNode;

    [Header("Node Information")]
    public string nodeName;
    [TextArea]
    public string nodeDescription;

    [Header("Scene Link")]
    [Tooltip("Tên scene mà node này sẽ load khi player tương tác")]
    public string sceneToLoad;
}
