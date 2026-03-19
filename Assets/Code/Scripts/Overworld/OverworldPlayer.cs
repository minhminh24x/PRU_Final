using UnityEngine;
using UnityEngine.SceneManagement;
public class OverworldPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public MapNode currentNode;
    
    // --- CODE MỚI THÊM VÀO ---
    public OverworldUIManager uiManager; // Kéo thả UIManager vào đây
    // -------------------------

    private MapNode targetNode;
    private bool isMoving = false;

    void Start()
    {
        if (currentNode != null)
        {
            transform.position = currentNode.transform.position;
            // Cập nhật UI ngay khi vừa load scene cho Node đầu tiên
            if (uiManager != null) uiManager.UpdateNodeDisplay(currentNode.nodeName, currentNode.nodeDescription); 
        }
    }

void Update()
{
    if (isMoving)
    {
        MoveTowardsTarget();
        return;
    }

    HandleInput();

    // NHẤN SPACE / ENTER ĐỂ VÀO MÀN CHƠI
    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
    {
        if (currentNode != null && !string.IsNullOrEmpty(currentNode.sceneToLoad))
        {
            Debug.Log($"<color=cyan>Vào map: {currentNode.sceneToLoad}</color>");
            SceneManager.LoadScene(currentNode.sceneToLoad);
        }
    }
}

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) TryMove(currentNode.upNode);
        else if (Input.GetKeyDown(KeyCode.S)) TryMove(currentNode.downNode);
        else if (Input.GetKeyDown(KeyCode.A)) TryMove(currentNode.leftNode);
        else if (Input.GetKeyDown(KeyCode.D)) TryMove(currentNode.rightNode);
    }

    void TryMove(MapNode nextNode)
    {
        if (nextNode != null)
        {
            targetNode = nextNode;
            isMoving = true;
        }
    }

    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetNode.transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.01f)
        {
            transform.position = targetNode.transform.position; 
            currentNode = targetNode; 
            isMoving = false; 

            // --- CODE MỚI THÊM VÀO ---
            // Cập nhật UI khi đã đặt chân lên Node mới
            if (uiManager != null) uiManager.UpdateNodeDisplay(currentNode.nodeName, currentNode.nodeDescription);
            // -------------------------
        }
    }
}