using Cysharp.Threading.Tasks;
using GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Ist { get; private set; }
        public GameObject instructionPanelGo, selectPanelGo;
        public TextMeshProUGUI hitTxt, addHpTxt, addGoldTxt, attackTxt, defenceTxt, healthTxt, scoreTxt;
        public TextMeshProUGUI select1Txt, select2Txt, select3Txt;
        public Button select1Btn, select2Btn, select3Btn;
        public Slider healthSlider;
        public int attack;
        public int score;
        public int defence;
        public int health;
        public int selectedCount;
        private void Awake()
        {
            if (null != Ist)
            {
                Destroy(this);
                return;
            }
            Ist = this;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            attack = defence = 10;
            attackTxt.text = attack.ToString();
            defenceTxt.text = defence.ToString();
            health = 100;
            healthTxt.text = health.ToString();
            healthSlider.value = health;
            if (addHpTxt) addHpTxt.gameObject.SetActive(false);
            if (addGoldTxt) addGoldTxt.gameObject.SetActive(false);
            if (instructionPanelGo) instructionPanelGo.SetActive(false);
            if (selectPanelGo) selectPanelGo.SetActive(false);
            if (select1Btn)
            {
                select1Btn.onClick.AddListener(SelectAttack);
                select1Btn.gameObject.SetActive(false);
            }

            if (select2Btn)
            {
                select2Btn.onClick.AddListener(SelectDefence);
                select2Btn.gameObject.SetActive(false);
            }

            if (select3Btn)
            {
                select3Btn.onClick.AddListener(SelectHealth);
                select3Btn.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                instructionPanelGo.SetActive(!instructionPanelGo.activeSelf);
                Time.timeScale = instructionPanelGo.activeSelf ? 0 : 1;
            }
            ShowSelectPanel();
        }

        private void SelectHealth()
        {
            select3Btn.enabled = false;
            health = Mathf.Min(health + int.Parse(select3Txt.text), 100);
            healthTxt.text = health.ToString();
            healthSlider.value = health;
            selectPanelGo.SetActive(false);
            select3Btn.enabled = true;
            m_IsShowing = false;
        }
        private void SelectDefence()
        {
            select2Btn.enabled = false;
            defence = Mathf.Min(defence + int.Parse(select2Txt.text), 600);
            defenceTxt.text = defence.ToString();
            selectPanelGo.SetActive(false);
            select2Btn.enabled = true;
            m_IsShowing = false;
        }
        private void SelectAttack()
        {
            select1Btn.enabled = false;
            attack = Mathf.Min(attack + int.Parse(select1Txt.text), 600);
            attackTxt.text = attack.ToString();
            selectPanelGo.SetActive(false);
            select1Btn.enabled = true;
            m_IsShowing = false;
        }

        private bool m_IsShowing;
        public void ShowSelectPanel(bool isChest = false)
        {
            if (m_IsShowing) return;
            if (!isChest && (score < 1 || score / 100 <= selectedCount)) return;
            m_IsShowing = true;
            if (!isChest) selectedCount += 1;
            var r1 = Random.Range(0, 3);
            switch (r1)
            {
                case 0:
                    select3Btn.gameObject.SetActive(false);
                    select1Txt.text = Random.Range((int)GameManager.Ist.curScene, (int)GameManager.Ist.curScene * 2).ToString();
                    select2Txt.text = Random.Range((int)GameManager.Ist.curScene, (int)GameManager.Ist.curScene * 2).ToString();
                    select1Btn.gameObject.SetActive(true);
                    select2Btn.gameObject.SetActive(true);
                    break;
                case 1:
                    select1Btn.gameObject.SetActive(false);
                    select2Txt.text = Random.Range((int)GameManager.Ist.curScene, (int)GameManager.Ist.curScene * 2).ToString();
                    select3Txt.text = Random.Range((int)GameManager.Ist.curScene, (int)GameManager.Ist.curScene * 2).ToString();
                    select2Btn.gameObject.SetActive(true);
                    select3Btn.gameObject.SetActive(true);
                    break;
                case 2:
                    select2Btn.gameObject.SetActive(false);
                    select3Txt.text = Random.Range((int)GameManager.Ist.curScene, (int)GameManager.Ist.curScene * 2).ToString();
                    select1Txt.text = Random.Range((int)GameManager.Ist.curScene, (int)GameManager.Ist.curScene * 2).ToString();
                    select3Btn.gameObject.SetActive(true); 
                    select1Btn.gameObject.SetActive(true);
                    break;
            }
            selectPanelGo.SetActive(true);
        }
        public async UniTaskVoid HitNumber(Vector3 pos, int value = 10)
        {
            var txt = Instantiate(hitTxt, Vector3.zero, Quaternion.identity, hitTxt.transform.parent);
            txt.transform.localPosition = pos;
            txt.text = value.ToString();
            txt.gameObject.SetActive(true);
            while (txt.rectTransform.anchoredPosition.y < 200)
            {
                txt.rectTransform.anchoredPosition += Vector2.up * 600f * Time.fixedDeltaTime;
                await UniTask.WaitForSeconds(Time.fixedDeltaTime);
            }
            Destroy(txt.gameObject);
        }
        public async UniTaskVoid AddScore(int value = 10)
        {
            score += value;
            scoreTxt.text = score.ToString();
            var txt = Instantiate(addGoldTxt, Vector3.zero, Quaternion.identity, addGoldTxt.transform.parent);
            txt.transform.localPosition = Vector3.zero;
            txt.text = $"+{value} Gold";
            txt.gameObject.SetActive(true);
            while (txt.rectTransform.anchoredPosition.y < 200)
            {
                txt.rectTransform.anchoredPosition += Vector2.up * 600f * Time.fixedDeltaTime;
                await UniTask.WaitForSeconds(Time.fixedDeltaTime);
            }
            Destroy(txt.gameObject);
        }
        public async UniTaskVoid AddHp(int value = 10)
        {
            health = Mathf.Min(health + value, 100);
            var txt = Instantiate(addHpTxt, Vector3.zero, Quaternion.identity, addHpTxt.transform.parent);
            txt.transform.localPosition = Vector3.zero;
            txt.text = $"+{value} Hp";
            txt.gameObject.SetActive(true);
            while (txt.rectTransform.anchoredPosition.y < 200)
            {
                txt.rectTransform.anchoredPosition += Vector2.up * 600f * Time.fixedDeltaTime;
                await UniTask.WaitForSeconds(Time.fixedDeltaTime);
            }
            Destroy(txt.gameObject);
            healthSlider.value = health;
            healthTxt.text = health.ToString();
        }

        public void ChangeHealth(int damage)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, 100);
            healthSlider.value = health;
            healthTxt.text = health.ToString();
        }
    }
}