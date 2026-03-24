using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tự động tạo toàn bộ Canvas UI Shop khi scene bắt đầu.
/// Chỉ cần gắn script này vào bất kỳ GameObject nào trong Hub.
/// Không cần kéo thả gì thêm — script tự sinh UI và kết nối ShopUIManager.
/// </summary>
public class ShopUIBootstrapper : MonoBehaviour
{
    [Header("Font (tuỳ chọn — để trống vẽ font mặc định)")]
    public TMP_FontAsset customFont;

    [Header("Màu sắc UI")]
    public Color panelBgColor    = new Color(0.08f, 0.06f, 0.14f, 0.96f);
    public Color headerColor     = new Color(0.15f, 0.10f, 0.25f, 1f);
    public Color rowBgColor      = new Color(0.13f, 0.09f, 0.20f, 1f);
    public Color buyBtnColor     = new Color(0.20f, 0.60f, 0.25f, 1f);
    public Color goldColor       = new Color(1.00f, 0.85f, 0.25f, 1f);

    void Awake()
    {
        // Nếu ShopUIManager đã có sẵn và đã setup đầy đủ → bỏ qua
        if (ShopUIManager.Instance != null
            && ShopUIManager.Instance.shopPanel != null
            && ShopUIManager.Instance.shopItemRowPrefab != null
            && ShopUIManager.Instance.itemListParent != null)
        {
            Destroy(gameObject);
            return;
        }

        // Nếu có Instance cũ nhưng thiếu references quan trọng → hủy dứt điểm để tạo lại cái mới
        if (ShopUIManager.Instance != null)
        {
            Debug.Log("[ShopUIBootstrapper] Phát hiện ShopUIManager cũ không đầy đủ, đang dọn dẹp...");
            DestroyImmediate(ShopUIManager.Instance.gameObject);
        }

        BuildShopUI();
    }

    void BuildShopUI()
    {
        // ── Canvas ────────────────────────────────────────────────────────
        var canvasGO = new GameObject("[ShopCanvas]");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Panel chính ───────────────────────────────────────────────────
        var panel     = MakeImage(canvasGO, "ShopPanel", panelBgColor);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.05f);
        panelRect.anchorMax = new Vector2(0.9f, 0.95f);
        panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
        panel.SetActive(false);

        // ── Header ────────────────────────────────────────────────────────
        var header = MakeImage(panel, "Header", headerColor);
        SetRect(header, 0, 0.82f, 1, 1);

        // Portrait
        var portrait     = MakeImage(header, "ShopkeeperPortrait", Color.white);
        var portraitRect = portrait.GetComponent<RectTransform>();
        portraitRect.anchorMin = new Vector2(0.01f, 0.05f);
        portraitRect.anchorMax = new Vector2(0.08f, 0.95f);
        portraitRect.offsetMin = portraitRect.offsetMax = Vector2.zero;
        portrait.GetComponent<Image>().preserveAspect = true;

        // NPC name
        var npcNameTxt = MakeText(header, "ShopkeeperName", "Sena", 36, TextAlignmentOptions.MidlineLeft);
        SetRect(npcNameTxt, 0.09f, 0.55f, 0.70f, 0.95f);
        npcNameTxt.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Greeting
        var greetingTxt = MakeText(header, "GreetingText", "Mại vô!", 24, TextAlignmentOptions.MidlineLeft);
        SetRect(greetingTxt, 0.09f, 0.05f, 0.70f, 0.52f);
        greetingTxt.GetComponent<TextMeshProUGUI>().color = new Color(0.85f, 0.85f, 0.85f);

        // Gold
        var goldTxt = MakeText(header, "PlayerGoldText", "Gold: 0 G", 26, TextAlignmentOptions.MidlineRight);
        SetRect(goldTxt, 0.68f, 0.1f, 0.99f, 0.9f);
        goldTxt.GetComponent<TextMeshProUGUI>().color = goldColor;

        // ── ScrollView ────────────────────────────────────────────────────
        var scrollGO  = new GameObject("ScrollView");
        scrollGO.transform.SetParent(panel.transform, false);
        var scrollRect = scrollGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        SetRect(scrollGO, 0, 0.10f, 1, 0.82f);
        // ScrollView root KHÔNG cần Mask — chỉ Viewport mới Mask

        var viewportGO = new GameObject("Viewport");
        viewportGO.transform.SetParent(scrollGO.transform, false);
        StretchFull(viewportGO);
        viewportGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.01f); // alpha > 0 để Mask hoạt động
        viewportGO.AddComponent<Mask>().showMaskGraphic = false;

        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(viewportGO.transform, false);
        var contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0,1);
        contentRect.anchorMax = new Vector2(1,1);
        contentRect.pivot     = new Vector2(0.5f,1);
        contentRect.offsetMin = contentRect.offsetMax = Vector2.zero;
        var vlg = contentGO.AddComponent<VerticalLayoutGroup>();
        vlg.padding   = new RectOffset(8,8,8,8);
        vlg.spacing   = 6;
        vlg.childControlWidth  = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        contentGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportGO.GetComponent<RectTransform>();
        scrollRect.content  = contentRect;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        // ── Feedback text ─────────────────────────────────────────────────
        var feedbackTxt = MakeText(panel, "FeedbackText", "", 20, TextAlignmentOptions.Center);
        SetRect(feedbackTxt, 0.05f, 0.01f, 0.95f, 0.10f);

        // ── Nút đóng ─────────────────────────────────────────────────────
        var closeBtn = MakeButton(panel, "CloseBtn", "[E] Đóng");
        var closeBtnRect = closeBtn.GetComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(0.80f, 0.01f);
        closeBtnRect.anchorMax = new Vector2(0.99f, 0.09f);
        closeBtnRect.offsetMin = closeBtnRect.offsetMax = Vector2.zero;
        closeBtn.GetComponent<Button>().onClick.AddListener(() => ShopUIManager.Instance?.CloseShop());

        // ── ItemRow Template (prefab nội bộ) ─────────────────────────────
        var rowTemplate = BuildItemRowTemplate();

        // ── ShopUIManager ─────────────────────────────────────────────────
        var mgrGO = new GameObject("[ShopUIManager]");
        var mgr   = mgrGO.AddComponent<ShopUIManager>();

        mgr.shopPanel            = panel;
        mgr.shopkeeperNameText   = npcNameTxt .GetComponent<TextMeshProUGUI>();
        mgr.shopkeeperPortrait   = portrait   .GetComponent<Image>();
        mgr.greetingText         = greetingTxt.GetComponent<TextMeshProUGUI>();
        mgr.playerGoldText       = goldTxt    .GetComponent<TextMeshProUGUI>();
        mgr.itemListParent       = contentRect;
        mgr.shopItemRowPrefab    = rowTemplate;
        mgr.feedbackText         = feedbackTxt.GetComponent<TextMeshProUGUI>();

        Debug.Log("[ShopUIBootstrapper] Shop UI đã được tạo tự động!");
    }

    // ─── Template hàng item ───────────────────────────────────────────────────

    GameObject BuildItemRowTemplate()
    {
        var row = new GameObject("ShopItemRow_Template");
        row.SetActive(false); // ẩn — chỉ dùng làm nguồn Instantiate

        var layoutEl = row.AddComponent<LayoutElement>();
        layoutEl.preferredHeight = 80;

        var bg = row.AddComponent<Image>();
        bg.color = rowBgColor;

        // Icon
        var iconGO = MakeImage(row, "ItemIcon", Color.white);
        SetRectAnchored(iconGO, 0.01f, 0.1f, 0.09f, 0.9f);
        iconGO.GetComponent<Image>().preserveAspect = true;

        // Name
        var nameGO = MakeText(row, "ItemName", "Item", 26, TextAlignmentOptions.MidlineLeft);
        SetRectAnchored(nameGO, 0.10f, 0.55f, 0.55f, 0.95f);
        nameGO.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Description
        var descGO = MakeText(row, "ItemDesc", "", 20, TextAlignmentOptions.MidlineLeft);
        SetRectAnchored(descGO, 0.10f, 0.05f, 0.55f, 0.52f);
        descGO.GetComponent<TextMeshProUGUI>().color = new Color(0.75f,0.75f,0.75f);

        // Stock
        var stockGO = MakeText(row, "StockText", "∞", 22, TextAlignmentOptions.Center);
        SetRectAnchored(stockGO, 0.55f, 0.1f, 0.70f, 0.9f);

        // Price
        var priceGO = MakeText(row, "PriceText", "50 G", 24, TextAlignmentOptions.Center);
        SetRectAnchored(priceGO, 0.70f, 0.1f, 0.80f, 0.9f);
        priceGO.GetComponent<TextMeshProUGUI>().color = goldColor;

        // Buy button
        var btnGO = MakeButton(row, "BuyButton", "Mua");
        SetRectAnchored(btnGO, 0.82f, 0.15f, 0.99f, 0.85f);
        btnGO.GetComponent<Image>().color = buyBtnColor;

        return row;
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    GameObject MakeImage(GameObject parent, string name, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        go.AddComponent<RectTransform>();
        return go;
    }

    GameObject MakeText(GameObject parent, string name, string text,
                        float fontSize, TextAlignmentOptions align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.alignment = align;
        tmp.color     = Color.white;
        if (customFont != null) tmp.font = customFont;
        tmp.enableWordWrapping = true;
        return go;
    }

    GameObject MakeButton(GameObject parent, string name, string label)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.25f, 0.25f, 0.35f);
        go.AddComponent<Button>();

        var lblGO = MakeText(go, "Label", label, 18, TextAlignmentOptions.Center);
        StretchFull(lblGO);
        return go;
    }

    void SetRect(GameObject go, float xMin, float yMin, float xMax, float yMax)
    {
        var r = go.GetComponent<RectTransform>();
        if (r == null) r = go.AddComponent<RectTransform>();
        r.anchorMin = new Vector2(xMin, yMin);
        r.anchorMax = new Vector2(xMax, yMax);
        r.offsetMin = r.offsetMax = Vector2.zero;
    }

    void SetRectAnchored(GameObject go, float xMin, float yMin, float xMax, float yMax)
    {
        var r = go.GetComponent<RectTransform>();
        if (r == null) r = go.AddComponent<RectTransform>();
        r.anchorMin = new Vector2(xMin, yMin);
        r.anchorMax = new Vector2(xMax, yMax);
        r.offsetMin = r.offsetMax = Vector2.zero;
    }

    void StretchFull(GameObject go)
    {
        var r = go.GetComponent<RectTransform>();
        if (r == null) r = go.AddComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = r.offsetMax = Vector2.zero;
    }
}
