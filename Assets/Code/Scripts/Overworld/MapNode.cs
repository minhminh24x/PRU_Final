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

    [Header("Node Visual")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Kéo 4 sprite UI_Flat_Button02a_1 đến 4 vào đây")]
    public Sprite[] activeSprites; 
    public float animationSpeed = 0.15f;
    
    [Header("Node Locking")]
    public int requiredLevel = 1;
    public GameObject lockedVisual;
    
    private bool isPlayerOnNode = false;
    private float timer = 0f;
    private int currentFrame = 0;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Kiểm tra khóa ngay khi khởi tạo
        if (lockedVisual != null)
            lockedVisual.SetActive(IsLocked());
    }

    void Update()
    {
        if (isPlayerOnNode && activeSprites != null && activeSprites.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= animationSpeed)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % activeSprites.Length;
                spriteRenderer.sprite = activeSprites[currentFrame];
            }
        }
    }

    public void SetPlayerOnNode(bool isOn)
    {
        isPlayerOnNode = isOn;
        if (!isOn && activeSprites != null && activeSprites.Length > 0)
        {
            // Trả về trạng thái mặc định khi người chơi rời đi (frame đầu tiên)
            currentFrame = 0;
            spriteRenderer.sprite = activeSprites[0];
            timer = 0f;
        }
    }

    public bool IsLocked()
    {
        return GameSession.CurrentLevel < requiredLevel;
    }
}
