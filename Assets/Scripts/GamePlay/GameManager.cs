using System;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GamePlay
{
    public enum PlayerName
    {
        Npc,
        Player,
    }
    public enum SceneName
    {
        GameStart = -1,
        TransitionScene = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        VictoryEnding = 6
    }
    public class GameManager : MonoBehaviour
    {
        public RectTransform canvas;
        public PlayerController player;
        public Room curRoom;
        public SceneName lastScene = SceneName.GameStart;
        public SceneName curScene = SceneName.GameStart;
        public AudioSource uiAudio, bgmAudio;
        public AudioClip startBgm, levelBgm, level5Bgm;
        public AudioClip coinAudio, foodAudio, heartAudio, hitAudio, dieAudio, clickAudio;
        public string playerName = PlayerName.Player.ToString();
        public string playerCharacter = "sword";
        public RankListSo rankList;
        public static GameManager Ist { get; private set; }
        private const string Formatter1 = "yyyy-MM-dd HH:mm";
        private const string Formatter2 = "yyMMddHHmmss";
        private SceneName _targetScene;


        private void Awake()
        {
            if (null != Ist)
            {
                Destroy(this);
                Destroy(gameObject);
                return;
            }
            Ist = this;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            SKipSplash();
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep; 
            if (uiAudio) uiAudio.volume = 0.5f;
            if (bgmAudio) InitBgmVolume();

            var dialogueSystem = FindFirstObjectByType<DialogueSystem>();
            if (dialogueSystem != null)
            {
                dialogueSystem.OnDialogueComplete += () =>
                {
                Debug.Log("GameManager: Dialogue finished. Proceeding to next scene...");
                NextScene();
                };
            }
            else
            {
            Debug.LogWarning("GameManager: No DialogueSystem found in scene to hook OnDialogueComplete.");
            }
        }

        private void InitBgmVolume()
        {
            var bgmVolume = PlayerPrefs.HasKey("BgmVolume") ? PlayerPrefs.GetFloat("BgmVolume") : 0.6f;
            bgmAudio.volume = bgmVolume;
        }

        private void Update()
        {
            if (!player) player = FindFirstObjectByType<PlayerController>();
            // Canvas
            if (!canvas) canvas = GameObject.FindWithTag("UiRoot")?.GetComponent<RectTransform>();
            if (!canvas) canvas = FindFirstObjectByType<CanvasScaler>()?.GetComponent<RectTransform>();
        }

        public void SaveScore()
        {
            if (!rankList.dict.ContainsKey(playerName) || rankList.dict[playerName] < GameUI.Ist.score)
            {
                rankList.dict[playerName] = GameUI.Ist.score;
            }
        }
        public void NextScene()
        {
            if (curScene == SceneName.Level5)
            {
                if (rankList.dict.ContainsKey(playerName) && rankList.dict[playerName] < GameUI.Ist.score)
                {
                    rankList.dict[playerName] = GameUI.Ist.score;
                }
                UIManager.ShowBox("Victory!", 2).Forget();
                GoToVictoryEnding().Forget();
                return;
            }
            lastScene = curScene;
            SceneName  nextScene  = curScene switch
            {
                SceneName.GameStart => SceneName.TransitionScene,
                SceneName.TransitionScene => SceneName.Level1,
                SceneName.Level1 => SceneName.Level2,
                SceneName.Level2 => SceneName.Level3,
                SceneName.Level3 => SceneName.Level4,
                SceneName.Level4 => SceneName.Level5,
                SceneName.Level5 => SceneName.VictoryEnding,
                SceneName.VictoryEnding => SceneName.GameStart,
                _ => throw new ArgumentOutOfRangeException()
            };

             if (curScene == SceneName.GameStart && nextScene == SceneName.TransitionScene)
            {
                curScene = nextScene;
                SceneManager.LoadScene("TransitionScene");
                return;
            }

            if (nextScene == SceneName.VictoryEnding)
            {
                curScene = nextScene;
                SceneManager.LoadScene("VictoryEnding");
                return;
            }
            _targetScene = nextScene;
            curScene = nextScene;
            SceneManager.LoadScene("Loading");
        }


        private async UniTaskVoid GoToVictoryEnding()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2)); 
            curScene = SceneName.VictoryEnding;
            SceneManager.LoadScene("VictoryEnding");
        }

        public void RegisterDialogueSystem(DialogueSystem system)
        {
        system.OnDialogueComplete += () =>
        {
            Debug.Log("GameManager: Dialogue finished (via Register). Proceeding to next scene...");
            NextScene();
        };
        }

        public void RegisterEndingSequence(EndingSequence sequence)
        {
            sequence.OnEndingDialogueComplete += () =>
            {
                Debug.Log("GameManager: Ending sequence complete. Returning to GameStart...");
                NextScene();
            };
        }

        private async UniTaskVoid Back2Start()
        {
            Debug.Log("[GameManager] Back2Start() called, loading GameStart...");
            await UniTask.WaitForSeconds(2);
            lastScene = curScene;
            curScene = SceneName.GameStart;
            SceneManager.LoadScene("GameStart");
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)] private static void SKipSplash()
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
    }
}