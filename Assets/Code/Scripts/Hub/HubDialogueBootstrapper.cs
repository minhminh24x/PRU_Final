using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tự động tạo Canvas UI Hội Thoại cho HubDialogueManager.
/// Hỗ trợ cả NPC dialogue (có portrait) và Bệ Đá Cổ (lore chỉ có tên + text).
/// Chỉ cần gắn script này vào một GameObject trong Hub — không cần kéo thả gì thêm.
/// </summary>
public class HubDialogueBootstrapper : MonoBehaviour
{
    [Header("Font (tuỳ chọn)")]
    public TMP_FontAsset customFont;

    [Header("Màu sắc")]
    public Color panelBgColor    = new Color(0.06f, 0.04f, 0.12f, 0.95f);
    public Color nameBgColor     = new Color(0.18f, 0.10f, 0.30f, 1f);
    public Color bodyBgColor     = new Color(0.08f, 0.05f, 0.16f, 0.98f);
    public Color borderColor     = new Color(0.55f, 0.40f, 0.80f, 1f);
    public Color nameTextColor   = new Color(1.00f, 0.90f, 0.40f, 1f); // vàng cổ
    public Color bodyTextColor   = new Color(0.92f, 0.90f, 0.85f, 1f); // trắng ngà
    public Color promptTextColor = new Color(0.60f, 0.80f, 1.00f, 1f); // xanh nhạt

    void Awake()
    {
        if (HubDialogueManager.Instance != null && HubDialogueManager.Instance.dialoguePanel != null)
        {
            Destroy(gameObject);
            return;
        }
        BuildDialogueUI();
    }

    void BuildDialogueUI()
    {
        // ── Canvas ────────────────────────────────────────────────────────
        var canvasGO = new GameObject("[DialogueCanvas]");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 15;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Panel chính: dải băng phía dưới màn hình ─────────────────────
        var panel = MakeImage(canvasGO, "DialoguePanel", panelBgColor);
        var pr    = panel.GetComponent<RectTransform>();
        pr.anchorMin = new Vector2(0f,   0f);
        pr.anchorMax = new Vector2(1f,   0.30f);
        pr.offsetMin = pr.offsetMax = Vector2.zero;
        panel.SetActive(false);

        // Viền trên panel (trang trí)
        var border = MakeImage(panel, "TopBorder", borderColor);
        var br     = border.GetComponent<RectTransform>();
        br.anchorMin = new Vector2(0, 0.97f);
        br.anchorMax = new Vector2(1, 1f);
        br.offsetMin = br.offsetMax = Vector2.zero;

        // ── Portrait NPC (trái panel) ─────────────────────────────────────
        var portraitBg = MakeImage(panel, "PortraitBg", nameBgColor);
        var pbr        = portraitBg.GetComponent<RectTransform>();
        pbr.anchorMin = new Vector2(0.01f, 0.05f);
        pbr.anchorMax = new Vector2(0.12f, 0.92f);
        pbr.offsetMin = pbr.offsetMax = Vector2.zero;

        var portrait = MakeImage(portraitBg, "NpcPortrait", Color.white);
        StretchFull(portrait);
        portrait.GetComponent<Image>().preserveAspect = true;
        portraitBg.SetActive(false); // Bệ đá không cần portrait

        // ── Ô tên NPC / tên bệ đá ────────────────────────────────────────
        var nameBg  = MakeImage(panel, "NameBg", nameBgColor);
        var nbr     = nameBg.GetComponent<RectTransform>();
        nbr.anchorMin = new Vector2(0.01f, 0.75f);
        nbr.anchorMax = new Vector2(0.40f, 0.95f);
        nbr.offsetMin = nbr.offsetMax = Vector2.zero;

        var nameText = MakeText(nameBg, "NpcNameText", "???", 38,
                                TextAlignmentOptions.MidlineLeft);
        StretchWithPad(nameText, 10, 0, 10, 0);
        var nameTmp = nameText.GetComponent<TextMeshProUGUI>();
        nameTmp.color     = nameTextColor;
        nameTmp.fontStyle = FontStyles.Bold;

        // ── Thân hội thoại ────────────────────────────────────────────────
        var bodyBg  = MakeImage(panel, "BodyBg", bodyBgColor);
        var bbr     = bodyBg.GetComponent<RectTransform>();
        bbr.anchorMin = new Vector2(0.01f, 0.04f);
        bbr.anchorMax = new Vector2(0.97f, 0.74f);
        bbr.offsetMin = bbr.offsetMax = Vector2.zero;

        var bodyText = MakeText(bodyBg, "DialogueBodyText", "", 32,
                                TextAlignmentOptions.TopLeft);
        StretchWithPad(bodyText, 16, 8, 16, 8);
        var bodyTmp = bodyText.GetComponent<TextMeshProUGUI>();
        bodyTmp.color              = bodyTextColor;
        bodyTmp.enableWordWrapping = true;
        bodyTmp.overflowMode       = TextOverflowModes.Overflow;

        // ── Prompt [E] ────────────────────────────────────────────────────
        var promptText = MakeText(panel, "PromptText", "[E] Tiếp tục", 24,
                                  TextAlignmentOptions.BottomRight);
        var ptRect = promptText.GetComponent<RectTransform>();
        ptRect.anchorMin = new Vector2(0.75f, 0.02f);
        ptRect.anchorMax = new Vector2(0.97f, 0.18f);
        ptRect.offsetMin = ptRect.offsetMax = Vector2.zero;
        promptText.GetComponent<TextMeshProUGUI>().color = promptTextColor;

        // ── HubDialogueManager ────────────────────────────────────────────
        var mgrGO = new GameObject("[HubDialogueManager]");
        var mgr   = mgrGO.AddComponent<HubDialogueManager>();

        mgr.dialoguePanel    = panel;
        mgr.npcNameText      = nameTmp;
        mgr.npcPortrait      = portrait.GetComponent<Image>();
        mgr.dialogueBodyText = bodyTmp;
        mgr.promptText       = promptText.GetComponent<TextMeshProUGUI>();
        mgr.typingSpeed      = 0.03f;

        // Tìm PlayerUICanvas trong scene để gán (ẩn HUD khi đang dialogue)
        var hud = GameObject.Find("PlayerUICanvas");
        if (hud != null) mgr.playerHUDCanvas = hud;

        Debug.Log("[HubDialogueBootstrapper] HubDialogue UI đã được tạo!");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    GameObject MakeImage(GameObject parent, string name, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = color;
        return go;
    }

    GameObject MakeText(GameObject parent, string name, string text,
                        float size, TextAlignmentOptions align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text               = text;
        tmp.fontSize           = size;
        tmp.alignment          = align;
        tmp.color              = Color.white;
        tmp.enableWordWrapping = true;
        if (customFont != null) tmp.font = customFont;
        return go;
    }

    void StretchFull(GameObject go)
    {
        var r = go.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = r.offsetMax = Vector2.zero;
    }

    void StretchWithPad(GameObject go, float l, float b, float r, float t)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(l, b);
        rt.offsetMax = new Vector2(-r, -t);
    }
}
