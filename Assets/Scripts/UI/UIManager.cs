using Cysharp.Threading.Tasks;
using GamePlay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public static class UIManager
    {
        private static GameObject m_CurView;

        public static async UniTaskVoid ShowBox(string txt, int duration = 1, bool additional = true)
        {
            var txtBoxView = OpenView("TxtBoxView", additional : additional);
            var txtBoxUI = txtBoxView.GetComponent<TxtBoxUI>();
            txtBoxUI.boxTxt.text = txt;
            await UniTask.WaitForSeconds(duration);
            Object.Destroy(txtBoxView);
        }
        public static GameObject OpenView(string viewAssetName, Transform parent = null, GameObject prefab = null, bool additional = false)
        {
            // 0、销毁其他的 View
            if (!parent && m_CurView && !additional) Object.Destroy(m_CurView);
            // 1、查找 Canvas 节点
            parent = !parent ? GameManager.Ist.canvas : parent;
            // 2、加载 viewPrefab
            var viewPrefab = prefab ?? Resources.Load<GameObject>("Views/" + viewAssetName);
            // 3、实例化 view
            var viewGo = Object.Instantiate(viewPrefab, Vector3.zero, Quaternion.identity, parent);
            viewGo.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            viewGo.transform.rotation = Quaternion.Euler(Vector3.zero);
            viewGo.transform.localScale = Vector3.one;
            viewGo.name = viewAssetName;
            viewGo.transform.SetAsLastSibling();
            viewGo.SetActive(true);
            if (parent && null != parent && !parent.gameObject.activeSelf) parent.gameObject.SetActive(true);
            if (!additional) m_CurView = viewGo; 
            return viewGo;
        }
        
        public static async UniTask<bool> ShowText(string fullStr, Text txt, float delayBetweenChars = 0.05f, float delayAfterPunctuation = 0.05f)
        {
            for (var i = 0; i < fullStr.Length; i++)
            {
                var currentTextStr = fullStr.Substring(0, i + 1);
                if (txt) txt.text = currentTextStr;
                // 遇到标点符号时稍作停顿
                if (i < fullStr.Length - 1 && IsPunctuation(fullStr[i]))
                {
                    await UniTask.WaitForSeconds(delayAfterPunctuation);
                }
                else
                {
                    await UniTask.WaitForSeconds(delayBetweenChars);
                }
            }
            return true;
        }
        private static bool IsPunctuation(char character)
        {
            return character is ',' or '.' or '，' or '。' or '!' or '?' or '？';
        }
    }
}