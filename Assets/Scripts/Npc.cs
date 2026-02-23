using System.Threading;
using Cysharp.Threading.Tasks;
using GamePlay;
using UI;
using UnityEngine;
using UnityEngine.UI;


public class Npc : MonoBehaviour
{
    public Sprite swordSprite, spearSprite;
    public RectTransform npcDialogPanel, playerDialogPanel;
    public Text npcTxt, playerTxt, playerNameTxt;
    public Image playerHead;
    private const string D1Npc = "The Last Oasis... that name gets people killed."; 
    private const string D2Player = "You've been there?"; 
    private const string D3Npc = "I've seen what's left of those who tried."; 
    private const string D4Npc = "What Can I do for U?"; 
    private const string D5Player = "Nothing."; 
    private const string D6Npc = "Be careful, the wolf becomes faster and danger."; 
    private const string D7Npc = "The boss is very different."; 
    private const string D8Npc = "This is the final level, the boss can follow and attack you."; 
    private CancellationTokenSource m_Cancel;
    private PlayerName m_CurPlayer;
    private string m_FullStr = string.Empty;
    private Text m_CurText;
    [SerializeField] private bool m_PlayerInRange, m_IsDialogPlaying;
    // private DialogueManager dialogueManager;

    void Start()
    {
        m_IsDialogPlaying = m_PlayerInRange = false;
    }

    void Update()
    {
        if (!m_IsDialogPlaying && Input.GetKeyDown(KeyCode.Space)) NextDialog();
        if (m_PlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (m_IsDialogPlaying) return;
            GameUI.Ist.gameObject.SetActive(false);
            var str = GameManager.Ist.curScene switch
            {
                SceneName.Level1 => D1Npc,
                SceneName.Level2 => D4Npc,
                SceneName.Level3 => D6Npc,
                SceneName.Level4 => D7Npc,
                SceneName.Level5 => D8Npc,
                _ => D8Npc
            };
            ShowDialog(str).Forget();
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) m_PlayerInRange = true;
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) m_PlayerInRange = false;
    }
    private void NextDialog()
    {
        if (m_IsDialogPlaying) return;
        DialogEnd(true).Forget();
    }
    private async UniTaskVoid ShowDialog(string textStr, PlayerName playerName = PlayerName.Npc)
    {
        Debug.Log($"ShowDialog, str: {textStr}");
        m_FullStr = textStr;
        m_Cancel = new CancellationTokenSource();
        if (m_CurPlayer != playerName)
        {
            npcDialogPanel.gameObject.SetActive(false);
            playerDialogPanel.gameObject.SetActive(false);
            m_CurPlayer = playerName;
        }
        switch (playerName)
        {
            case PlayerName.Npc:
                npcDialogPanel.gameObject.SetActive(true);
                m_CurText = npcTxt; 
                break;
            case PlayerName.Player:
                playerHead.sprite = GameManager.Ist.playerCharacter == "sword" ? swordSprite : spearSprite;
                playerDialogPanel.gameObject.SetActive(true);
                playerNameTxt.text = $"{GameManager.Ist.playerName}:";
                m_CurText = playerTxt;
                break;
        } 
        await ShowText(m_Cancel.Token);
        m_IsDialogPlaying = false;
        DialogEnd().Forget();
    }
        private async UniTaskVoid DialogEnd(bool isNext = false)
        {
            if (string.IsNullOrEmpty(m_FullStr)) return;
            if (m_FullStr.Equals(D3Npc))
            {
                await UniTask.WaitForSeconds(1f);
                npcDialogPanel.gameObject.SetActive(false);
                GameUI.Ist.gameObject.SetActive(true);
            }    
            if (m_FullStr.Equals(D5Player) || m_FullStr.Equals(D6Npc) || m_FullStr.Equals(D7Npc) || m_FullStr.Equals(D8Npc))
            {
                await UniTask.WaitForSeconds(1f);
                npcDialogPanel.gameObject.SetActive(false);
                playerDialogPanel.gameObject.SetActive(false);
                GameUI.Ist.gameObject.SetActive(true);
            }
            if (!isNext) return;
            switch (m_FullStr)
            {
                case D1Npc:
                    ShowDialog(D2Player, PlayerName.Player).Forget();
                    break;
                case D2Player: 
                    ShowDialog(D3Npc).Forget();
                    break;  
                case D4Npc:
                    ShowDialog(D5Player, PlayerName.Player).Forget();
                    break;
            }

        }
        private async UniTask<bool> ShowText(CancellationToken ct, float delayBetweenChars = 0.02f, float delayAfterPunctuation = 0.01f)
        {
            for (var i = 0; i < m_FullStr.Length; i++)
            {
                ct.ThrowIfCancellationRequested(); // 检查取消请求
                var currentTextStr = m_FullStr.Substring(0, i + 1);
                m_CurText.text = currentTextStr;
                // 遇到标点符号时稍作停顿
                if (i < m_FullStr.Length - 1 && IsPunctuation(m_FullStr[i]))
                {
                    await UniTask.WaitForSeconds(delayAfterPunctuation, cancellationToken: ct);
                }
                else
                {
                    await UniTask.WaitForSeconds(delayBetweenChars, cancellationToken: ct);
                }
            }
            return true;
        }
        private bool IsPunctuation(char character)
        {
            return character is ',' or '.' or '，' or '。' or '!' or '?' or '？' or '！' or '（' or '）' or '“' or '”';
        }
    
}
